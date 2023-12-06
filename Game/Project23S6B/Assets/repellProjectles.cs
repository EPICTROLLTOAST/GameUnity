using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class repellProjectles : MonoBehaviour
{
    [SerializeField]
    float repellVelocity = 5f;
    GameObject player;

    enum reflectTypes{
        awayFromPlayerCenter,
        oppositeDirectionToThatOfEntry
    }
    [SerializeField]
    reflectTypes reflectType = reflectTypes.awayFromPlayerCenter;

    void Start(){
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void OnTriggerEnter2D(Collider2D otherCollider){
        print(otherCollider.name);
        if(otherCollider.gameObject.GetComponent<ArrowHandler>() != null){
            otherCollider.gameObject.GetComponent<ArrowHandler>().enemyTag = "Enemy";
            otherCollider.gameObject.GetComponent<Rigidbody2D>().velocity = 
            repellVelocity * ((reflectType == reflectTypes.awayFromPlayerCenter) ? 
            (Vector2)(otherCollider.gameObject.transform.position - player.transform.position).normalized:
            (Vector2)(otherCollider.gameObject.GetComponent<Rigidbody2D>().velocity.normalized * -1f));
            otherCollider.gameObject.transform.eulerAngles = new Vector3(0f, 0f, RangedAttackController.
            getAngleBetweenPoints(otherCollider.gameObject.GetComponent<Rigidbody2D>().velocity, Vector3.zero) + 90f);
        }
    }

}
