using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttackerController : EnemyController
{
    public float attackDistance;
    


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float distanceToPlayer = Vector2.Distance(this.gameObject.transform.position, player.transform.position);
        if(distanceToPlayer < attackDistance){

        }    
    }


    
}
