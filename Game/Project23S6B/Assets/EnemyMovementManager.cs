using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;




[RequireComponent(typeof (AIPath), typeof(Seeker))]
public class EnemyMovementManager : MonoBehaviour
{



    GameObject player;
    AIPath pathfinder;
    AIDestinationSetter destinationSetter;

    public enum movementModes
    {
        idle,
        wanderingWithRadius,
        stayAtRadiusWithLOS,
        charge,
        retreat,
        noMovement
    }

    public movementModes mode = movementModes.idle;
    [SerializeField]
    private GameObject target;
    float deviation;
    int iterations;
    float baseRange;
    float rangeRange;
    float randomness;
    bool newPath = true;
    [Header("Wander settings")]
    public float wanderDev;
    public int wanderIters;
    public float wanderRange;
    public float wanderRandomness;
    public float wanderRandomRange;
    [Header("Staying at radius with LOS settings")]
    public float LOSRadius;
    public float LOSDeviation;
    public int LOSIterations;
    [Description("This one controls the range")]
    public int LOSRangeIterations;
    public float LOSMinRadius;
    public float LOSRangeRandomness;
    public float LOSRandomness;
    [Header("Flags")]
    public bool wanderFlag = false;
    public bool stayAtRadiusWithLOSFlag = false;
    public Vector3 wanderDest = Vector3.zero;
    public Vector3 stayAtRadiusWithLOSDest = Vector3.zero;

    public int timesRun = 0;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        pathfinder = GetComponent<AIPath>();
        destinationSetter = GetComponent<AIDestinationSetter>();
        StartCoroutine(switchModes());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator switchModes()
    {
        while (true)
        {
            switch (mode)
            {
                
                case movementModes.idle:
                    prepIdle();
                    break;
                case movementModes.wanderingWithRadius:
                    prepWanderRadius();
                    break;
                case movementModes.stayAtRadiusWithLOS:
                    prepStayAtRadiusWithLOS();
                    break;
                case movementModes.retreat:
                    prepRetreat();
                    break;
                case movementModes.charge:
                    prepCharge();
                    break;
                //case movementModes.noMovement:
            }
            timesRun++;
            yield return new WaitForSeconds(0.2f + Random.Range(0.1f,0.2f));
        }
    }




    void prepIdle()
    {
        idle();
    }

    void prepWanderRadius()
    {
        if (!wanderFlag)
        {
            deviation = wanderDev;
            iterations = 20;
            randomness = wanderRandomness;
            rangeRange = wanderRandomRange;
            baseRange = wanderRange;
            wanderRadius();
            wanderFlag = true;
            destinationSetter.target = null;
        }
        else
        {
            if (Vector2.Distance(wanderDest,gameObject.transform.position) < 0.5f)
            {
                wanderFlag = false;
            }
        }
        
    }

    void prepCharge()
    {
        charge();
    }

    void prepStayAtRadiusWithLOS()
    {
        if (!stayAtRadiusWithLOSFlag)
        {
            baseRange = LOSRadius;
            deviation = LOSDeviation;
            iterations = LOSIterations;
            randomness = LOSRandomness;
            rangeRange = LOSRangeRandomness;
            stayAtRadiusWithLOSFlag = true;
            stayAtRadiusWithLOS();
            destinationSetter.target = null;
        }
        else
        {
            if (!hasLOS(stayAtRadiusWithLOSDest,player) || Vector2.Distance(player.transform.position,stayAtRadiusWithLOSDest) < (LOSMinRadius - 1f))
            {
                stayAtRadiusWithLOSFlag = false;
            }
        }
        
    }

    void prepRetreat()
    {

    }


    void idle()
    {
        pathfinder.destination = gameObject.transform.position;
    }

    void wanderRadius()
    {
        float rangeUse = baseRange;
        for (int x = 0; x < wanderIters; x++)
        {
            ArrayList positions = GetMoveableAtRadius(deviation, iterations, gameObject.transform.position + Vector3.one * 0.001f + new Vector3(Random.Range(-randomness, randomness), Random.Range(-randomness, randomness), 0f),
                        rangeUse + Random.Range(-rangeRange, rangeRange));

            if (positions.Count > 1)
            {
                pathfinder.destination = (Vector3)positions[Random.Range(0, positions.Count - 1)];
                wanderDest = pathfinder.destination;
                newPath = false;
            }
            else
            {
                rangeUse -= baseRange/wanderIters;
            }
        }

    }

    void charge()
    {
        destinationSetter.target = target.transform;
    }


    void stayAtRadiusWithLOS()
    {
        ArrayList positionsStore = new ArrayList();
        float rangeUse = baseRange;
        for (int x = 0; x < LOSRangeIterations; x++)
        {
            ArrayList positions = GetMoveableAtRadiusWithLOS(deviation, iterations, player.transform.position + new Vector3(Random.Range(-randomness, randomness), Random.Range(-randomness, randomness), 0f),
                            rangeUse + Random.Range(-rangeRange, rangeRange));
            rangeUse -= (baseRange - LOSMinRadius) / LOSRangeIterations;
            foreach (Vector3 position in positions)
            {
                positionsStore.Add(position);
            }
        }

        if (positionsStore.Count >= 1)
        {
            Vector3 closest = Vector3.positiveInfinity;
            float distance = 50000000000f;

            foreach (Vector3 position in positionsStore)
            {
                float newDist = Vector2.Distance(position, gameObject.transform.position);
                if (newDist < distance)
                {
                    closest = position;
                    distance = newDist;
                }
            }


            pathfinder.destination = closest;
            stayAtRadiusWithLOSDest = pathfinder.destination;
            newPath = false;
        }
        else
        {
            newPath = true;
        }
    }

    void retreat()
    {

    }

    //second gameobject must have collider2D
    public bool hasLOS(Vector2 pos, GameObject second)
    {
        int layerName = gameObject.layer;
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        RaycastHit2D hit = Physics2D.Raycast(pos,(Vector2)second.transform.position-pos);
        gameObject.layer = layerName;
        try
        {
            if (hit.rigidbody.gameObject == second)
            {
                return true;
            }
        }
        catch
        {
            return false;
        }
        
        return false;
    }

    public bool hasLOSToPlayer(Vector2 pos){
        return hasLOS(pos, player);
    }


    ArrayList GetMoveableAtRadiusWithLOS(float dev, int iterationsR, Vector3 origin, float radius)
    {
        ArrayList moveablePos = GetMoveableAtRadius(dev, iterationsR, origin, radius);
        ArrayList withLOS = new ArrayList();
        foreach (Vector3 position in moveablePos)
        {
            if (hasLOS(position, player))
            {
                withLOS.Add(position);
            }
        }
        return withLOS;
    }

    ArrayList GetMoveableAtRadius(float dev, int iterationsR, Vector3 origin, float radius)
    {
        Transform enemyTransform = gameObject.GetComponent<Transform>();
        float radiusAt = radius;
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
                Vector3 testDestination = new Vector3(origin.x + maxX, origin.y + maxZ, origin.z);



                if (PathUtilities.IsPathPossible(AstarPath.active.GetNearest(enemyTransform.position).node, AstarPath.active.GetNearest(testDestination).node))
                {
                    positions.Add(testDestination);
                }
                else
                {
                    testDestination = new Vector3(origin.x + minX, origin.y + minZ, origin.z);
                    if (PathUtilities.IsPathPossible(AstarPath.active.GetNearest(enemyTransform.position).node, AstarPath.active.GetNearest(testDestination).node))
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

                Vector3 testDestination = new Vector3(origin.x - maxX, origin.y + maxZ, origin.z);
                if (PathUtilities.IsPathPossible(AstarPath.active.GetNearest(enemyTransform.position).node, AstarPath.active.GetNearest(testDestination).node))
                {
                    positions.Add(testDestination);
                }
                else
                {
                    testDestination = new Vector3(origin.x - minX, origin.y + minZ, origin.z);
                    if (PathUtilities.IsPathPossible(AstarPath.active.GetNearest(enemyTransform.position).node, AstarPath.active.GetNearest(testDestination).node))
                    {
                        positions.Add(testDestination);
                    }
                }
            }
        }
        return positions;

    }
}
