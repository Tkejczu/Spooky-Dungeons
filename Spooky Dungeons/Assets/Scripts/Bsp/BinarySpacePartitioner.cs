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
using System.Collections.Generic;
using UnityEngine;

public class BinarySpacePartitioner
{
    public BinarySpacePartitioner(int dungeonWidth, int dungeonLenght)
    {
        RootLeaf = new RoomLeaf(new Vector2Int(0, 0), new Vector2Int(dungeonWidth, dungeonLenght), null, 0);
    }

    public RoomLeaf RootLeaf { get; }

    public List<RoomLeaf> PrepareLeavesCollection(int maxIterations, int roomWidthMin, int roomLenghtMin)
    {
        var graph = new Queue<RoomLeaf>();
        var listToReturn = new List<RoomLeaf>();
        graph.Enqueue(RootLeaf);
        listToReturn.Add(RootLeaf);
        var iterations = 0;
        while (iterations < maxIterations && graph.Count > 0)
        {
            iterations++;
            var currentLeaf = graph.Dequeue();
            if (currentLeaf.Width >= roomWidthMin * 2 || currentLeaf.Length >= roomLenghtMin * 2)
                SplitTheSpace(currentLeaf, listToReturn, roomLenghtMin, roomWidthMin, graph);
        }

        return listToReturn;
    }

    public void SplitTheSpace(RoomLeaf currentLeaf, List<RoomLeaf> listToReturn, int roomLenghtMin, int roomWidthMin,
        Queue<RoomLeaf> graph)
    {
        var line = GetLineDividingSpace(
            currentLeaf.BottomLeftAreaCorner,
            currentLeaf.TopRightAreaCorner,
            roomWidthMin,
            roomLenghtMin);
        RoomLeaf leaf1, leaf2;
        if (line.Orientation == Orientation.Horizontal)
        {
            leaf1 = new RoomLeaf(currentLeaf.BottomLeftAreaCorner,
                new Vector2Int(currentLeaf.TopRightAreaCorner.x, line.Coordinates.y),
                currentLeaf,
                currentLeaf.TreeLayerIndex + 1);
            leaf2 = new RoomLeaf(new Vector2Int(currentLeaf.BottomLeftAreaCorner.x, line.Coordinates.y),
                currentLeaf.TopRightAreaCorner,
                currentLeaf,
                currentLeaf.TreeLayerIndex + 1);
        }
        else
        {
            leaf1 = new RoomLeaf(currentLeaf.BottomLeftAreaCorner,
                new Vector2Int(line.Coordinates.x, currentLeaf.TopRightAreaCorner.y),
                currentLeaf,
                currentLeaf.TreeLayerIndex + 1);
            leaf2 = new RoomLeaf(new Vector2Int(line.Coordinates.x, currentLeaf.BottomLeftAreaCorner.y),
                currentLeaf.TopRightAreaCorner,
                currentLeaf,
                currentLeaf.TreeLayerIndex + 1);
        }

        AddNewLeafToCollections(listToReturn, graph, leaf1);
        AddNewLeafToCollections(listToReturn, graph, leaf2);
    }

    public void AddNewLeafToCollections(List<RoomLeaf> listToReturn, Queue<RoomLeaf> graph, RoomLeaf leaf)
    {
        listToReturn.Add(leaf);
        graph.Enqueue(leaf);
    }

    public Line GetLineDividingSpace(Vector2Int bottomLeftAreaCorner, Vector2Int topRightAreaCorner, int roomWidthMin,
        int roomLenghtMin)
    {
        Orientation orientation;
        var lengthStatus = topRightAreaCorner.y - bottomLeftAreaCorner.y >= 2 * roomLenghtMin;
        var widthStatus = topRightAreaCorner.x - bottomLeftAreaCorner.x >= 2 * roomWidthMin;
        if (lengthStatus && widthStatus)
            orientation = (Orientation) Random.Range(0, 2);
        else if (widthStatus)
            orientation = Orientation.Vertical;
        else
            orientation = Orientation.Horizontal;
        return new Line(orientation, GetCoordinatesForOrientation(
            orientation,
            bottomLeftAreaCorner,
            topRightAreaCorner,
            roomWidthMin,
            roomLenghtMin));
    }

    public Vector2Int GetCoordinatesForOrientation(Orientation orientation, Vector2Int bottomLeftAreaCorner,
        Vector2Int topRightAreaCorner, int roomWidthMin, int roomLenghtMin)
    {
        var coordinates = Vector2Int.zero;
        if (orientation == Orientation.Horizontal)
            coordinates = new Vector2Int(
                0,
                Random.Range(
                    bottomLeftAreaCorner.y + roomLenghtMin,
                    topRightAreaCorner.y - roomLenghtMin));
        else
            coordinates = new Vector2Int(
                Random.Range(
                    bottomLeftAreaCorner.x + roomWidthMin,
                    topRightAreaCorner.x - roomWidthMin)
                , 0);
        return coordinates;
    }
}