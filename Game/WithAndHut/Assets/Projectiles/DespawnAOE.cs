using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DespawnAOE : MonoBehaviour{

    [SerializeField] float timeToDespawn;

    float timeAtStart;

    private void Start()
    {
        timeAtStart = Time.realtimeSinceStartup;
    }
    // Update is called once per frame
    void Update()
    {
        if (Time.realtimeSinceStartup-timeAtStart>timeToDespawn)
        {
            Destroy(gameObject);
        }
    }
}
