using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseInteractScript : MonoBehaviour
{
    [SerializeField]
    const float INTERACT_DISTANCE = 1f;
    [SerializeField]
    GameObject player;
    [SerializeField] 
    const float REFRESH_TIME = 0.1f;
    
    // Start is called before the first frame update
    public virtual void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        
        StartCoroutine(checkPlayerPresence());
    }

    IEnumerator checkPlayerPresence(){
        while (true)
        {
            float distance = Vector2.Distance(gameObject.transform.position, player.transform.position);
            if( distance <= INTERACT_DISTANCE)
            {
                playerPresent(distance);
            }
            else
            {
                playerNotPresent(distance);
            }
            yield return new WaitForSeconds(REFRESH_TIME);
        }
        
    }

    public virtual void playerPresent(float distance){
        
    }

    public virtual void playerNotPresent(float distance){
        
    }

    public GameObject getPlayerGameObject()
    {
        return player;
    }
}
