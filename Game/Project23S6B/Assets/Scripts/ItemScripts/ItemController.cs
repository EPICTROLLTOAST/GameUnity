using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    public GameObject player;
    BaseItem item;
    private BasePlayerController playerControllerScript;

    float limit = 1f; //when item is within 1 unit of player, it will get picked up

    // Start is called before the first frame update
    void Start()
    {
        playerControllerScript = player.GetComponent<BasePlayerController>();
        item = new BaseItem("0", "triangle");
    }

    // Update is called once per frame
    void Update()
    {
        //if player is within the limit distance, then the item is added to player inventory and then destroyed.
        if (checkPlayerProximity(limit))
        {
            playerControllerScript.pickUpItem(item);
            Destroy(gameObject);
        }

        //do some item animation or something idk
    }

    bool checkPlayerProximity(float limit)
    {
        //return true if distance to player is less than or equals to limit
        float distance = Vector3.Distance(player.transform.position, transform.position);

        if (distance <= limit)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //IEnumerator checkPlayer(){}
}
