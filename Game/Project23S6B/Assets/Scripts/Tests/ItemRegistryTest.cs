
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;

class ItemRegistryTest:MonoBehaviour{
    public static void Main()
    {
        BaseItem rectangle = new BaseItem("rectangle");
        rectangle.register();
        BaseItem v = ItemRegistry.Instance.getItemByID(1);
        if (v.itemName == "rectangle"){
            print("Item registry test success");
        }else{
            throw new System.Exception("Test failed for item registry");
        }

    }
}