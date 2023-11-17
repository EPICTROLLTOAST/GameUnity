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
    public float maxAngle = 90f;
    public float timeTaken = 2f;
    public Vector3 currentEulerAngles;
    public bool DoorTurning = false;
    public float toggle = 1f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    float stopWatch = 0;
    void Update()
    {
        stopWatch += Time.deltaTime;
        float maxDistanceFromDoor = 3f;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        float distFromPlayer = (player.transform.position - gameObject.transform.position).magnitude;
        if(DoorTurning){
            gameObject.GetComponent<RotateAroundPivot>().angle = maxAngle * (stopWatch / timeTaken) * toggle;
            if(stopWatch > timeTaken){
                DoorTurning = false;
                toggle = 0f - toggle;
            }
        }else if(distFromPlayer <= maxDistanceFromDoor && Input.GetKeyDown(KeyCode.E)){
            stopWatch = 0; 
            DoorTurning = true;
            currentEulerAngles = gameObject.transform.eulerAngles;
        }
    }
}
