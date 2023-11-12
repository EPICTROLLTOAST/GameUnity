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
    [SerializeField]
    GameObject pivot;
    float heightAtStart;


    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        this.movementManager.mode = EnemyMovementManager.movementModes.wanderingWithRadius;
        distanceAtStart = arm.transform.localPosition.x;
        heightAtStart = arm.transform.localPosition.y;
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

        if(currAnimationState == AnimationState.winding_up_attack){
            canDealDamage = false;
            Vector2 dir = (player.transform.position - pivot.transform.position).normalized;
            float angle = (float)Math.Atan(dir.y/dir.x) * (float)(180/ Math.PI);

            timerSinceAnimStateChange += Time.deltaTime;
            if(timerSinceAnimStateChange >= timeToSpawnAttack){
                
                arm.transform.localPosition = new Vector2(distanceAtStart + distanceRetract, arm.transform.localPosition.y);
                arm.GetComponent<RotateAroundPivot>().angle = angle;
                

                currAnimationState = AnimationState.delivering_attack;
                timerSinceAnimStateChange = 0f;
                canDealDamage = true;
            }else{
                arm.GetComponent<RotateAroundPivot>().angle = angle/timeToSpawnAttack * timerSinceAnimStateChange;
                Vector2 dirAngle = new Vector2(1f, (float)Math.Tan(arm.GetComponent<RotateAroundPivot>().angle* (float)(Math.PI / 180f)));
                arm.transform.localPosition = dirAngle.normalized * (distanceRetract * timerSinceAnimStateChange / timeToSpawnAttack);
                arm.transform.localPosition = new Vector2(distanceAtStart + arm.transform.localPosition.x, arm.transform.localPosition.y);
            }
            
        }else if (currAnimationState == AnimationState.delivering_attack){
            timerSinceAnimStateChange += Time.deltaTime;
            if(timerSinceAnimStateChange >= timeForFollowUp){
                arm.transform.localPosition = new Vector2(distanceAtStart + distanceThrust , arm.transform.localPosition.y);
                currAnimationState = AnimationState.return_to_normal;
                timerSinceAnimStateChange = 0f;
                canDealDamage = false;
            }else{
                Vector2 dirAngle = new Vector2(1f, (float)Math.Tan(arm.GetComponent<RotateAroundPivot>().angle * (float)(Math.PI / 180f)));
                arm.transform.localPosition = dirAngle.normalized * (distanceThrust * timerSinceAnimStateChange / timeForFollowUp);
                arm.transform.localPosition = new Vector2(distanceAtStart + arm.transform.localPosition.x, arm.transform.localPosition.y);
            }
        }else if(currAnimationState == AnimationState.return_to_normal){
            timerSinceAnimStateChange += Time.deltaTime;
            if(timerSinceAnimStateChange >= cooldownTime){
                arm.transform.localPosition = new Vector2(distanceAtStart, heightAtStart);
                currAnimationState = AnimationState.not_handling;
                timerSinceAnimStateChange = 0f;
            }else{
                //Shut up fucking vscode code suggestion "Oh you don't need the brackets!" STFU it makes the code easier to read!!!
                Vector2 dirAngle = new Vector2(1f, (float)Math.Tan(arm.GetComponent<RotateAroundPivot>().angle * (float)(Math.PI / 180f)));
                arm.transform.localPosition = dirAngle.normalized * (distanceThrust - distanceThrust * timerSinceAnimStateChange / cooldownTime);
                arm.transform.localPosition = new Vector2(distanceAtStart + arm.transform.localPosition.x, arm.transform.localPosition.y);
            }
        }else{
            Vector2 dir = (player.transform.position - pivot.transform.position).normalized;
            float angle = (float)Math.Atan(dir.y/dir.x) * (float)(180/ Math.PI);
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
        
        if(direction == lookDirection.left){
            gameObject.transform.Rotate(new Vector3(0f, 0f, 180f));
        }else{
            gameObject.transform.Rotate(new Vector3(0f,0f, -180f));
        }

    }


}
