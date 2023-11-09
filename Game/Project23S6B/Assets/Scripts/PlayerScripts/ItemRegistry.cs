using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;


public sealed class ItemRegistry
{
    ArrayList registry;
    private static readonly ItemRegistry instance = new ItemRegistry();
    int id = 0;

    // Explicit static constructor to tell C# compiler
    // not to mark type as beforefieldinit
    static ItemRegistry()
    {
    }

    private ItemRegistry()
    {
        registry = new ArrayList();
    }

    public static ItemRegistry Instance
    {
        get
        {
            return instance;
        }
    }

    public void addListItem(BaseItem item){ //TODO make sure that it can be class or smthg
        id += 1;
        item.itemID = id;
        if (!registry.Contains(item)){
            //NEEDS TO BE UNIQUE OBJECT NOT INTERFERED WITH    
            registry.Add(item);
        }
    }

    public BaseItem getItemByID(int itemID){
        foreach (BaseItem x in registry){
            if(x.itemID == itemID){
                return x;
            }
        }
        throw new System.Exception("Registry does not contain item of id " + itemID + "!");
    }
}
