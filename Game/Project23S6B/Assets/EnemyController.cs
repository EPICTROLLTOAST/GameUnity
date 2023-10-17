using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public GameObject player;
    Vector2 startPos;
    [SerializeField]
    bool movementLockFlag = false;
    public EnemyMovementManager movementManager;

    // Start is called before the first frame update
    public virtual void Start()
    {
        player = GameObject.FindWithTag("Player");
        movementManager = this.gameObject.GetComponent<EnemyMovementManager>();
    }

    


    private void Update()
    {
        if (movementLockFlag)
        {
            this.transform.position = startPos;
        }
    }



    public void movementLock(float time)
    {
        movementLockFlag = true;
        startPos = this.transform.position;
        Invoke("disableMovementLock", time);
    }

    private void disableMovementLock()
    {
        movementLockFlag = false;
    }
}
