using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings;

public class testMovement : MonoBehaviour
{
    //TODO reserve the area around a hiding spot, so other enemies cant go there
    public Camera camera;
    public NavMeshAgent navMeshAgent;
    public Transform enemyTransform;
    public GameObject player;
    public LayerMask playerLayer;
    public GameObject marker;
    public GameObject playerPretend;
    public Vector3 destination;

    Vector3 moveTowards;
    public enum movementState
    {
        movingToPlayer,
        runningFromPlayer,
        stayingAtDist,
        holdingPos,
        wandering,
        stayingWithLOS,
        idle
    }
    public movementState state = movementState.wandering;

    public float radiusAt = 0f;
    Vector3 previousPlayerPos = Vector3.zero;
    float runAwayDev = 30f;
    int runAwayIters = 6;
    public Vector3 hidingSpot = Vector3.zero;
    float lastSinceHidingCalled = 0f;
    public float timeSeenByPlayer = 0f;
    float timeNotSeen = 0f;

    [SerializeField]
    float minLOSRad;
    [SerializeField]
    float maxLOSRad;
    

    //Start method 
    void Start()
    {
        runAwayIters = 6;
        runAwayDev = 30f;
        hidingSpot = Vector3.zero;
        radiusAt = 0f;
        previousPlayerPos = Vector3.zero;
    }


    // Update is called once per frame
    void Update()
    {
        destination = navMeshAgent.destination;
        if (Vector3.Distance(hidingSpot ,enemyTransform.position) < 1f && state != movementState.holdingPos)
        {
            state = movementState.holdingPos;
        }
        //Flag check
        if (state == movementState.movingToPlayer)
        {

           navMeshAgent.SetDestination(player.transform.position);

        }else if (state == movementState.runningFromPlayer)
        {
            if (!previousPlayerPos.Equals(player.transform.position))
            {
                float dist = 300f;
                float reduction = 5f;
                int counter = 0;
                StayAtRadius(dist);
                
                while (!MoveToRadius(60f, 8) || dist - (reduction * counter) <= 0)
                {
                    counter++;
                    StayAtRadius(dist - (reduction * counter));   
                }

                previousPlayerPos = player.transform.position;
            }

        }else if (state == movementState.stayingAtDist)
        {
            if(!previousPlayerPos.Equals(player.transform.position))
            {
                //TODO 
                MoveToRadius(180f,15);
                previousPlayerPos = player.transform.position;
            }  
        }else if (state == movementState.holdingPos)
        {
            //Included 0.2s delay
            if ((Time.timeSinceLevelLoad - lastSinceHidingCalled) >= 0.2f)
            {
                lastSinceHidingCalled = Time.timeSinceLevelLoad;
                holdCurrentPos(5f);
            }
        }else if (state == movementState.wandering)
        {
            Vector3 chosen = wanderAround(10f,28f,8);
            if(chosen != Vector3.zero)
            {
                if(Vector3.Distance(moveTowards,enemyTransform.position) < 1f)
                {
                    moveTowards = chosen;
                    navMeshAgent.destination = chosen;
                }
            }
        }else if (state == movementState.stayingWithLOS)
        {
            float dist = Vector3.Distance(enemyTransform.position, player.transform.position);
            if (dist > minLOSRad && dist < maxLOSRad && LineOfSightWithPlayer())
            {

            }
            else
            {
                Vector3 chosen = posWithLOS(8f, 16f, 8);
                if (chosen != Vector3.zero)
                {
                    if (Vector3.Distance(moveTowards, enemyTransform.position) < 1f)
                    {
                        moveTowards = chosen;
                        navMeshAgent.destination = chosen;
                    }
                }
            }

            
        }


        RaycastHit hit;
        playerPretend.transform.position = new Vector3(-100,-100,-100);
        if (Physics.Raycast(enemyTransform.position, player.transform.position - enemyTransform.position, out hit, Mathf.Infinity))
        {
            if(hit.collider.gameObject == player)
            {
                timeNotSeen = 0f;
                timeSeenByPlayer += Time.deltaTime;
            }
            else
            {
                timeNotSeen += Time.deltaTime;
                if(timeNotSeen >= 0.2f)
                {
                    timeSeenByPlayer = 0f;
                }
            }
        }
        else
        {
            timeNotSeen += Time.deltaTime;
            if(timeNotSeen >= 0.2f)
            {
                timeSeenByPlayer = 0f;
            }  
        }
    }
    public bool LineOfSightWithPlayer() { return LineOfSightWithPlayer(enemyTransform.position); }
    public bool LineOfSightWithPlayer(Vector3 position)
    {
        RaycastHit hit;
        if (Physics.Raycast(position,player.transform.position - position, out hit))
        {
            if(hit.collider.gameObject == player)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }


    //Function that makes enemies move towards the player
    void MoveToPlayer() { MoveToPlayer(player); }
    //Function that makes enemies run from player
    //Maximum deviation defines the angle of the cone cast from player to enemy
    //Iters defines the number of test lines drawn
    void RunFromPlayer(float maxdev, int iters) { RunFromPlayer(player, maxdev, iters); }
    //Tells enemies to stay at a radius from the player
    //If there are no available paths, whelp, need to add some code to go around that later
    public void StayAtRadius(float radius)
    {

        //updating hover radius
        radiusAt = radius;
        //updating flags
        state = movementState.stayingAtDist;
    }
    //The following are Utilities and overrides
    bool MoveToRadius(float dev, int iterationsR)
    {
        bool done = false;

        //TODO optimise this with a statement that checks whether or not to calculate again based on whether or not the player moved

        bool isLeftOfPlayer = player.transform.position.x < enemyTransform.position.x ? false : true;

        float maxDev = dev;
        float gradient = (Mathf.Atan((player.transform.position.z - enemyTransform.position.z) / (player.transform.position.x - enemyTransform.position.x)));
        int iterations = iterationsR;
        float iter = maxDev / iterations;

        for (int i = 0; i < iterations; i++)
        {
            if (!isLeftOfPlayer)
            {
                float maxGrad = ((iter * i * Mathf.Deg2Rad) + gradient);
                float minGrad = (gradient - (iter * i * Mathf.Deg2Rad));

                float maxX = Mathf.Cos(maxGrad) * radiusAt;
                float maxZ = Mathf.Sin(maxGrad) * radiusAt;

                float minX = Mathf.Cos(minGrad) * radiusAt;
                float minZ = Mathf.Sin(minGrad) * radiusAt;

                Vector3 testDestination = new Vector3(player.transform.position.x + maxX, player.transform.position.y, player.transform.position.z + maxZ);
                NavMeshPath path = new NavMeshPath();
                NavMesh.CalculatePath(enemyTransform.position, testDestination, NavMesh.AllAreas, path);

                if (path.status == NavMeshPathStatus.PathComplete)
                {
                    done = true;
                    navMeshAgent.SetDestination(testDestination);
                }
                else
                {
                    testDestination = new Vector3(player.transform.position.x + minX, player.transform.position.y, player.transform.position.z + minZ);
                    NavMesh.CalculatePath(enemyTransform.position, testDestination, NavMesh.AllAreas, path);
                    if (path.status == NavMeshPathStatus.PathComplete)
                    {
                        done = true;
                        navMeshAgent.SetDestination(testDestination);
                    }
                }
            }
            else
            {
                float maxGrad = -((iter * i * Mathf.Deg2Rad) + gradient);
                float minGrad = -(gradient - (iter * i * Mathf.Deg2Rad));

                float maxX = Mathf.Cos(maxGrad) * radiusAt;
                float maxZ = Mathf.Sin(maxGrad) * radiusAt;

                float minX = Mathf.Cos(minGrad) * radiusAt;
                float minZ = Mathf.Sin(minGrad) * radiusAt;

                Vector3 testDestination = new Vector3(player.transform.position.x - maxX, player.transform.position.y, player.transform.position.z + maxZ);
                NavMeshPath path = new NavMeshPath();
                NavMesh.CalculatePath(enemyTransform.position, testDestination, NavMesh.AllAreas, path);

                if (path.status == NavMeshPathStatus.PathComplete)
                {
                    done = true;
                    navMeshAgent.SetDestination(testDestination);
                }
                else
                {
                    testDestination = new Vector3(player.transform.position.x - minX, player.transform.position.y, player.transform.position.z + minZ);
                    NavMesh.CalculatePath(enemyTransform.position, testDestination, NavMesh.AllAreas, path);
                    if (path.status == NavMeshPathStatus.PathComplete)
                    {
                        done = true;
                        navMeshAgent.SetDestination(testDestination);
                    }
                }
            }

            if (done)
            {
                break;
            }

        }

        if (done)
        {
            //returns true if it did find a proper place for the enemy to go
            return true;
        }
        //returns false if it couldnt find a proper place for the enemy to go
        return false;
    }
    public void MoveToPlayer(GameObject playerReplace)
    {
        //replacing player to use in main loop
        player = playerReplace;
        //Updating flag so it checks it in main loop
        state = movementState.movingToPlayer;
    }
    public void RunFromPlayer() { RunFromPlayer(player , runAwayDev, runAwayIters); }
    public void RunFromPlayer(GameObject playerReplace, float maxDev, int iters)
    {
        //replacing player
        player = playerReplace;
        runAwayDev = maxDev;
        runAwayIters = iters;

        //updating flags
        state = movementState.runningFromPlayer;
    }
    public void MoveToLineOfSight()
    {
        state = movementState.stayingWithLOS;
        moveTowards = enemyTransform.position;
    }
    public void wander()
    {
        state = movementState.wandering;
        moveTowards = enemyTransform.position;
    }
    void StopMovingToPlayer()
    {
        //Updating flag os it stops moving to player
        state = movementState.idle;
    }
    
    ArrayList GetMoveableAtRadius(float dev, int iterationsR) { return GetMoveableAtRadius(dev, iterationsR, player.transform.position); }
    ArrayList GetMoveableAtRadius(float dev, int iterationsR, Vector3 origin)
    {
        ArrayList positions = new ArrayList();

        bool isLeftOfPlayer = origin.x < enemyTransform.position.x ? false : true;

        float maxDev = dev;
        float gradient = (Mathf.Atan((origin.z - enemyTransform.position.z) / (origin.x - enemyTransform.position.x)));
        int iterations = iterationsR;
        float iter = maxDev / iterations;

        for (int i = 0; i < iterations; i++)
        {
            if (!isLeftOfPlayer)
            {
                float maxGrad = (iter * i * Mathf.Deg2Rad) + gradient;
                float minGrad = gradient - (iter * i * Mathf.Deg2Rad);

                float maxX = Mathf.Cos(maxGrad) * radiusAt;
                float maxZ = Mathf.Sin(maxGrad) * radiusAt;

                float minX = Mathf.Cos(minGrad) * radiusAt;
                float minZ = Mathf.Sin(minGrad) * radiusAt;
                Vector3 testDestination = new Vector3(origin.x + maxX, origin.y, origin.z + maxZ);
                NavMeshPath path = new NavMeshPath();
                NavMesh.CalculatePath(enemyTransform.position, testDestination, NavMesh.AllAreas, path);

                if (path.status == NavMeshPathStatus.PathComplete)
                {
                    positions.Add(testDestination);
                }
                else
                {
                    testDestination = new Vector3(origin.x + minX, origin.y, origin.z + minZ);
                    NavMesh.CalculatePath(enemyTransform.position, testDestination, NavMesh.AllAreas, path);
                    if (path.status == NavMeshPathStatus.PathComplete)
                    {
                        positions.Add(testDestination);
                    }
                }
            }
            else
            {
                float maxGrad = -((iter * i * Mathf.Deg2Rad) + gradient);
                float minGrad = -(gradient - (iter * i * Mathf.Deg2Rad));

                float maxX = Mathf.Cos(maxGrad) * radiusAt;
                float maxZ = Mathf.Sin(maxGrad) * radiusAt;

                float minX = Mathf.Cos(minGrad) * radiusAt;
                float minZ = Mathf.Sin(minGrad) * radiusAt;

                Vector3 testDestination = new Vector3(origin.x - maxX, origin.y, origin.z + maxZ);
                NavMeshPath path = new NavMeshPath();
                NavMesh.CalculatePath(enemyTransform.position, testDestination, NavMesh.AllAreas, path);

                if (path.status == NavMeshPathStatus.PathComplete)
                {
                    positions.Add(testDestination);
                }
                else
                {
                    testDestination = new Vector3(origin.x - minX, origin.y, origin.z + minZ);
                    NavMesh.CalculatePath(enemyTransform.position, testDestination, NavMesh.AllAreas, path);
                    if (path.status == NavMeshPathStatus.PathComplete)
                    {
                        positions.Add(testDestination);
                    }
                }
            }
        }
        return positions;

    }

    public Vector3 GetHidingPosition(float minRadR, float maxRadR, int itersRad) { return GetHidingPosition(20f, 6, minRadR, maxRadR, itersRad, player.transform.position); }
    //Returns possible hiding positions, returns (0,0,0) if no valid positions are found
    public Vector3 GetHidingPosition(float dev, int itersA, float minRadR, float maxRadR, int itersRad, Vector3 origin)
    {
        //TODO, make sure it doesn't give hiding spots reserved for other players
        //TODO implement layer mask so I can give back the collider to enemy
        //TODO make this more customisable
        playerPretend.transform.position = player.transform.position;
        playerPretend.transform.localScale = new Vector3(5,1,5);
        float minRad = minRadR;
        float maxRad = maxRadR;
        int iters = itersRad;
        float originalRad = radiusAt;

        for(int x = 0; x < iters; x++)
        {
            Vector3 toUse = Vector3.zero;
            radiusAt = (maxRad - minRad)/iters * x + minRad;
            ArrayList positions = GetMoveableAtRadius(dev , itersA, origin);
            foreach(Vector3 pos in positions)
            {
                RaycastHit hit;
                if(Physics.Raycast(pos,player.transform.position - pos, out hit,Mathf.Infinity))
                {
                    if (hit.collider.gameObject == player || hit.collider.gameObject == playerPretend)
                    {
                        continue;
                    }
                }
                Vector3 altDir = (player.transform.position - pos);

                altDir.x = altDir.x * 0.8f;
                altDir.z = altDir.z * 0.8f;
                if (Physics.Raycast(pos, altDir, out hit, Mathf.Infinity))
                {
                    if (hit.collider.gameObject == player || hit.collider.gameObject == playerPretend)
                    {
                        continue;
                    }
                }
                altDir = (player.transform.position - pos);

                altDir.x = altDir.x * 1.2f;
                altDir.z = altDir.z * 1.2f;

                if (Physics.Raycast(pos, altDir, out hit, Mathf.Infinity))
                {
                    if (hit.collider.gameObject == player || hit.collider.gameObject == playerPretend)
                    {
                        continue;
                    }
                }
                altDir = (player.transform.position - pos);

                altDir.x = altDir.x * 1.1f;
                altDir.z = altDir.z * 1.1f;

                if (Physics.Raycast(pos, altDir, out hit, Mathf.Infinity))
                {
                    if (hit.collider.gameObject == player || hit.collider.gameObject == playerPretend)
                    {
                        continue;
                    }
                }

                altDir = (player.transform.position - pos);

                altDir.x = altDir.x * 0.9f;
                altDir.z = altDir.z * 0.9f;

                if (Physics.Raycast(pos, altDir, out hit, Mathf.Infinity))
                {
                    if (hit.collider.gameObject == player || hit.collider.gameObject == playerPretend)
                    {
                        continue;
                    }
                }
               
                radiusAt = originalRad;
                return pos;
            }
        }

        radiusAt = originalRad;
        print("Returning Vector3 zero");
        return Vector3.zero;
    }

    bool CloseToLOSOfPlayer(Vector3 pos)
    {
        RaycastHit hit;
        if (Physics.Raycast(pos, player.transform.position - pos, out hit, Mathf.Infinity))
        {
            if (hit.collider.gameObject == player || hit.collider.gameObject == playerPretend)
            {
                return true;
            }
        }
        Vector3 altDir = (player.transform.position - pos);

        altDir.x = altDir.x * 0.8f;
        altDir.z = altDir.z * 0.8f;
        if (Physics.Raycast(pos, altDir, out hit, Mathf.Infinity))
        {
            if (hit.collider.gameObject == player || hit.collider.gameObject == playerPretend)
            {
                return true;
            }
        }
        altDir = (player.transform.position - pos);

        altDir.x = altDir.x * 1.2f;
        altDir.z = altDir.z * 1.2f;

        if (Physics.Raycast(pos, altDir, out hit, Mathf.Infinity))
        {
            if (hit.collider.gameObject == player || hit.collider.gameObject == playerPretend)
            {
                return true;
            }
        }
        altDir = (player.transform.position - pos);

        altDir.x = altDir.x * 1.1f;
        altDir.z = altDir.z * 1.1f;

        if (Physics.Raycast(pos, altDir, out hit, Mathf.Infinity))
        {
            if (hit.collider.gameObject == player || hit.collider.gameObject == playerPretend)
            {
                return true;
            }
        }

        altDir = (player.transform.position - pos);

        altDir.x = altDir.x * 0.9f;
        altDir.z = altDir.z * 0.9f;

        if (Physics.Raycast(pos, altDir, out hit, Mathf.Infinity))
        {
            if (hit.collider.gameObject == player || hit.collider.gameObject == playerPretend)
            {
                return true;
            }
        }
        return false;
    }

    void holdCurrentPos(float distBeforeCharge)
    {
        if (Vector3.Distance(player.transform.position, enemyTransform.position) < distBeforeCharge)
        {
            navMeshAgent.SetDestination(enemyTransform.position);
            //Charge player, in range
            StayAtRadius(1f);
            return;
        }

        if (CloseToLOSOfPlayer(enemyTransform.position))
        {
            Vector3 origin = enemyTransform.position;
            origin.x = origin.x + 0.01f;
            origin.z = origin.z + 0.01f;

            Vector3 moveTo = GetHidingPosition(360f, 18, 2f, 8f, 6, origin);   
            if (moveTo != Vector3.zero)
            {
                navMeshAgent.SetDestination(moveTo);
            }
            else
            {
                print("Rushing player");
                StayAtRadius(1f);
                //Charge player, no way out
                return;
            }
        }



    }
    //Returns vector3.zero if no valid positions
    Vector3 wanderAround(float min, float max, int iters)
    {
        Vector3 chosen = Vector3.zero;
        for (int x = 0; x < iters; x++)
        {
            float current = max - (max - min) / iters * x;
            float restore = radiusAt;
            radiusAt = current;
            ArrayList positions = GetMoveableAtRadius(180f, 8, enemyTransform.position + new Vector3(0.1f,0f,0.1f));
            if(positions.Count != 0)
            {
                print(positions[0].ToString());
                radiusAt = restore;
                int chosenInt = (int)Random.Range(0, positions.Count);
                return (Vector3)positions[chosenInt == positions.Count?chosenInt - 1:chosenInt];
            }
            radiusAt = restore;
        }
        return Vector3.zero;
    }

    Vector3 posWithLOS(float minRad, float maxRad, int iters)
    {
        ArrayList chosen = new ArrayList() { };
        float reset = 0;
        for (int x  = 0; x < iters; x++)
        {
            reset = radiusAt;
            radiusAt = maxRad - ((maxRad - minRad) / iters * x);
            ArrayList positions = GetMoveableAtRadius(90, 8);
            if (positions.Count > 0)
            {
                foreach(Vector3 position in positions)
                {
                    if (LineOfSightWithPlayer(position))
                    {
                        chosen.Add(position);
                    }
                    if(chosen.Count > 0)
                    {
                        int chosenInt = (int)Random.Range(0, chosen.Count);
                        return (Vector3)chosen[chosenInt == chosen.Count ? chosenInt - 1 : chosenInt];
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            else
            {
                continue;
            }
        }
        return Vector3.zero;
    }

}
