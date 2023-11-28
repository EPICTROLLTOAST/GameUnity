using System.Net.NetworkInformation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BarrelManager : MonoBehaviour
{

    [SerializeField]
    float maxDistanceFromBarrel = 10f;
    [SerializeField]
    float forceToUse = 10f;
    [SerializeField]
    float damageToDeal = 10f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        
    }

    public void explode(){
        print("123");
        Collider2D[] colls = Physics2D.OverlapCircleAll(gameObject.transform.position, maxDistanceFromBarrel);
        foreach(Collider2D coll in colls){
            Rigidbody2D rb = coll.gameObject.GetComponent<Rigidbody2D>();
            print(coll.gameObject.name);
            if(rb != null){
                Vector2 dir = gameObject.transform.position - coll.gameObject.transform.position;
                float distance = dir.magnitude;
                dir = dir.normalized;
                dir = distance != 0? dir * forceToUse/(distance * distance) : new Vector2(0,0);
                new DamageEvent(damageToDeal, damageTypes.FIRE, rb.gameObject, gameObject, dir.magnitude);
            }
        }
        Destroy(gameObject);
    }
    
}
