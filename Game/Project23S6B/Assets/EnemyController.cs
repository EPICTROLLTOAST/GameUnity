using System.Collections;
using System.Collections.Generic;
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


    public EnemyMovementManager movementManager;

    // Start is called before the first frame update
    public virtual void Start()
    {
        player = GameObject.FindWithTag("Player");
        health = maxHealth;
        movementManager = this.gameObject.GetComponent<EnemyMovementManager>();
    }

    


    private void Update()
    {
        if (movementLockFlag)
        {
            this.transform.position = startPos;
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

}
