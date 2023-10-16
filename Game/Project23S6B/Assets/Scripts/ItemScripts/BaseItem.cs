using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BaseItem
{

    public string itemID;
    public string itemName;
    public BaseItem (string id, string name)
    {
        itemID = id;
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
}