using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BasePlayerController : MonoBehaviour
{
    GameObject player;
    Rigidbody2D rigidbody;
    Vector2 velocity;
    [SerializeField]
    float walkingSpeed = 1f;

    // Start is called before the first frame update
    void Start()
    {
        player = this.gameObject;
        rigidbody = player.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        velocity = new Vector2 (0f , 0f);
        if (Input.GetKey(KeyCode.W))
        {
            velocity.y += walkingSpeed;
        }
        if (Input.GetKey(KeyCode.S))
        {
            velocity.y -= walkingSpeed;
        }

        if (Input.GetKey(KeyCode.D))
        {
            velocity.x += walkingSpeed;
        }
        if (Input.GetKey(KeyCode.A))
        {
            velocity.x -= walkingSpeed;
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            print("Hello!");
            //Just call a new damage event to damage someone
            new DamageEvent(5f, damageTypes.FIRE, player, player);
        }

        rigidbody.velocity = velocity;
    }
}
