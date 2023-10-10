using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSeekerMovementControlller : BaseEnemyMovementScript
{

    AIDestinationSetter setter;

    // Start is called before the first frame update
    void Start()
    {
        
        setter = GetComponent<AIDestinationSetter>();
        setter.target = player.transform;
    }

    public override void controlMovement()
    {
        base.controlMovement();
    }
}
