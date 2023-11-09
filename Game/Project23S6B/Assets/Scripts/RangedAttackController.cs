using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Callbacks;
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

                    currAnimationState = AnimationState.not_handling;
                    timerSinceAnimStateChange = 0f;
                    StartCoroutine("cooldownTimer");
                }

            }

            if(currAnimationState != AnimationState.not_handling){
                //TODO point the bow in the player's direction
            }


        }
    }


    IEnumerator cooldownTimer(){
        yield return new WaitForSeconds(cooldownTime);
        canAttack = true;
    }
}
