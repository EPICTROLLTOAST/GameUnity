using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordHandler : MonoBehaviour
{

    [SerializeField]
    float timeToSpawnAttack = 0.2f;
    [SerializeField]
    float timeForFollowUp = 0.2f;
    [SerializeField]
    float cooldownTime = 0.8f;
    [SerializeField]
    bool inAttack = false;

    GameObject arm;

    [SerializeField]
    float finalRetractAngle = 20f;
    [SerializeField]
    float finalSwipeAngle = -65f;
    [SerializeField]
    float damageToDeal = 2f;
    [SerializeField]
    float passiveAngleOffset = 90f;
    float currAngleOffset;
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
    GameObject player;

    BasePlayerController BPC;

    void Start()
    {
        arm = gameObject;
        player = GameObject.FindGameObjectWithTag("Player");  
        BPC = player.GetComponent<BasePlayerController>();
    }


    // Update is called once per frame
    void Update()
    {
        currAngleOffset = 0f;
        if(BPC.movementLockFlag){
            currAnimationState = AnimationState.not_handling;
            timerSinceAnimStateChange = 0f;
        }

        if(currAnimationState == AnimationState.not_handling && Input.GetMouseButton(0) && !BPC.movementLockFlag){
            currAnimationState = AnimationState.winding_up_attack;
        }

        if(currAnimationState == AnimationState.winding_up_attack){
            timerSinceAnimStateChange += Time.deltaTime;
            if(timerSinceAnimStateChange >= timeToSpawnAttack){
                currAngleOffset = finalRetractAngle;
                currAnimationState = AnimationState.delivering_attack;
                timerSinceAnimStateChange = 0f;
            }else{
                currAngleOffset = finalRetractAngle/timeToSpawnAttack * timerSinceAnimStateChange;
            }
            
        }else if (currAnimationState == AnimationState.delivering_attack){
            timerSinceAnimStateChange += Time.deltaTime;
            if(timerSinceAnimStateChange >= timeForFollowUp){
                currAngleOffset = finalSwipeAngle;
                currAnimationState = AnimationState.return_to_normal;
                timerSinceAnimStateChange = 0f;
                canDealDamage = true;
            }else{
                currAngleOffset = finalRetractAngle - (finalRetractAngle - finalSwipeAngle)/timeForFollowUp * timerSinceAnimStateChange;
            }
        }else if(currAnimationState == AnimationState.return_to_normal){
            timerSinceAnimStateChange += Time.deltaTime;
            if(timerSinceAnimStateChange >= cooldownTime){
                currAngleOffset = 0f;
                currAnimationState = AnimationState.not_handling;
                timerSinceAnimStateChange = 0f;
            }else{
                currAngleOffset = finalSwipeAngle - finalSwipeAngle/cooldownTime * timerSinceAnimStateChange;
            }
        }
        Vector2 manToMouse = Camera.main.ScreenToWorldPoint(Input.mousePosition) - player.transform.position;
        float angleToMouse = (float) (Math.Atan(manToMouse.y/manToMouse.x) * 180/Math.PI);


        currAngleOffset += passiveAngleOffset + angleToMouse;
        arm.GetComponent<RotateAroundPivot>().angle = currAngleOffset;
    }
}
