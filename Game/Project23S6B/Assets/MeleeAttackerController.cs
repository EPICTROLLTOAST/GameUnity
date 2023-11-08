using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Build;
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
    float cooldownTime = 0.8f;
    [SerializeField]
    bool inAttack = false;

    enum attackTypes{
        swipe,
        thrust,
        clamp
    }

    [SerializeField]
    attackTypes attackType = attackTypes.swipe;

    [SerializeField]
    GameObject arm;

    [SerializeField]
    float finalRetractAngle = 45f;
    [SerializeField]
    float finalSwipeAngle = -45f;
    [SerializeField]
    float damageToDeal = 2f;
    float angleToPlayer = 0f;
    bool canDealDamage = true;

    enum AnimationState{
        winding_up_attack,
        delivering_attack,
        return_to_normal,
        not_handling
    }
    [SerializeField]
    AnimationState currAnimationState = AnimationState.not_handling;
    float timerSinceAnimStateChange = 0f;


    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        this.movementManager.mode = EnemyMovementManager.movementModes.wanderingWithRadius;
        foreach(Transform armR in gameObject.transform){
            arm = armR.gameObject;
        }
    }

    void attackPlayer(){

        if(currAnimationState == AnimationState.not_handling && canDealDamage){
            this.movementLock(timeToSpawnAttack + timeForFollowUp);

            currAnimationState = AnimationState.winding_up_attack;
            timerSinceAnimStateChange = 0f;
        }

        
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

        if(currAnimationState == AnimationState.winding_up_attack){
            timerSinceAnimStateChange += Time.deltaTime;
            if(timerSinceAnimStateChange >= timeToSpawnAttack){
                arm.GetComponent<RotateAroundPivot>().angle = finalRetractAngle;
                currAnimationState = AnimationState.delivering_attack;
                timerSinceAnimStateChange = 0f;
            }else{
                arm.GetComponent<RotateAroundPivot>().angle = finalRetractAngle/timeToSpawnAttack * timerSinceAnimStateChange;
            }
            
        }else if (currAnimationState == AnimationState.delivering_attack){
            timerSinceAnimStateChange += Time.deltaTime;
            if(timerSinceAnimStateChange >= timeForFollowUp){
                arm.GetComponent<RotateAroundPivot>().angle = finalSwipeAngle;
                currAnimationState = AnimationState.return_to_normal;
                timerSinceAnimStateChange = 0f;
                canDealDamage = true;
            }else{
                arm.GetComponent<RotateAroundPivot>().angle = finalRetractAngle - (finalRetractAngle - finalSwipeAngle)/timeForFollowUp * timerSinceAnimStateChange;
            }
        }else if(currAnimationState == AnimationState.return_to_normal){
            timerSinceAnimStateChange += Time.deltaTime;
            if(timerSinceAnimStateChange >= cooldownTime){
                arm.GetComponent<RotateAroundPivot>().angle = 0f;
                currAnimationState = AnimationState.not_handling;
                timerSinceAnimStateChange = 0f;
            }else{
                arm.GetComponent<RotateAroundPivot>().angle = finalSwipeAngle - finalSwipeAngle/cooldownTime * timerSinceAnimStateChange;
            }
        }


    }

    public void dealDamage(GameObject thingHit){
        if(currAnimationState == AnimationState.delivering_attack){
            if(thingHit.tag == "Player" && canDealDamage){
                //Deal damage
                new DamageEvent(damageToDeal, damageTypes.BLUNT, gameObject, thingHit);
                canDealDamage = false;
            }
        }

    }


}
