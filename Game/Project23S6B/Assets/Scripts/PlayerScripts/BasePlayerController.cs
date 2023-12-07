using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BasePlayerController : MonoBehaviour, IDamageable
{
    GameObject player;
    Rigidbody2D rigidbody;
    Vector2 velocity;
    [SerializeField]

    float walkingSpeed = 7.5f;
    float runningSpeed = 15f;

    [SerializeField]
    public float maxHealth = 10f;
    [SerializeField]
    float health;
    [SerializeField]
    float knockbackResistance = 1f;
    public bool movementLockFlag = false;
    float timeSinceMovementLock = 0f;
    float timeToWaitForMovementLock = 0f;

    [SerializeField]
    float maxKnockbackSpeed = 20f;
    [SerializeField]
    float minKnockbackSpeed = 2f;
    [SerializeField]
    float maxMovementLocktime = 3f;
    [SerializeField]
    float minMovementLockTime = 1f;
    [SerializeField]
    float flungMultiplier = 2f;
    [SerializeField]
    float debugKnockback = 5f;
    bool isDashing = false;
    bool prepDashing = false;
    float dashTime = 0f;

    enum dashTypes{
        dashRelyingOnButtons,
        clickTheDashLocation
    }

    [SerializeField]
    dashTypes dashType = dashTypes.clickTheDashLocation;
    [SerializeField]
    [Range(0f, 200f)]
    float dashRadius = 5f;
    [SerializeField]
    [Range(0f, 200f)]
    float dashSpeedClick = 1f;
    Vector2 dashLocation;

    [SerializeField]
    [Range(0f, 1.95f)]
    float dashTimeButtons = 1f;
    [SerializeField]
    [Range(0f, 200f)]
    float dashDistanceButtons = 5f;
    [SerializeField]
    [Range(0f, 5f)]
    float delayBeforeStartingTDash = 0.5f;


    float dashTimeoutCounter = 0f;
    bool buttonsDashSpeedSet = false;
    
    // Start is called before the first frame update
    void Start()
    {
        player = gameObject;
        rigidbody = player.GetComponent<Rigidbody2D>();
        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if(isDashing || prepDashing){
            //dash timeout
            dashTimeoutCounter += Time.deltaTime;
        }

        //Enter dashing mode
        
        if(!movementLockFlag){
            if(!prepDashing && !isDashing){
                playerMovementKeyCheck();
            }

        }else{
            timeSinceMovementLock += Time.deltaTime;
            if(timeSinceMovementLock >= timeToWaitForMovementLock){
                movementLockFlag = false;
                timeSinceMovementLock = 0f;
                timeToWaitForMovementLock = 0f;
            }
        }
        
        playerMouseCheck();


        if(Input.GetKeyDown(KeyCode.Space) && !isDashing && !prepDashing){
            prepDashing = true;
            dashTimeoutCounter = 0f;
            gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }else if(isDashing){
            //Stop prep dashing

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
        if(prepDashing && dashTimeoutCounter > delayBeforeStartingTDash){
            if(dashType == dashTypes.clickTheDashLocation){

                if(Input.GetMouseButtonDown(0)){
                    //The location to dash to
                    dashLocation = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    if((dashLocation - (Vector2)gameObject.transform.position).magnitude > dashRadius){
                        //if dash exceeds dash radius, player will dash to the furthest point within 
                        //dash radius in the direction of the mouse
                        dashLocation = (dashLocation - (Vector2)gameObject.transform.position).normalized 
                        * dashRadius + (Vector2)gameObject.transform.position;
                    }
                    isDashing = true;
                    dashTimeoutCounter = 0f;
                    prepDashing = false;
                }

            }else if(dashType == dashTypes.dashRelyingOnButtons){
                Vector2 direction = new Vector2(0f,0f);
                bool buttonPressed = false;

                if(Input.GetKey(KeyCode.W)){
                    direction.y += 1f;
                    buttonPressed = true;
                }
                if(Input.GetKey(KeyCode.S)){
                    direction.y -= 1f;
                    buttonPressed = true;
                }
                if(Input.GetKey(KeyCode.D)){
                    direction.x += 1f;
                    buttonPressed = true;
                }
                if(Input.GetKey(KeyCode.A)){
                    direction.x -= 1f;
                    buttonPressed = true;
                }

                if(buttonPressed){
                    isDashing = true;
                    dashTimeoutCounter = 0f;
                    prepDashing = false;
                    dashLocation = (Vector2)gameObject.transform.position + 
                    direction.normalized * dashDistanceButtons;
                    buttonsDashSpeedSet = false;
                }


            }

        }else{
            if(isDashing){
                if(dashType == dashTypes.clickTheDashLocation){

                    if(Vector2.Distance(dashLocation, gameObject.transform.position) <= 0.2f){
                        isDashing = false;
                        gameObject.transform.position = dashLocation;
                    }
                    gameObject.GetComponent<Rigidbody2D>().velocity = 
                    dashSpeedClick * (dashLocation - (Vector2)gameObject.transform.position).normalized;
                    if(dashTimeoutCounter > 1f){
                        isDashing = false;
                        gameObject.transform.position = dashLocation;
                    }

                }else if(dashType == dashTypes.dashRelyingOnButtons){

                    if(!buttonsDashSpeedSet){
                        gameObject.GetComponent<Rigidbody2D>().velocity = 
                        (dashLocation - (Vector2)gameObject.transform.position) / dashTimeButtons;

                        buttonsDashSpeedSet = true;
                    }

                    //End the dash if enough time has passed
                    if(dashTimeoutCounter > dashTimeButtons){
                        isDashing = false;
                        gameObject.transform.position = dashLocation;                        
                    }   

                }

            }
        }

        //mouse actions
        // if (Input.GetMouseButtonDown(0))
        // {
        //     BaseItem item = PlayerInventory.Instance.getHeldItem();
        //     if (item != null)
        //     {
        //         PlayerInventory.Instance.getHeldItem().leftClickAction();
        //     }
        //     else
        //     {
        //         print("NO ITEM HELD");
        //     }
        // }
        // if (Input.GetMouseButtonDown(1))
        // {
        //     BaseItem item = PlayerInventory.Instance.getHeldItem();
        //     if (item != null)
        //     {
        //         PlayerInventory.Instance.getHeldItem().rightClickAction();
        //     }
        //     else
        //     {
        //         print("NO ITEM HELD");
        //     }
        // }
    }

    public void pickUpItem(BaseItem item)
    {
        PlayerInventory.Instance.addToInventory(item);
        PlayerInventory.Instance.displayInventoryToConsole();
    }

    public void recceiveDamage(DamageEvent damageEvent)
    {
        //Ignore damage if immune
        if(immunityCheck(damageEvent)){
            return;
        }

        health -= damageEvent.damageAmount;
        if (health <= 0 )
        {
            die();
        }
        print(damageEvent.knockbackValue);
        if(damageEvent.knockbackValue > knockbackResistance){
            //Full on being flung back
            //It's actually force not speed btw


            movementLockFlag = true;
            timeSinceMovementLock = 0f;
            float knockbackSpeed;

            if(damageEvent.knockbackValue >= 10f){
                timeToWaitForMovementLock = ((damageEvent.knockbackValue - knockbackResistance) * (maxMovementLocktime - minMovementLockTime) / (10f) + minMovementLockTime) * flungMultiplier;
                if (timeToWaitForMovementLock > maxMovementLocktime * flungMultiplier){
                    timeToWaitForMovementLock = maxMovementLocktime * flungMultiplier;
                }

                knockbackSpeed = ((damageEvent.knockbackValue - knockbackResistance) * (maxKnockbackSpeed - minKnockbackSpeed) / (10f) + minKnockbackSpeed) * flungMultiplier;
                if (knockbackSpeed > maxKnockbackSpeed * flungMultiplier){
                    knockbackSpeed = maxKnockbackSpeed * flungMultiplier;
                }

            }else{
                //From knockbackResistance - 10
                
                timeToWaitForMovementLock = (damageEvent.knockbackValue - knockbackResistance) * (maxMovementLocktime - minMovementLockTime) / (10f - knockbackResistance) + minMovementLockTime;
                knockbackSpeed = ((damageEvent.knockbackValue - knockbackResistance) * (maxKnockbackSpeed - minKnockbackSpeed) / (10f) + minKnockbackSpeed) * flungMultiplier;

            }
            damageEvent.target.GetComponent<Rigidbody2D>().velocity = new Vector2(0f,0f);
            print(damageEvent.getKnockbackDirection().normalized);
            print(knockbackSpeed);
            damageEvent.target.GetComponent<Rigidbody2D>().AddForce(damageEvent.getKnockbackDirection().normalized * knockbackSpeed * 10);
            
        }
    }

    public bool immunityCheck(DamageEvent damageEvent){
        if(isDashing){
            return true;
        }else if(prepDashing){
            prepDashing = false;

            return false;
        }

        return false;
    }

    private void die()
    {

    }

    public float getHealth(){
        return health;
    }
}