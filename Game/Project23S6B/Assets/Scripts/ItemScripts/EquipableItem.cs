using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipableItem: BaseItem //stuffs like armour and accessories that only have an equip/unequip action
{   
    //constructor
    public EquipableItem (string id, string name) : base(id, name)
    {

    }


    public string equipableType;
    public bool isEquipped;
    public void equipItem()
    {
        if (isEquipped == false)
        {
            
        }
    }

    public void unequipItem()
    {
        if (isEquipped == true)
        {

        }

    }

}