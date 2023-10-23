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

    float walkingSpeed = 7.5f;
    float runningSpeed = 15f;

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
        playerMovementKeyCheck();
        playerMouseCheck();
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            print("Hello!");
            //Just call a new damage event to damage someone
            new DamageEvent(5f, damageTypes.FIRE, player, player);
        }


    }
    public void playerMovementKeyCheck()
    {
        velocity = new Vector2 (0f , 0f);
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKey(KeyCode.W))
            {
                velocity.y += runningSpeed;
            }
            if (Input.GetKey(KeyCode.S))
            {
                velocity.y -= runningSpeed;
            }

            if (Input.GetKey(KeyCode.D))
            {
                velocity.x += runningSpeed;
            }
            if (Input.GetKey(KeyCode.A))
            {
                velocity.x -= runningSpeed;
            }
        }
        else
        {
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
        }

        rigidbody.velocity = velocity;
    }

    public void playerMouseCheck()
    {
        //mouse actions
        if (Input.GetMouseButtonDown(0))
        {
            BaseItem item = playerInventory.getHeldItem();
            if (item != null)
            {
                playerInventory.getHeldItem().leftClickAction();
            }
            else
            {
                print("NO ITEM HELD");
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            BaseItem item = playerInventory.getHeldItem();
            if (item != null)
            {
                playerInventory.getHeldItem().rightClickAction();
            }
            else
            {
                print("NO ITEM HELD");
            }
        }
    }

    public void pickUpItem(BaseItem item)
    {
        playerInventory.addToInventory(item);
        playerInventory.displayInventoryToConsole();
    }
}