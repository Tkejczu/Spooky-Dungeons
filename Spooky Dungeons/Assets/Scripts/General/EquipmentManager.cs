using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    #region Equipment Singleton
    public static EquipmentManager instance;

    
    private void Awake()
    {
        instance = this;
    }
    #endregion
    Transform equipmentSlotsParent;
    EquipmentItem[] currentEquipment;

    public delegate void OnEquipmentChange(EquipmentItem newItem, EquipmentItem oldItem);
    public OnEquipmentChange onEquipmentChange;

    Inventory inventory;

    private void Start()
    {
        inventory = Inventory.instance;

        int numberOfSlots = System.Enum.GetNames(typeof(EquipmentType)).Length;
        currentEquipment = new EquipmentItem[numberOfSlots];
    }

    public void Equip(EquipmentItem newItem)
    {
        int slotIndex = (int)newItem.equipSlot;
        EquipmentItem oldItem = null;

        if (currentEquipment[slotIndex] != null)
        {
            oldItem = currentEquipment[slotIndex];
            inventory.Add(oldItem);
        }

        if (onEquipmentChange != null)
        {
            onEquipmentChange.Invoke(newItem, oldItem);
        }

        currentEquipment[slotIndex] = newItem;
    }
    public void Unequip(int slotIndex)
    {
        if (currentEquipment[slotIndex] != null)
        {
            EquipmentItem oldItem = currentEquipment[slotIndex];
            if (inventory.Add(oldItem))
            {
                currentEquipment[slotIndex] = null;
            }

            if (onEquipmentChange != null)
            {
                onEquipmentChange.Invoke(null, oldItem);
            }
        }
    }
}
