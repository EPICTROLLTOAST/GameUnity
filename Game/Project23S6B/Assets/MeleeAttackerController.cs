using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttackerController : EnemyController
{
    public float attackDistance;
    [SerializeField]
    bool playerPresence = false;
    [SerializeField]
    float distanceToRecogPlayer  = 20f;
    [SerializeField]
    float timeToSpawnAttack = 0.2f;
    [SerializeField]
    float timeForFollowUp = 0.2f;
    [SerializeField]
    bool inAttack = false;


    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        this.movementManager.mode = EnemyMovementManager.movementModes.wanderingWithRadius;
    }

    void attackPlayer(){
        //TODO like spawn the swiping effect and do the thing
        this.movementLock(timeToSpawnAttack + timeForFollowUp);


    }

    bool checkPlayerPresence(){
        float distanceToPlayer = Vector2.Distance(this.gameObject.transform.position, player.transform.position);
        if(distanceToPlayer < distanceToRecogPlayer){
            return true;
        }
        return false;
    }

    // Update is called once per frame
    void Update()
    {
        
        if(!playerPresence){
            playerPresence = checkPlayerPresence();
        }else{
            if(this.movementManager.mode != EnemyMovementManager.movementModes.charge){
                this.movementManager.mode = EnemyMovementManager.movementModes.charge;
            }
        }

        float distanceToPlayer = Vector2.Distance(this.gameObject.transform.position, player.transform.position);
        if(playerPresence && (distanceToPlayer < attackDistance)){
            attackPlayer();
        }    
    }

    
    
}
