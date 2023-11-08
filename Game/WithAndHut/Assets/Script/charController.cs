using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class charController : MonoBehaviour
{
    //Sorry team, the file size may have gotten a bit bigger than planned, but it should all work fine
    public float speed;
    public Rigidbody rb;
    public float dashSpeedModifier;
    public float dashCooldown;
    public float dashDuration;

    bool dashFlag = false;
    float lastTimeSinceDashEnd;
    float timeAtDashStart;
    private void Start(){
        lastTimeSinceDashEnd = -dashCooldown;
    }

    void Update(){
        Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"),0, Input.GetAxisRaw("Vertical"));
        input = input.normalized;

        rb.velocity = input * speed;

        //dash
        if (Input.GetKeyDown(KeyCode.Space) && ((Time.realtimeSinceStartup - lastTimeSinceDashEnd) >= dashCooldown))
        {
            speed = speed * dashSpeedModifier;
            dashFlag = true;
            timeAtDashStart = Time.realtimeSinceStartup;
        }
        else if(dashFlag && (!Input.GetKey(KeyCode.Space) || (Time.realtimeSinceStartup - timeAtDashStart) >= dashDuration))
        {
            speed = speed / dashSpeedModifier;
            dashFlag = false;
            lastTimeSinceDashEnd = Time.realtimeSinceStartup;
        }
    }
}
