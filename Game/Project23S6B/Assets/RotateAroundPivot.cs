using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RotateAroundPivot : MonoBehaviour
{
    public float angle = 0f;
    float lastAngle = 0f;
    [SerializeField]
    GameObject anchor;

    void Start()
    {

    }
    


    void Update()
    {
        gameObject.transform.RotateAround(anchor.transform.position, new Vector3(0f,0f,1f),angle - lastAngle);
        lastAngle = angle;
    }

}
