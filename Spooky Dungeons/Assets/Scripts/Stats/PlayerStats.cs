using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : CharacterStats
{
    void Start()
    { 
        EquipmentManager.instance.onEquipmentChange += OnEquipmentChanged;
        InvokeRepeating("RegenerateHealth", 2f, 2f);
    }

    void OnEquipmentChanged(EquipmentItem newItem, EquipmentItem oldItem)
    {
        if (newItem != null)
        {
            armor.AddModifier(newItem.armorModifier);
            damage.AddModifier(newItem.damageModifier);
        }
        if (oldItem != null)
        {
            armor.RemoveModifier(oldItem.armorModifier);
            damage.RemoveModifier(oldItem.damageModifier);
        }
    }



    public override void Die()
    {
        base.Die();
        PlayerManager.instance.KillPlayer();
    }   
}
