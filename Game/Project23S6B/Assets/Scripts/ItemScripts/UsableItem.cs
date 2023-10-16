using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsableItem: BaseItem //weapons that can attack 
{
    //constructor
    public UsableItem (string id, string name) : base(id, name)
    {
        

    }
    public float cooldownSeconds;

    public void useItem()
    {
        
    }
}
