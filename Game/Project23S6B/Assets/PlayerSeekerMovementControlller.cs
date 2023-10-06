using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSeekerMovementControlller : BaseEnemyMovementScript
{

    [SerializeField]
    GameObject player;
    Seeker seeker;

    // Start is called before the first frame update
    void Start()
    {
        seeker = GetComponent<Seeker>();
        
    }

    public override void controlMovement()
    {
        base.controlMovement();
        seeker.StartPath(this.transform.position, player.transform.position);
        //TODO make the pathfinding work
    }
}
