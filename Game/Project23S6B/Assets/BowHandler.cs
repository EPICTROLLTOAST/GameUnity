using System;
using System.Buffers.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowHandler : MonoBehaviour
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
    float damageToDeal = 2f;
    [SerializeField]
    float passiveAngleOffset = 90f;
    [SerializeField]
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

    void Start()
    {
        arm = gameObject;
        player = GameObject.FindGameObjectWithTag("Player");    
        RAP = arm.GetComponent<RotateAroundPivot>();
        BPC = player.GetComponent<BasePlayerController>();
    }


    // Update is called once per frame
    void Update()
    {

        mouseWorldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        if(BPC.movementLockFlag){
            currAnimationState = AnimationState.not_handling;
            timerSinceAnimStateChange = 0f;
        }

        currAngleOffset = 0f;
        if(currAnimationState == AnimationState.not_handling && Input.GetMouseButton(0) && !BPC.movementLockFlag){
            currAnimationState = AnimationState.winding_up_attack;
        }

        if(currAnimationState == AnimationState.winding_up_attack){
            timerSinceAnimStateChange += Time.deltaTime;
            if(timerSinceAnimStateChange >= timeToSpawnAttack){

                GameObject arrowChild = Instantiate(arrow);
                arrowChild.transform.position = gameObject.transform.position;
                arrowChild.GetComponent<Rigidbody2D>().velocity = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - RAP.anchor.transform.position).normalized * speed;
                print(RangedAttackController.getAngleBetweenPoints(Camera.main.ScreenToWorldPoint(Input.mousePosition), RAP.anchor.transform.position));
                arrowChild.transform.Rotate(new Vector3(0f, 0f, RangedAttackController.getAngleBetweenPoints(RAP.anchor.transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition)) - 90f));
                


                currAnimationState = AnimationState.delivering_attack;
                timerSinceAnimStateChange = 0f;
            }else{
                
            }
            
        }else if (currAnimationState == AnimationState.delivering_attack){
            timerSinceAnimStateChange += Time.deltaTime;
            if(timerSinceAnimStateChange >= timeForFollowUp){
                currAnimationState = AnimationState.return_to_normal;
                timerSinceAnimStateChange = 0f;
            }else{
                
            }
        }else if(currAnimationState == AnimationState.return_to_normal){
            timerSinceAnimStateChange += Time.deltaTime;
            if(timerSinceAnimStateChange >= cooldownTime){
                
                currAnimationState = AnimationState.not_handling;
                timerSinceAnimStateChange = 0f;
            }else{
                
            }
        }
        Vector2 dir = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - player.transform.position).normalized;
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
