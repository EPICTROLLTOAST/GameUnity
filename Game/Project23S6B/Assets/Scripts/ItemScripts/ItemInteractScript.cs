using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ItemInteractScript : BaseInteractScript
{  
    BaseItem item;
    private BasePlayerController playerControllerScript;
    bool playerInRange = false;

    void Start()
    {
        base.Start();
        playerControllerScript = getPlayerGameObject().GetComponent<BasePlayerController>();
        item = new UsableItem("0", "triangle"); //for testing 
    }

    void Update()
    {
        if (playerInRange && Input.GetKey(KeyCode.F))
        {
            playerControllerScript.pickUpItem(item);
            Destroy(gameObject);
        }
    }

    public override void playerPresent(float distance)
    {
        playerInRange = true;

        //TODO: add function to set playerInRange to false after player out of range
    }
}
