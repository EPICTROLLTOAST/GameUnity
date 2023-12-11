using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyController : MonoBehaviour, IDamageable
{
    public GameObject player;
    Vector2 startPos;
    [SerializeField]
    bool movementLockFlag = false;
    [SerializeField]
    float maxHealth = 5f;
    [SerializeField]
    float health;
    public enum lookDirection{
        left,
        right
    }
    public lookDirection orientation;


    public EnemyMovementManager movementManager;

    [SerializeField]
    float knockbackResistance = 1f;
    float timeSinceMovementLock = 0f;
    float timeToWaitForMovementLock = 0f;

    [SerializeField]
    float maxKnockbackSpeed = 20f;
    [SerializeField]
    float minKnockbackSpeed = 2f;
    [SerializeField]
    float maxMovementLocktime = 3f;
    [SerializeField]
    float minMovementLockTime = 1f;
    [SerializeField]
    float flungMultiplier = 2f;
    AIPath path;
    AIDestinationSetter destinationSetter;


    // Start is called before the first frame update
    public virtual void Start()
    {
        player = GameObject.FindWithTag("Player");
        health = maxHealth;
        movementManager = this.gameObject.GetComponent<EnemyMovementManager>();
        path = this.gameObject.GetComponent<AIPath>();
        destinationSetter = this.gameObject.GetComponent<AIDestinationSetter>();
        
    }

    


    public virtual void Update()
    {

        if (movementLockFlag)
        {
            timeSinceMovementLock += Time.deltaTime;
            if(timeSinceMovementLock >= timeToWaitForMovementLock){
                movementLockFlag = false;
                timeSinceMovementLock = 0f;
                timeToWaitForMovementLock = 0f;
                path.enabled = true;
                destinationSetter.enabled = true;
            }
        }else{
            updateOrientation();
        }
    }



    public void movementLock(float time)
    {
        movementLockFlag = true;
        startPos = this.transform.position;
        Invoke("disableMovementLock", time);
    }

    private void disableMovementLock()
    {
        movementLockFlag = false;
    }

    public virtual void recceiveDamage(DamageEvent damageEvent)
    {
        health -= damageEvent.damageAmount;
        if(health <= 0f){
            die();
        }
        if(damageEvent.knockbackValue > knockbackResistance){
            //Full on being flung back
            //It's actually force not speed btw


            movementLockFlag = true;
            path.enabled = false;
            destinationSetter.enabled = false;
            
            timeSinceMovementLock = 0f;
            float knockbackSpeed;

            if(damageEvent.knockbackValue >= 10f){
                timeToWaitForMovementLock = ((damageEvent.knockbackValue - knockbackResistance) * (maxMovementLocktime - minMovementLockTime) / (10f) + minMovementLockTime) * flungMultiplier;
                if (timeToWaitForMovementLock > maxMovementLocktime * flungMultiplier){
                    timeToWaitForMovementLock = maxMovementLocktime * flungMultiplier;
                }

                knockbackSpeed = ((damageEvent.knockbackValue - knockbackResistance) * (maxKnockbackSpeed - minKnockbackSpeed) / (10f) + minKnockbackSpeed) * flungMultiplier;
                if (knockbackSpeed > maxKnockbackSpeed * flungMultiplier){
                    knockbackSpeed = maxKnockbackSpeed * flungMultiplier;
                }

            }else{
                //From knockbackResistance - 10
                
                timeToWaitForMovementLock = (damageEvent.knockbackValue - knockbackResistance) * (maxMovementLocktime - minMovementLockTime) / (10f - knockbackResistance) + minMovementLockTime;
                knockbackSpeed = ((damageEvent.knockbackValue - knockbackResistance) * (maxKnockbackSpeed - minKnockbackSpeed) / (10f) + minKnockbackSpeed) * flungMultiplier;

            }
            damageEvent.target.GetComponent<Rigidbody2D>().velocity = new Vector2(0f,0f);
            damageEvent.target.GetComponent<Rigidbody2D>().AddForce(damageEvent.getKnockbackDirection().normalized * knockbackSpeed * 10);
        }
    }

    public virtual void die(){
        Destroy(gameObject);
    }

    public virtual void updateOrientation(){
        float xDiff = player.transform.position.x - gameObject.transform.position.x;

        if(xDiff >= 0){
            if(orientation == lookDirection.left){
                orientation = lookDirection.right;
                orientationChange(orientation);
            }
            
        }else{
            if(orientation == lookDirection.right){
                orientation = lookDirection.left;
                orientationChange(orientation);
            }
        }
    }


    public virtual void orientationChange(lookDirection direction){

    }
}
