using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : MonoBehaviour{
    public GameObject projectile;
    public Camera cam;
    public LayerMask layerMask;
    public bool holdToggle;
    public float multiple;
    private void Update(){
        if ((Input.GetMouseButton(0) && holdToggle) || (Input.GetMouseButtonDown(0) && !holdToggle)){
            Vector3 target = Vector3.zero;

            Debug.DrawRay(cam.ScreenToWorldPoint(Input.mousePosition* multiple), cam.transform.rotation * Vector3.forward, Color.red, 10);

            RaycastHit hit;
            if (Physics.Raycast(cam.ScreenToWorldPoint(Input.mousePosition * multiple), cam.transform.rotation * Vector3.forward, out hit, 1000, layerMask)){
                target = hit.point;
                target.y = transform.position.y;
                transform.rotation = Quaternion.LookRotation(target-transform.position,Vector3.up);
            }


            GameObject clone = Instantiate(projectile, transform.position, transform.rotation);
            if (clone.GetComponent<PhysicsProjectile>()!=null){
                clone.GetComponent<PhysicsProjectile>().target = target;
            }
        }
    }

}
