using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsableItem: BaseItem //weapons that can attack 
{
    public float cooldownSeconds;

    //constructor
    public UsableItem (string name) : base(name)
    {
        

    }

    public override void leftClickAction()
    {
        base.leftClickAction();
        Debug.Log("USABLE ITEM");
    }

}
