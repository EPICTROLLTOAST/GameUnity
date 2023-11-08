using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMeleeAttack : MonoBehaviour
{
    [Range(0f,Mathf.Infinity)]
    public float fireDistance;
    [Range(0f,Mathf.Infinity)]
    public float timeBetweenFiring;
    public float timeSinceLastFire;
    public float damage;
    GameObject player;
    GameObject pretendPlayer;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        timeSinceLastFire = timeBetweenFiring;
        player = gameObject.GetComponent<testMovement>().player;
        pretendPlayer = gameObject.GetComponent<testMovement>().playerPretend;

    }

    // Update is called once per frame
    void Update()
    {
        timeSinceLastFire += Time.deltaTime;
        if (Vector3.Distance(gameObject.transform.position, player.transform.position) <= fireDistance && timeSinceLastFire >= timeBetweenFiring)
        {
            //shoot towards player with some calculations to account for accuracy
            pretendPlayer.transform.position = new Vector3(-100,-100,-100);
            RaycastHit hit;
            if(Physics.Raycast(gameObject.transform.position,player.transform.position-gameObject.transform.position,out hit, Mathf.Infinity))
            {
                if (hit.collider.gameObject == player)
                {
                    //fire 
                    timeSinceLastFire = 0f;
                    //Play some animation of hitting the player
                    player.GetComponent<health>().ChangeHP(-damage);
                }
            }
        }

    }
}
