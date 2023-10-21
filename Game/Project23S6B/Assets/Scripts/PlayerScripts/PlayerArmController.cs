using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerArmController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //transform.Rotate(0f, 0f, 45f);
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 currentPosition = transform.position;
        float armAngleDeg = AngleBetweenPoints(mousePosition, transform.position);
        armAngleDeg = armAngleDeg - (transform.rotation.z * Mathf.Rad2Deg);

        transform.Rotate(0f, 0f, armAngleDeg);
        //print(armAngleDeg);
    }

    float AngleBetweenPoints(Vector2 a, Vector2 b) {
		float angle = Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;
        if (angle < 0)
        {
            return 180 + angle;
        }
        else
        {
            return angle;
        }
	}
}
