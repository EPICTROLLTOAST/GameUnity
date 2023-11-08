using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ManageMelee : MonoBehaviour
{
    public enum state
    {
        hide,
        charge,
        retreat,
        wander
    }

    public bool hidingCalled = false;
    public bool knowEnemyLocation = false;
    public ArrayList enemies = new ArrayList();
    public state currentState = state.wander;
    public int charging = 0;
    public float averageDistance = Mathf.Infinity;
    public float averageDistanceBeforeCharge;
    public int enemyCount;

    [Range(0f,1f)]
    public float percentageChargingBeforeFullCharge;

    // Start is called before the first frame update
    void Start()
    {
        currentState = state.wander;
        //Adds all the children to a list 
        Transform[] ts = gameObject.GetComponentsInChildren<Transform>();
        foreach (Transform enemy in ts)
        {
            enemies.Add(enemy.gameObject);
        }
        enemies.RemoveAt(0);
        if(averageDistanceBeforeCharge == 0)
        {
            averageDistanceBeforeCharge = 10f;
        }
        if(percentageChargingBeforeFullCharge == 0)
        {
            percentageChargingBeforeFullCharge = 0.6f;
        }
        StartCoroutine(makeDecisions());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            currentState = state.hide;
        }


        //make sure that the children are hiding
        //Just call once 

        if (currentState == state.hide)
        {
            if (!hidingCalled)
            {
                hidingCalled = true;
                foreach(GameObject enemy in enemies)
                {
                    testMovement ts = enemy.GetComponent<testMovement>();
                    Vector3 hiding = ts.GetHidingPosition(6f, 24f, 10);
                    if (hiding != Vector3.zero)
                    {
                        enemy.GetComponent<NavMeshAgent>().SetDestination(hiding);
                        ts.hidingSpot = hiding;
                    }
                }
            }
            foreach (GameObject enemy in enemies)
            {
                testMovement ts = enemy.GetComponent<testMovement>();
                if (ts.hidingSpot != Vector3.zero)
                {
                    if(Vector3.Distance(enemy.transform.position, ts.hidingSpot) < 1f)
                    {
                        ts.state = testMovement.movementState.holdingPos;
                    }
                }

            }

        }else if(currentState == state.charge)
        {
            //make sure all the children are charging towards the enemy
            foreach (GameObject enemy in enemies)
            {
                if (!(enemy.GetComponent<testMovement>().state == testMovement.movementState.stayingAtDist))
                { 
                    enemy.GetComponent<testMovement>().StayAtRadius(1f);
                }
                
            }
        }else if(currentState == state.retreat)
        {
            //make sure all the children are retreating away from the enemy
        }else if(currentState == state.wander){
            //make sure all the children are wandering around the level
            foreach (GameObject enemy in enemies)
            {
                if (!(enemy.GetComponent<testMovement>().state == testMovement.movementState.wandering))
                {
                    enemy.GetComponent<testMovement>().wander();
                }
            }
        }
    }


    IEnumerator makeDecisions()
    {
        while (true)
        {
            enemyCount = enemies.Count;
            averageDistance = 0;
            charging = 0;
            foreach (GameObject enemy in enemies)
            {
                testMovement ts = enemy.GetComponent<testMovement>();
                if (ts.radiusAt == 1f && enemy.GetComponent<testMovement>().state == testMovement.movementState.stayingAtDist)
                {
                    charging++;
                }
                averageDistance += Vector3.Distance(enemy.transform.position, ts.player.transform.position);

            }
            averageDistance = averageDistance / enemyCount;
            if (charging >= enemyCount * percentageChargingBeforeFullCharge)
            {
                currentState = state.charge;
            }

            if (averageDistance < averageDistanceBeforeCharge)
            {
                currentState = state.charge;
            }
            //If enemy is distracted then charge
            //Add enemy detection before charge too
            yield return new WaitForSeconds(0.2f);
        }
    }
}
