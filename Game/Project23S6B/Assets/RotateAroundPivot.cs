using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RotateAroundPivot : MonoBehaviour
{
    public float angle = 0f;
    float lastAngle = 0f;
    GameObject anchor;

    void Start()
    {

        foreach(Transform child in gameObject.transform){
            anchor = child.gameObject;
        }

        StartCoroutine("changeAngle");
    }

    IEnumerator changeAngle(){
        while (true){
            gameObject.transform.RotateAround(anchor.transform.position, new Vector3(0f,0f,1f),angle - lastAngle);
            lastAngle = angle;
            print("hello!");
            yield return new WaitForSeconds(0.1f);
        }
    }

}
