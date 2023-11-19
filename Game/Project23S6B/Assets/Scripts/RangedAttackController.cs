using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Callbacks;
using UnityEditor.Tilemaps;
using UnityEngine;

public class RangedAttackController : EnemyController
{
    public float attackDistance;
    [SerializeField]
    bool playerPresence = false;
    [SerializeField]
    float distanceToRecogPlayer  = 20f;
    [SerializeField]
    float timeToSpawnAttack = 0.6f;
    [SerializeField]
    float timeForFollowUp = 0.1f;
    [SerializeField]
    float cooldownTime = 2f;
    bool inAttack = false;
    GameObject ArrowOrigin;

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
    GameObject arrow;
    [SerializeField]
    float speed;
    [SerializeField]
    GameObject bow;
    [SerializeField]
    GameObject pivot;
    SpriteRenderer spriteRenderer;


    bool canAttack = true;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        movementManager.mode = EnemyMovementManager.movementModes.wanderingWithRadius;

        foreach(Transform transform in gameObject.transform){
            //Assuming there's only one child
            ArrowOrigin = transform.gameObject;
        }
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        updateOrientation();
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

    public override void Update()
    {   
        base.Update();

        if(!playerPresence){
            playerPresence = checkPlayerPresence();
        }else{
            if(this.movementManager.mode != EnemyMovementManager.movementModes.stayAtRadiusWithLOS){
                this.movementManager.mode = EnemyMovementManager.movementModes.stayAtRadiusWithLOS;
            }

            if(canAttack && currAnimationState == AnimationState.not_handling && this.movementManager.hasLOSToPlayer(gameObject.transform.position)){
                currAnimationState = AnimationState.winding_up_attack;
                timerSinceAnimStateChange = 0f;
                movementLock(timeToSpawnAttack + timeForFollowUp);
                canAttack = false;
            }


            if(currAnimationState == AnimationState.winding_up_attack){
                timerSinceAnimStateChange += Time.deltaTime;

                if(timerSinceAnimStateChange >= timeToSpawnAttack){
                    currAnimationState = AnimationState.delivering_attack;
                    timerSinceAnimStateChange = 0f;
                }else{
                    Vector2 dir = (player.transform.position - pivot.transform.position).normalized;
                    float angle = (float)Math.Atan(dir.y/dir.x) * (float)(180/ Math.PI);
                    bow.GetComponent<RotateAroundPivot>().angle = angle;
                }
            }
            else if (currAnimationState == AnimationState.delivering_attack){
                timerSinceAnimStateChange += Time.deltaTime;

                if(!this.movementManager.hasLOSToPlayer(gameObject.transform.position)){
                    //Interrupt attack cos no LOS to player
                    currAnimationState = AnimationState.not_handling;
                    timerSinceAnimStateChange = 0f;
                    StartCoroutine("cooldownTimer");
                }else if(timerSinceAnimStateChange >= timeForFollowUp){
                    GameObject arrowClone = Instantiate(arrow);
                    arrowClone.transform.position = ArrowOrigin.transform.position;
                    arrowClone.GetComponent<ArrowHandler>().setOriginEnemy(gameObject);

                    Rigidbody2D rb = arrowClone.GetComponent<Rigidbody2D>();
                    Vector2 dir = (player.transform.position - ArrowOrigin.transform.position).normalized;
                    rb.velocity = dir * speed;
                    //Assuming arrow is initially pointing up
                    arrowClone.transform.Rotate(new Vector3(0f, 0f, getAngleBetweenPoints(ArrowOrigin.transform.position, player.transform.position) - 90f));
                    
                    currAnimationState = AnimationState.not_handling;
                    timerSinceAnimStateChange = 0f;
                    StartCoroutine("cooldownTimer");
                }

            }

            if(currAnimationState == AnimationState.not_handling){
                Vector2 dir = (player.transform.position - pivot.transform.position).normalized;
                float angle = (float)Math.Atan(dir.y/dir.x) * (float)(180/ Math.PI);
                bow.GetComponent<RotateAroundPivot>().angle = angle;
            }


        }
    }

    IEnumerator cooldownTimer(){
        yield return new WaitForSeconds(cooldownTime);
        canAttack = true;
    }

    public static float getAngleBetweenPoints(Vector2 pos1, Vector2 pos2){
        Vector2 dir = pos2 - pos1;
        float alpha = (float) Math.Atan(Math.Abs(dir.y)/Math.Abs(dir.x)) * (float)(180/Math.PI);
        if(dir.x > 0){
            if(dir.y > 0){
                //1st quadrant
                return alpha;
            }else{
                //4th quadrant
                return 360f - alpha;
            }
        }else{
            if(dir.y > 0){
                //2nd quadrant
                return 180f - alpha;
            }else{
                //3rd quadrant
                return 180f + alpha;
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
