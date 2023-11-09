using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SwordItem : UsableItem
{
    [SerializeField]
    GameObject sword;
    GameObject realWorldInstance;
    GameObject player;

    public SwordItem(string name) : base(name)
    {

    }

    public override void leftClickAction()
    {
        if(player == null){
            player = GameObject.FindWithTag("Player");
        }

        base.leftClickAction();
        if(realWorldInstance == null){
            realWorldInstance = GameObject.Instantiate(sword, player.transform);
        }

        

    }

}
