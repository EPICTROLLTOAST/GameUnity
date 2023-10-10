using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PlayerHealth : MonoBehaviour, IDamageable
{

    [SerializeField]
    float maxHealth = 10f;
    [SerializeField]
    float health;


    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void recceiveDamage(DamageEvent damageEvent)
    {
        health -= damageEvent.damageAmount;
        if (health <= 0 )
        {
            die();
        }
    }

    private void die()
    {

    }

}
