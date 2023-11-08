using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAttackController : EnemyController
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
    float cooldownTime = 0.8f;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        movementManager.mode = EnemyMovementManager.movementModes.wanderingWithRadius;
    }

    void attackPlayer(){

    }

    bool checkPlayerPresence(){
        float distanceToPlayer = Vector2.Distance(this.gameObject.transform.position, player.transform.position);
        if(distanceToPlayer < distanceToRecogPlayer){
            return true;
        }
        return false;
    }

    void Update()
    {
        
        if(!playerPresence){
            playerPresence = checkPlayerPresence();
        }else{
            if(this.movementManager.mode != EnemyMovementManager.movementModes.stayAtRadiusWithLOS){
                this.movementManager.mode = EnemyMovementManager.movementModes.stayAtRadiusWithLOS;
            }
        }

        
    }
}
