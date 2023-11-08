using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOnImpact : MonoBehaviour
{
    MeleeAttackerController mlc;

    void Start()
    {
        mlc = gameObject.transform.parent.GetComponent<MeleeAttackerController>();
    }


    void OnTriggerStay2D(Collider2D other)
    {
        mlc.dealDamage(other.gameObject);
    }


}
