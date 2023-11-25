using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField]
    float speed = 1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //make it so that if d is pressed, the camera moves right
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += new Vector3(speed * Time.deltaTime * gameObject.GetComponent<Camera>().orthographicSize , 0, 0);
        }
        //make it so that if a is pressed, the camera moves left
        if (Input.GetKey(KeyCode.A))
        {
            transform.position += new Vector3(-speed * Time.deltaTime * gameObject.GetComponent<Camera>().orthographicSize, 0, 0);
        }
        //make it so that if w is pressed, the camera moves up
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += new Vector3(0, speed * Time.deltaTime * gameObject.GetComponent<Camera>().orthographicSize, 0);
        }
        //make it so that if s is pressed, the camera moves down
        if (Input.GetKey(KeyCode.S))
        {
            print(gameObject.GetComponent<Camera>().orthographicSize);
            transform.position += new Vector3(0, -speed * Time.deltaTime * gameObject.GetComponent<Camera>().orthographicSize, 0);
        }
    }
}
