using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRangedAttack : MonoBehaviour
{
    public GameObject rayToShoot;
    [SerializeField]
    float damage;
    [SerializeField]
    float seenPlayer = 0f;
    [SerializeField]
    [Range(0.01f,Mathf.Infinity)]
    float timeToFire;
    testMovement ts;
    LayerMask entity;
    // Start is called before the first frame update
    void Start()
    {
        ts = gameObject.GetComponent<testMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (ts.LineOfSightWithPlayer())
        {
            seenPlayer += Time.deltaTime;
        }
        if(seenPlayer >= timeToFire)
        {
            seenPlayer = 0f;
            GameObject ray = Instantiate(rayToShoot,gameObject.GetComponent<Transform>());
            ray.GetComponent<RayProjectile>().construct(damage,entity, ts.player.transform.position - gameObject.transform.position);
        }
    }
}
