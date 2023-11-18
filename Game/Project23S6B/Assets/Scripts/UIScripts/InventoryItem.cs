using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItem : MonoBehaviour
{

    BaseItem item;
    GameObject sword;
    GameObject realWorldInstance;
    GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        item = new BaseItem("InventoryItemTest");
    }


    void dropItem()
    {
        if(player == null){
            player = GameObject.FindWithTag("Player");
        }

        if(realWorldInstance == null){
            realWorldInstance = GameObject.Instantiate(sword, player.transform);
        }

    }
}
