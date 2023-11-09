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

    void Start()
    {
        arm = gameObject;
        player = GameObject.FindGameObjectWithTag("Player");    
    }


    // Update is called once per frame
    void Update()
    {
        currAngleOffset = 0f;
        if(currAnimationState == AnimationState.not_handling && Input.GetMouseButton(0)){
            currAnimationState = AnimationState.winding_up_attack;
        }

        if(currAnimationState == AnimationState.winding_up_attack){
            timerSinceAnimStateChange += Time.deltaTime;
            if(timerSinceAnimStateChange >= timeToSpawnAttack){

                GameObject arrowChild = Instantiate(arrow);
                arrowChild.transform.position = gameObject.transform.position;
                arrowChild.GetComponent<Rigidbody2D>().velocity = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - gameObject.transform.position).normalized * speed;

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
        Vector2 manToMouse = Camera.main.ScreenToWorldPoint(Input.mousePosition) - player.transform.position;
        float angleToMouse = (float) (Math.Atan(manToMouse.y/manToMouse.x) * 180/Math.PI);


        currAngleOffset += angleToMouse;
        arm.GetComponent<RotateAroundPivot>().angle = currAngleOffset;
    }
}
