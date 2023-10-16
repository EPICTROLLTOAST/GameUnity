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

    PlayerInventory playerInventory = new PlayerInventory();

    // Start is called before the first frame update
    void Start()
    {
        player = gameObject;
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

        //mouse actions
        if (Input.GetMouseButtonDown(0))
        {
            playerInventory.getHeldItem().leftClickAction();
        }
        if (Input.GetMouseButtonDown(1))
        {
            playerInventory.getHeldItem().rightClickAction();
        }
    }

    public void pickUpItem(BaseItem item)
    {
        playerInventory.addToInventory(item);
        playerInventory.displayInventoryToConsole();
    }

    public PlayerInventory getPlayerInventory()
    {
        return playerInventory;
    }
}