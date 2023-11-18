using System.Reflection.Emit;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Build;
using UnityEngine;
using System;
using Unity.Mathematics;

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
    [Header("Swipe")]
    [SerializeField]
    float finalRetractAngle = 45f;
    [SerializeField]
    float finalSwipeAngle = -45f;

    [Header("Thrust")]
    [SerializeField]
    float distanceRetract = -2f;
    [SerializeField]
    float distanceThrust = 5f;
    [SerializeField]
    float distanceAtStart = 0f;
    [SerializeField]
    float heightAtStart = 0f;

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
    [SerializeField]
    float timerSinceAnimStateChange = 0f;
    [SerializeField]
    GameObject pivot;
    [SerializeField]
    RotateAroundPivot rotateAroundPivot;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        this.movementManager.mode = EnemyMovementManager.movementModes.wanderingWithRadius;
        distanceAtStart = arm.transform.localPosition.x;
        heightAtStart = arm.transform.localPosition.y;
        RotateAroundPivot rotateAroundPivot = arm.GetComponent<RotateAroundPivot>();
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
    public override void Update()
    {
        base.Update();


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
        
        if(attackType == attackTypes.swipe){
            swipeAnimationHandler();
        }else if(attackType == attackTypes.thrust){
            thrustAnimationHandler();
        }
    }

    void swipeAnimationHandler(){
        if(currAnimationState == AnimationState.winding_up_attack){
            timerSinceAnimStateChange += Time.deltaTime;
            if(timerSinceAnimStateChange >= timeToSpawnAttack){
                arm.GetComponent<RotateAroundPivot>().angle = finalRetractAngle * (orientation == lookDirection.right?1:-1);
                currAnimationState = AnimationState.delivering_attack;
                timerSinceAnimStateChange = 0f;
            }else{
                arm.GetComponent<RotateAroundPivot>().angle = finalRetractAngle/timeToSpawnAttack * timerSinceAnimStateChange * (orientation == lookDirection.right?1:-1);
            }
            
        }else if (currAnimationState == AnimationState.delivering_attack){
            timerSinceAnimStateChange += Time.deltaTime;
            if(timerSinceAnimStateChange >= timeForFollowUp){
                arm.GetComponent<RotateAroundPivot>().angle = finalSwipeAngle * (orientation == lookDirection.right?1:-1);
                currAnimationState = AnimationState.return_to_normal;
                timerSinceAnimStateChange = 0f;
                canDealDamage = true;
            }else{
                arm.GetComponent<RotateAroundPivot>().angle = (finalRetractAngle - (finalRetractAngle - finalSwipeAngle)/timeForFollowUp * timerSinceAnimStateChange) * (orientation == lookDirection.right?1:-1);
            }
        }else if(currAnimationState == AnimationState.return_to_normal){
            timerSinceAnimStateChange += Time.deltaTime;
            if(timerSinceAnimStateChange >= cooldownTime){
                arm.GetComponent<RotateAroundPivot>().angle = 0f;
                currAnimationState = AnimationState.not_handling;
                timerSinceAnimStateChange = 0f;
            }else{
                arm.GetComponent<RotateAroundPivot>().angle = (finalSwipeAngle - finalSwipeAngle/cooldownTime * timerSinceAnimStateChange) * (orientation == lookDirection.right?1:-1);
            }
        }
    }
    //! Hello future self, I coded all of this at 1 am so I wont be adding any comments 😛
    //! Also all this code is stupid and dumb
    //! I am very stupid
    void thrustAnimationHandler(){
        Vector2 dir = (player.transform.position - gameObject.transform.position).normalized;
        float directAngle = (float)Math.Atan(dir.y/Math.Abs(dir.x)) * (float)(180/ Math.PI);
        if(currAnimationState == AnimationState.winding_up_attack){
            timerSinceAnimStateChange += Time.deltaTime;
            canDealDamage = false;
            if(timerSinceAnimStateChange >= timeToSpawnAttack){

                float angleBefore = rotateAroundPivot.angle;
                rotateAroundPivot.resetAngle();
                float dist = 1f;
                
                dist = dist * distanceRetract;
                arm.transform.localPosition = new Vector2(distanceAtStart + dist, heightAtStart);
                
                rotateAroundPivot.angle = angleBefore;

                currAnimationState = AnimationState.delivering_attack;
                timerSinceAnimStateChange = 0f;
                canDealDamage = true;
            }else{
                
                float angleBefore = rotateAroundPivot.angle;
                rotateAroundPivot.resetAngle();
                float dist = 1f;

                dist = dist * (distanceRetract * timerSinceAnimStateChange / timeToSpawnAttack);
                arm.transform.localPosition = new Vector2(distanceAtStart + dist, heightAtStart);

                rotateAroundPivot.angle = angleBefore;
            }
            
        }else if (currAnimationState == AnimationState.delivering_attack){
            timerSinceAnimStateChange += Time.deltaTime;
            if(timerSinceAnimStateChange >= timeForFollowUp){
                float angleBefore = rotateAroundPivot.angle;
                rotateAroundPivot.resetAngle();
                float dist = 1f;
                
                dist = dist * distanceThrust;
                arm.transform.localPosition = new Vector2(distanceAtStart + dist, heightAtStart);

                currAnimationState = AnimationState.return_to_normal;
                timerSinceAnimStateChange = 0f;
                canDealDamage = false;
                rotateAroundPivot.angle = angleBefore;
            }else{
                float angleBefore = rotateAroundPivot.angle;
                rotateAroundPivot.resetAngle();
                float dist = 1f;
                
                dist = dist * (distanceThrust * timerSinceAnimStateChange / timeForFollowUp);
                arm.transform.localPosition = new Vector2(distanceAtStart + dist, heightAtStart);
                rotateAroundPivot.angle = angleBefore;
            }
        }else if(currAnimationState == AnimationState.return_to_normal){
            timerSinceAnimStateChange += Time.deltaTime;
            if(timerSinceAnimStateChange >= cooldownTime){
                float angleBefore = rotateAroundPivot.angle;
                rotateAroundPivot.resetAngle();
                arm.transform.localPosition = new Vector2(distanceAtStart, heightAtStart);
                currAnimationState = AnimationState.not_handling;
                timerSinceAnimStateChange = 0f;
                rotateAroundPivot.angle = angleBefore;
            }else{
                float angleBefore = rotateAroundPivot.angle;
                rotateAroundPivot.resetAngle();
                float dist = 1f;
                
                dist = dist * (distanceThrust - distanceThrust * timerSinceAnimStateChange / cooldownTime);;
                arm.transform.localPosition = new Vector2(distanceAtStart + dist, heightAtStart);
                rotateAroundPivot.angle = angleBefore;
            }
        }else{
            float angle = (float)Math.Atan(dir.y/Math.Abs(dir.x)) * (float)(180/ Math.PI);
            if(dir.y >= 0){
                angle = (float)Math.Atan(dir.y/Math.Abs(dir.x)) * (float)(180f/ Math.PI);
            }else{
                angle = ((float)Math.Atan(dir.y/Math.Abs(dir.x)) * (float)(180f/ Math.PI));
            }

            if(dir.x < 0){
                angle = -angle;
            }

            arm.GetComponent<RotateAroundPivot>().angle = angle;
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

    public override void orientationChange(lookDirection direction)
    {
        arm.GetComponent<RotateAroundPivot>().resetAngle();
        if(direction == lookDirection.left){
            gameObject.transform.Rotate(new Vector3(0f, 180f, 0f));
        }else{
            gameObject.transform.Rotate(new Vector3(0f, -180f, 0f));
        }

    }


}
