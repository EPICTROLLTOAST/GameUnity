using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsProjectile : MonoBehaviour{

    [SerializeField] float speed;
    [SerializeField] float timeToDespawn;
    [SerializeField] bool throwWithArc;
    [SerializeField] int bounceNumber;
    [SerializeField] LayerMask enemys;
    [SerializeField] GameObject aoePrefab;
    [SerializeField] Rigidbody body;


    public Vector3 target;
    float timeStarted;
    float bounceCoolDown;

    private void Start(){
        CalculateTrowVel();
        timeStarted = Time.realtimeSinceStartup;
    }
    private void FixedUpdate(){
        CalculateMovement();
        CalculateDespawn();
    }
    private void OnCollisionEnter(Collision c){
        if (Time.realtimeSinceStartup - bounceCoolDown < 0.1f) { return; }
        bounceCoolDown = Time.realtimeSinceStartup;


        if ((enemys.value & 1 << c.gameObject.layer) == 1 << c.gameObject.layer){
            Dammage(c.gameObject);
        }
        else{
            CalculateBounce(c.GetContact(0).normal);
        }
    }




    void CalculateTrowVel(){
        if (!throwWithArc) { return; }

        float d = Vector3.Distance(transform.position, target);
        float time = (d / (speed))*0.5f;

        float u = 0 - (-9f) * time;

        body.velocity = new Vector3(body.velocity.x,u, body.velocity.z);
    }
    void CalculateMovement(){
        Vector3 targetForward = transform.rotation * Vector3.forward;

        body.velocity = new Vector3(targetForward.x * (speed), body.velocity.y,targetForward.z * (speed));
    }
    void CalculateBounce(Vector3 normal){
        if (bounceNumber>0){
            Vector3 newVector = Vector3.Reflect(transform.rotation * Vector3.forward, normal);
            newVector.y = 0;
            newVector = newVector.normalized;


            transform.rotation = Quaternion.LookRotation(newVector, Vector3.up);
            bounceNumber -= 1;
        }
        else{
            Dammage(null);
        }
    }
    void CalculateDespawn(){
        if (Time.realtimeSinceStartup-timeStarted > timeToDespawn){
            Dammage(null);
        }
    }
    void Dammage(GameObject target){
        if (aoePrefab!=null){
            Instantiate(aoePrefab, transform.position, transform.rotation);
        }
        Destroy(transform.gameObject);
    }
}
