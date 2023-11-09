using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedWeapon: UsableItem//weapons that can attack 
{
    ArrowHandler arrowHandller;

    //constructor
    public RangedWeapon (string name) : base(name)
    {
        

    }
    
    public override void leftClickAction()
    {
        arrowHandller.spawnProjectile();
        Debug.Log("FIRE ARROW");
    }


}