using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedWeapon: UsableItem //weapons that can attack 
{
    public GameObject projectile;
    [SerializeField]
    //constructor
    public RangedWeapon (string name) : base(name)
    {
        

    }
    
    public override void leftClickAction()
    {
        //Instantiate(projectile);
        Debug.Log("FIRE ARROW");
    }


}