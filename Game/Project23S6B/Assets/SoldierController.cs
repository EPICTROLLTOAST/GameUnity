using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class SoldierController : MonoBehaviour
{
    public AIPath aiPath;
    public Seeker seeker;

    void Start()
    {
        //Get the AIPath and seeker component of the soldier\
        aiPath = gameObject.GetComponent<AIPath>();
        seeker = gameObject.GetComponent<Seeker>();
    }

    public void activatePathfinding(){
        //Activate the pathfinding of aipath and seeker
        aiPath.enabled = true;
        seeker.enabled = true;
    }

    public void deactivatePathfinding(){
        //Deactivate the pathfinding of aipath and seeker
        aiPath.enabled = false;
        seeker.enabled = false;
    }

    
}
