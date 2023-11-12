using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOnImpact : MonoBehaviour
{
    [SerializeField]
    MeleeAttackerController mlc;

    void Start()
    {
        mlc = gameObject.transform.parent.gameObject.GetComponent<MeleeAttackerController>();
    }


    void OnTriggerStay2D(Collider2D other)
    {
        try{
            mlc.dealDamage(other.gameObject);
        }catch{
            
        }
        
    }


}
