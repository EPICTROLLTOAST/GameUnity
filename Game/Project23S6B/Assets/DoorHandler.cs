using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR;

public class DoorHandler : MonoBehaviour
{
    [SerializeField]
    GameObject hinge;
    [SerializeField]
    float maxAngle = 90f;
    public float timeTaken = 2f;
    public bool DoorTurning = false;
    float toggle = 1f;

    [SerializeField]
    float maxDistanceFromDoor = 3f;
    float stopWatch = 0;
    float currentAngles = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        stopWatch += Time.deltaTime;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        float distFromPlayer = (player.transform.position - gameObject.transform.position).magnitude;
        if(DoorTurning){
            gameObject.GetComponent<RotateAroundPivot>().angle = currentAngles + maxAngle * (stopWatch / timeTaken) * toggle;
            if(stopWatch > timeTaken){
                DoorTurning = false;
                toggle = 0f - toggle;
                currentAngles = Math.Abs(90 - currentAngles);
            }
        }else if(distFromPlayer <= maxDistanceFromDoor && Input.GetKeyDown(KeyCode.E)){
            stopWatch = 0; 
            DoorTurning = true;
            currentAngles = gameObject.GetComponent<RotateAroundPivot>().angle;
        }
    }
}
