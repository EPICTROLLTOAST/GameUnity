using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;
using UnityEngine.Rendering.UI;

public class RayProjectile : MonoBehaviour{
    [SerializeField] float timeToDespawn;
    [SerializeField] int bounceNumber;
    [SerializeField] LayerMask enemys;
    [SerializeField] GameObject aoePrefab;
    [SerializeField] LineRenderer lineRenderer;
    Vector3 initialDirection = Vector3.zero;
    

    //Rayan's code
    [SerializeField] float damage;
    bool canDamage = true;
    //End code

    float timeStarted;

    List<Vector3> points;
    Vector3 lastPos, lastDir;
    private void Start(){
        timeStarted = Time.realtimeSinceStartup;

        
    }
    private void Update(){
        CalculateDespawn();
        if(!(initialDirection == Vector3.zero))
        {
            CalculatePoints();
            DrawLine();
        }
    }

    void CalculatePoints(){
        points = new List<Vector3>();
        points.Add(transform.position);

        lastPos = transform.position;
        lastDir = initialDirection;

        for (int i = 0; i <= bounceNumber; i++){
            RaycastHit hit;
            if (Physics.Raycast(lastPos, lastDir, out hit, Mathf.Infinity)){

                Debug.DrawRay(lastPos,lastDir,Color.blue,10);
                Debug.DrawRay(hit.point, hit.normal,Color.green,10);
                

                lastPos = hit.point;
                lastDir = Vector3.Reflect(lastDir, hit.normal);
                Debug.DrawRay(lastPos, lastDir, Color.red, 10);

                points.Add(hit.point);
                //Rayan's code
                if(hit.collider.gameObject.tag == "Enemy" || hit.collider.gameObject.tag == "Player")
                {
                    if (canDamage)
                    {
                        health entityHealth = hit.collider.gameObject.GetComponent<health>();
                        entityHealth.ChangeHP(-damage);
                        canDamage = false;
                    }
                    
                }
                //End Rayan's code
                if (i == bounceNumber || (enemys.value & 1 << hit.collider.gameObject.layer) == 1 << hit.collider.gameObject.layer){
                    Dammage(null);
                    break;
                }
            }else{
                Debug.Log(i);
                points.Add(lastPos + (lastDir * 100));
                break;
            }
        }
    }
    void DrawLine(){
        if (points.Count<2) { return; }

        lineRenderer.positionCount = points.Count;
        for (int i = 0; i < points.Count; i++){
            lineRenderer.SetPosition(i,points[i]);
        }
    }
    void CalculateDespawn(){
        if (Time.realtimeSinceStartup - timeStarted > timeToDespawn){
            Despawn();
        }
    }
    void Dammage(GameObject target){
        if (aoePrefab != null){
            Instantiate(aoePrefab, points[points.Count-1], transform.rotation);
        }
    }
    void Despawn(){
        Destroy(transform.gameObject);
    }
    
    //Rayan's code
    //Constructor for this ray
    public void construct(float damageR, LayerMask entity, Vector3 direction)
    {

        damage = damageR;
        enemys = entity;
        initialDirection = direction;
    }
    //End code
}
