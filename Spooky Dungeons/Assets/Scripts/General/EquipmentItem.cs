using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment", menuName = "Inventory/Equipment")]
public class EquipmentItem : Item
{
    public EquipmentType equipSlot;

    public int armorModifier;
    public int damageModifier;

    public override void Use()
    {
        base.Use();
        RemoveFromInventory();
        EquipmentManager.instance.Equip(this);
    }
}

public enum EquipmentType {
    Head, 
    Chest, 
    Legs, 
    Weapon, 
    Shield, 
    Feet }
