using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ItemInteractScript : BaseInteractScript
{  
    BaseItem item;
    private BasePlayerController playerControllerScript;
    bool playerInRange = false;

    [SerializeField] string itemID;
    [SerializeField] string itemName;
    public override void Start()
    {
        base.Start();
        playerControllerScript = getPlayerGameObject().GetComponent<BasePlayerController>();
        item = new RangedWeapon(itemName); //for testing 
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
    }

    public override void playerNotPresent(float distance)
    {
        playerInRange = false;
    }
}
