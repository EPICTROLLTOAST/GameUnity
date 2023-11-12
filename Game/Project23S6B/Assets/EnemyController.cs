using System.Collections;
using System.Collections.Generic;
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

    // Start is called before the first frame update
    public virtual void Start()
    {
        player = GameObject.FindWithTag("Player");
        health = maxHealth;
        movementManager = this.gameObject.GetComponent<EnemyMovementManager>();
    }

    


    public virtual void Update()
    {

        if (movementLockFlag)
        {
            this.transform.position = startPos;
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
