using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemyAttackController : MonoBehaviour
{
    public GameObject player;
    Vector2 startPos;
    [SerializeField]
    bool movementLockFlag = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public virtual void controlAttack()
    {

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
