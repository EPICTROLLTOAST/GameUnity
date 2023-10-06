using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttackController : BaseEnemyAttackController
{

    [SerializeField]
    float range = 1f;
    [SerializeField]
    float damage = 1f;
    [SerializeField]
    damageTypes damageType;
    [SerializeField]
    float attackCooldown = 1f;
    [SerializeField]
    float attackDuration = 1f;
    [SerializeField]
    //Time before attack reaches destination
    float attackLand = 0.6f;
    [SerializeField]
    GameObject attackTarget;
    [SerializeField]
    bool inAttackFlag = false;


    // Start is called before the first frame update
    void Start()
    {
        
    }


    public override void controlAttack()
    {
        base.controlAttack();
        if (Vector2.Distance(attackTarget.transform.position, gameObject.transform.position) <= range && !inAttackFlag)
        {
            inAttackFlag = true;
            movementLock(attackDuration);
            Invoke("meleeAttack", attackLand);
            Invoke("stopAttack", attackDuration);
        }

        
    }

    public void meleeAttack()
    {
        //Attack the player, have no clue how to implement this, a simple range check?? Or maybe a physical swipe that you have to move away from
    }

    public void stopAttack()
    {
        inAttackFlag = false;
    }

}
