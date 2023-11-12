using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthbarManager : MonoBehaviour
{
    GameObject player;
    float prevHealth;
    float maxHealth;
    float maxFillingSize = 392;
    [SerializeField]
    float changeHealthTime = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        maxHealth = player.GetComponent<PlayerHealth>().maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        //TODO make the animation of it decreasing or increaseing
    }
}
