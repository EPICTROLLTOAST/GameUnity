using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rangedSquadManager : MonoBehaviour
{

    public enum state
    {
        hide,
        attack,
        retreat,
        wander
    }
    ArrayList enemies = new ArrayList() { };
    public state currentState = state.attack;

    // Start is called before the first frame update
    void Start()
    {
        Transform[] ts = gameObject.GetComponentsInChildren<Transform>();
        foreach (Transform enemy in ts)
        {
            enemies.Add(enemy.gameObject);
        }
        enemies.RemoveAt(0);
        foreach(GameObject enemy in enemies)
        {
            print(enemy.name);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (currentState == state.wander)
        {
            //make sure all the children are wandering around the level
            foreach (GameObject enemy in enemies)
            {
                if (!(enemy.GetComponent<testMovement>().state == testMovement.movementState.wandering))
                {
                    enemy.GetComponent<testMovement>().wander();
                }
            }
        }else if(currentState == state.attack)
        {
            foreach(GameObject enemy in enemies)
            {
                if (!(enemy.GetComponent<testMovement>().state == testMovement.movementState.stayingWithLOS))
                {
                    enemy.GetComponent<testMovement>().MoveToLineOfSight();
                }
            }
        }
    }
}
