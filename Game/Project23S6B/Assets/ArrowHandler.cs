using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowHandler : MonoBehaviour
{

    public float damage = 3f;
    [SerializeField]
    GameObject projectile;
    
    GameObject originEnemy;
    [SerializeField]
    String enemyTag = "Player";

    public void setOriginEnemy(GameObject gameObjectR){
        originEnemy = gameObjectR;
    }

    public void spawnProjectile(){
        Instantiate(projectile);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == enemyTag)
        {
            //? maybe discuss about dealing damage also based on its momentum?
            new DamageEvent(damage, damageTypes.PIERCING, originEnemy, other.gameObject, 5f, gameObject.GetComponent<Rigidbody2D>().velocity);
            Destroy(gameObject);
        }else if(other.tag == "Structure"){
            breakArrow();
        }else if (other.GetComponent<BarrelManager>() != null){
            
            other.GetComponent<BarrelManager>().explode();
            Destroy(gameObject);
        }
    }

    void breakArrow(){
        //? Maybe if you want to play an animation
        Destroy(gameObject);
    }
}
