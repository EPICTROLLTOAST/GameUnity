using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class BaseEnemyController : MonoBehaviour,IDamageable
{

    [SerializeField]
    float maxHealth = 5f;
    [SerializeField]
    float health;
    BaseEnemyAttackController attackController;
    BaseEnemyMovementScript movementScript;
    GameObject player;

    void Start()
    {
        health = maxHealth;
        attackController = GetComponent<BaseEnemyAttackController>();
        movementScript = GetComponent<BaseEnemyMovementScript>();
        player = GameObject.FindGameObjectWithTag("Player");
        movementScript.player = player;
        attackController.player = player;
    }


    void Update()
    {
        movementScript.controlMovement();
        attackController.controlAttack();
    }
    private void die()
    {
        Destroy(gameObject);
    }


    public void recceiveDamage(DamageEvent damageEvent)
    {
        health = health - damageEvent.damageAmount;
        if(health < 0)
        {
            die();
        }

    }


}
