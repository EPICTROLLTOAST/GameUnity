using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BaseItem
{

    public int itemID;
    public string itemName;
    public BaseItem (string name)
    {
        itemID = -999;
        itemName = name;
    }

    public virtual void leftClickAction()
    {
        Debug.Log("LEFT CLICK" + ' '  + itemID + ' ' + itemName);
    }

    public virtual void rightClickAction()
    {
        Debug.Log("RIGHT CLICK" + ' '  + itemID + ' ' + itemName);
    }

    public void register(){
        ItemRegistry.Instance.addListItem(this);
    }
}