using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeflectorHandler : MonoBehaviour
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

    GameObject arrow;
    [SerializeField]
    float speed;

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
    bool lookingRight = true;
    RotateAroundPivot RAP;
    [SerializeField]
    Vector2 mouseWorldPoint;
    BasePlayerController BPC;

    // Start is called before the first frame update
    void Start()
    {
        RAP = GetComponent<RotateAroundPivot>();
        player = GameObject.FindGameObjectWithTag("Player");
        BPC = player.GetComponent<BasePlayerController>();
        //Check if the player is looking right
        lookingRight = 
        Camera.main.ScreenToWorldPoint(Input.mousePosition).x >= gameObject.transform.position.x ? true : false;
    }

    // Update is called once per frame
    void Update(){
        
        
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

                currAnimationState = AnimationState.delivering_attack;
                timerSinceAnimStateChange = 0f;
                RAP.angle = -90f;
            }else{
                RAP.angle = -90f * timerSinceAnimStateChange/timeToSpawnAttack;
            }
            
        }else if (currAnimationState == AnimationState.delivering_attack){
            timerSinceAnimStateChange += Time.deltaTime;
            if(timerSinceAnimStateChange >= timeForFollowUp){
                currAnimationState = AnimationState.return_to_normal;
                timerSinceAnimStateChange = 0f;
                RAP.angle = 90f;
            }else{
                RAP.angle = (-90f) + (180f) * timerSinceAnimStateChange/timeForFollowUp;
            }
        }else if(currAnimationState == AnimationState.return_to_normal){
            timerSinceAnimStateChange += Time.deltaTime;
            if(timerSinceAnimStateChange >= cooldownTime){
                
                currAnimationState = AnimationState.not_handling;
                timerSinceAnimStateChange = 0f;
                RAP.angle = 0f;
            }else{
                RAP.angle = 90f - 90f* timerSinceAnimStateChange/cooldownTime;
            }
        }
        Vector2 dir = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - player.transform.position).normalized;

        if(dir.x < 0){
            RAP.angle = -RAP.angle;
        }

        if(Camera.main.ScreenToWorldPoint(Input.mousePosition).x < player.transform.position.x){
            if(lookingRight){
                lookingRight = false;
                float angleR = RAP.angle;
                RAP.resetAngle();

                player.transform.Rotate(0f,180f,0f);
                RAP.angle = angleR;
            }
        }else if(Camera.main.ScreenToWorldPoint(Input.mousePosition).x > player.transform.position.x){
            if(!lookingRight){
                lookingRight = true;
                float angleR = RAP.angle;
                RAP.resetAngle();

                player.transform.Rotate(0f, -180f, 0f);
                RAP.angle = angleR;
            }  
        }
    }
}
