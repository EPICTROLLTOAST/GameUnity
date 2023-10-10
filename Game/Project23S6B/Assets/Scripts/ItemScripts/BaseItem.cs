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
}