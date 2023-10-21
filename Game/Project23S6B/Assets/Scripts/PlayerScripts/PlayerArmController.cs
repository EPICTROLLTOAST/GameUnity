using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.UI;
using UnityEngine;

public class PlayerArmController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        //Get mouse world position
        Vector3 mouseScreenPosition = Input.mousePosition;
        mouseScreenPosition.z = transform.position.z;
        Vector3 mouseWorldSpace = Camera.main.ScreenToWorldPoint(mouseScreenPosition);

        //Rotate player arm
        transform.LookAt(mouseWorldSpace, Vector3.forward);
        //remove x and y rotations
        transform.eulerAngles = new Vector3(0, 0, -transform.eulerAngles.z);
    }
}
