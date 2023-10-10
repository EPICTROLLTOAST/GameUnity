using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory
{
    public ArrayList inventory = new ArrayList();

    public PlayerInventory(){
        ArrayList inventory = new ArrayList();
    }

    public void addToInventory(BaseItem item) //adds item to player inventory
    {
        inventory.Add(item);
    }

    public void removeFromInventory(BaseItem item, bool destroyItem=false) 
    {
        //Removes an item from player inventory
        //When destroyItem is set to false (by default), item will fall on ground and can be picked up again
        //When destroyItem is set to true, item will be permanently deleted (eg. consumables/throwables)

        if (destroyItem == false)
        {
            //code to create an item object on the ground
        }

        //code to remove item from inventory
        inventory.Remove(item);
    }

    public void displayInventory()
    {
        //Displays inventory of player to console
        //foreach (BaseItem item in inventory)
        //{
        //    string output = item.itemID + item.itemName;
        //    Debug.Log(output);
        //}

        BaseItem outputItem = (BaseItem) inventory[0];
        Debug.Log(outputItem.itemName);
    }

}
