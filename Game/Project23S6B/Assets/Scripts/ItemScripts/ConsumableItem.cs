using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsumableItem: BaseItem
{
    //constructor
    public ConsumableItem (string id, string name) : base(id, name)
    {
        

    }
    public int itemCount;
    public void consumeItem()
    {
        //code to add some modifiers to player
    }
}
