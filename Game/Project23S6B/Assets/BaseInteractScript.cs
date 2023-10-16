using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseInteractScript : MonoBehaviour
{
    [SerializeField]
    const float INTERACT_DISTANCE = 1f;
    GameObject player;
    const float REFRESH_TIME = 0.1f;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    IEnumerator checkPlayerPresence(){
        float distance = Vector2.Distance(gameObject.transform.position, player.transform.position);
        if( distance <= INTERACT_DISTANCE){
            playerPresent(distance);
        }
        yield return new WaitForSeconds(REFRESH_TIME);
    }

    public virtual void playerPresent(float distance){
        print("The player is " + distance + " units away!");
    }
}
