//MIT License
//Copyright(c) [2019] [Piotr Maciejewski]

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

internal class CorridorNode : Leaf
{
    private Leaf structure1;
    private Leaf structure2;
    private int corridorWidth;
    private int modifierDistanceFromWall = 1;

    public CorridorNode(Leaf leaf1, Leaf leaf2, int corridorWidth) : base(null)
    {
        this.structure1 = leaf1;
        this.structure2 = leaf2;
        this.corridorWidth = corridorWidth;
        GenerateCorridor();
    }

    private void GenerateCorridor()
    {
        var relativePositionOfStructure2 = CheckPositionStructure2AgainstStructure1();
        switch (relativePositionOfStructure2)
        {
            case RelativePosition.Up:
                ProcessRoomInRelationUpOrDown(this.structure1, this.structure2);
                break;
            case RelativePosition.Down:
                ProcessRoomInRelationUpOrDown(this.structure2, this.structure1);
                break;
            case RelativePosition.Right:
                ProcessRoomInRelationRightOrLeft(this.structure1, this.structure2);
                break;
            case RelativePosition.Left:
                ProcessRoomInRelationRightOrLeft(this.structure2, this.structure1);
                break;
            default:
                break;
        }
    }

    private void ProcessRoomInRelationRightOrLeft(Leaf structure1, Leaf structure2)
    {
        Leaf leftStructure = null;
        List<Leaf> leftStructureChildren = StructureHelper.TraverseGraphToExtractLowestLeaves(structure1);
        Leaf rightStructure = null;
        List<Leaf> rightStructureChildren = StructureHelper.TraverseGraphToExtractLowestLeaves(structure2);

        var sortedLeftStructure = leftStructureChildren.OrderByDescending(child => child.TopRightAreaCorner.x).ToList();
        if(sortedLeftStructure.Count == 1)
        {
            leftStructure = sortedLeftStructure[0];
        }
        else
        {
            int maxX = sortedLeftStructure[0].TopRightAreaCorner.x;
            sortedLeftStructure = sortedLeftStructure.Where(
                children => Math.Abs(maxX - children.TopRightAreaCorner.x) < 10).ToList();
            int index = UnityEngine.Random.Range(0, sortedLeftStructure.Count);
            leftStructure = sortedLeftStructure[index];
        } 

        var possibleNeighboursInRightStructureList = rightStructureChildren.Where(
            child => GetValidYForNeightbourLeftRight(
                leftStructure.TopRightAreaCorner,
                leftStructure.BottomRightAreaCorner,
                child.TopLeftAreaCorner,
                child.BottomLeftAreaCorner
                ) != -1
            ).OrderBy(child => child.BottomRightAreaCorner.x).ToList();
        if(possibleNeighboursInRightStructureList.Count <= 0)
        {
            rightStructure = structure2;
        }
        else
        {
            rightStructure = possibleNeighboursInRightStructureList[0];
        }
        int y = GetValidYForNeightbourLeftRight(leftStructure.TopLeftAreaCorner, leftStructure.BottomRightAreaCorner,
            rightStructure.TopLeftAreaCorner,
            rightStructure.BottomLeftAreaCorner);
        while(y == -1 && sortedLeftStructure.Count > 1)
        {
            sortedLeftStructure = sortedLeftStructure.Where(
                child => child.TopLeftAreaCorner.y != leftStructure.TopLeftAreaCorner.y).ToList();
            leftStructure = sortedLeftStructure[0];
            y = GetValidYForNeightbourLeftRight(leftStructure.TopLeftAreaCorner, leftStructure.BottomRightAreaCorner,
            rightStructure.TopLeftAreaCorner,
            rightStructure.BottomLeftAreaCorner);
        }
        BottomLeftAreaCorner = new Vector2Int(leftStructure.BottomRightAreaCorner.x, y);
        TopRightAreaCorner = new Vector2Int(rightStructure.TopLeftAreaCorner.x, y + this.corridorWidth);
    }

    private int GetValidYForNeightbourLeftRight(Vector2Int leftLeafUp, Vector2Int leftLeafDown, Vector2Int rightLeafUp, Vector2Int rightLeafDown)
    {
        if(rightLeafUp.y >= leftLeafUp.y && leftLeafDown.y >= rightLeafDown.y)
        {
            return StructureHelper.CalculateMiddlePoint(
                leftLeafDown + new Vector2Int(0, modifierDistanceFromWall),
                leftLeafUp - new Vector2Int(0, modifierDistanceFromWall + this.corridorWidth)
                ).y;
        }
        if(rightLeafUp.y < leftLeafUp.y && leftLeafDown.y <= rightLeafDown.y)
        {
            return StructureHelper.CalculateMiddlePoint(
               rightLeafDown + new Vector2Int(0,modifierDistanceFromWall),
               rightLeafUp - new Vector2Int(0,modifierDistanceFromWall + this.corridorWidth)
               ).y;
        }
        if(leftLeafUp.y <= rightLeafDown.y && leftLeafUp.y <= rightLeafUp.y)
        {
            return StructureHelper.CalculateMiddlePoint(
                rightLeafDown + new Vector2Int(0,modifierDistanceFromWall),
                leftLeafUp - new Vector2Int(0,modifierDistanceFromWall)
              ).y;
        }
        if(leftLeafDown.y >= rightLeafDown.y && leftLeafDown.y <= rightLeafUp.y)
        {
            return StructureHelper.CalculateMiddlePoint(
              leftLeafDown + new Vector2Int(0, modifierDistanceFromWall),
              rightLeafUp- new Vector2Int(0, modifierDistanceFromWall + this.corridorWidth)
              ).y;
        }
        return -1;
    }

    private void ProcessRoomInRelationUpOrDown(Leaf structure1, Leaf structure2)
    {
        Leaf bottomStructure = null;
        List<Leaf> structureBottomChildren = StructureHelper.TraverseGraphToExtractLowestLeaves(structure1);
        Leaf topStructure = null;
        List<Leaf> structureAboveChildren = StructureHelper.TraverseGraphToExtractLowestLeaves(structure2);

        var sortedBottomStructure = structureBottomChildren.OrderByDescending(child => child.TopRightAreaCorner.y).ToList();

        if (sortedBottomStructure.Count == 1)
        {
            bottomStructure = structureBottomChildren[0];
        }
        else
        {
            int maxY = sortedBottomStructure[0].TopLeftAreaCorner.y;
            sortedBottomStructure = sortedBottomStructure.Where(child => Mathf.Abs(maxY - child.TopLeftAreaCorner.y) < 10).ToList();
            int index = UnityEngine.Random.Range(0, sortedBottomStructure.Count);
            bottomStructure = sortedBottomStructure[index];
        }

        var possibleNeightboursInTopStructure = structureAboveChildren.Where(
            child => GetValidXForNeightbourUpDown(
                bottomStructure.TopLeftAreaCorner,
                bottomStructure.TopRightAreaCorner,
                child.BottomLeftAreaCorner,
                child.BottomRightAreaCorner)
            != -1).OrderBy(child => child.BottomRightAreaCorner.y).ToList();
        if(possibleNeightboursInTopStructure.Count == 0)
        {
            topStructure = structure2;
        }
        else
        {
            topStructure = possibleNeightboursInTopStructure[0];
        }
        int x = GetValidXForNeightbourUpDown(
                bottomStructure.TopLeftAreaCorner,
                bottomStructure.TopRightAreaCorner,
                topStructure.BottomLeftAreaCorner,
                topStructure.BottomRightAreaCorner);
        while(x == -1 && sortedBottomStructure.Count > 1)
        {
            sortedBottomStructure = sortedBottomStructure.Where(
                child => child.TopLeftAreaCorner.x != topStructure.TopLeftAreaCorner.x).ToList();
            bottomStructure = sortedBottomStructure[0];
            x = GetValidXForNeightbourUpDown(
                bottomStructure.TopLeftAreaCorner,
                bottomStructure.TopRightAreaCorner,
                topStructure.BottomLeftAreaCorner,
                topStructure.BottomRightAreaCorner);
        }
        BottomLeftAreaCorner = new Vector2Int(x, bottomStructure.TopLeftAreaCorner.y);
        TopRightAreaCorner = new Vector2Int(x + this.corridorWidth, topStructure.BottomLeftAreaCorner.y);
    }

    private int GetValidXForNeightbourUpDown(Vector2Int bottomLeafLeft, Vector2Int bottomLeafRight,
        Vector2Int topLeafLeft, Vector2Int topLeafRight)
    {
        if(topLeafLeft.x < bottomLeafLeft.x && bottomLeafRight.x < topLeafRight.x)
        {
            return StructureHelper.CalculateMiddlePoint(
                bottomLeafLeft + new Vector2Int(modifierDistanceFromWall, 0),
                bottomLeafRight - new Vector2Int(this.corridorWidth + modifierDistanceFromWall, 0)
                ).x;
        }
        if(topLeafLeft.x >= bottomLeafLeft.x && bottomLeafRight.x >= topLeafRight.x)
        {
            return StructureHelper.CalculateMiddlePoint(
                topLeafLeft+new Vector2Int(modifierDistanceFromWall,0),
                topLeafRight - new Vector2Int(this.corridorWidth+modifierDistanceFromWall,0)
                ).x;
        }
        if(bottomLeafLeft.x >= (topLeafLeft.x) && bottomLeafLeft.x <= topLeafRight.x)
        {
            return StructureHelper.CalculateMiddlePoint(
                bottomLeafLeft+ new Vector2Int(modifierDistanceFromWall,0),
                topLeafRight - new Vector2Int(this.corridorWidth+modifierDistanceFromWall,0)
                ).x;
        }
        if(bottomLeafRight.x <= topLeafRight.x && bottomLeafRight.x >= topLeafLeft.x)
        {
            return StructureHelper.CalculateMiddlePoint(
                topLeafLeft + new Vector2Int(modifierDistanceFromWall,0),
                bottomLeafRight - new Vector2Int(this.corridorWidth + modifierDistanceFromWall, 0)
                ).x;
        }
        return -1;
    }

    private RelativePosition CheckPositionStructure2AgainstStructure1()
    {
        Vector2 middlePointStructure1Temp = ((Vector2)structure1.TopRightAreaCorner + structure1.BottomLeftAreaCorner) / 2;
        Vector2 middlePointStructure2Temp = ((Vector2)structure2.TopRightAreaCorner + structure2.BottomLeftAreaCorner) / 2;
        float angle = CalculateAngle(middlePointStructure1Temp, middlePointStructure2Temp);
        if((angle < 45 && angle >=0) || (angle>-45 && angle < 0))
        {
            return RelativePosition.Right;
        }
        else if(angle > 45 && angle < 135)
        {
            return RelativePosition.Up;
        }
        else if(angle > -135 && angle < -45)
        {
            return RelativePosition.Down;
        }
        else
        {
            return RelativePosition.Left;
        }
    }

    private float CalculateAngle(Vector2 middlePointStructure1Temp, Vector2 middlePointStructure2Temp)
    {
        return Mathf.Atan2(middlePointStructure2Temp.y - middlePointStructure1Temp.y,
            middlePointStructure2Temp.x - middlePointStructure1Temp.x) * Mathf.Rad2Deg;
    }
}