using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using System.Reflection.Emit;
using Unity.VisualScripting;
using UnityEngine;

public enum controlType
{
    move,
    rotate,
    select,
    attack,
    nullType
}

public interface IFormation
{
    void selectedFormationUpdate(controlType type);
    void engageOrders();
}

public class CompanyController : MonoBehaviour, IFormation
{
    [SerializeField]
    GameObject SoldierToUse;
    ArrayList soldiers = new ArrayList();

    ArrayList positionsToGoTo = new ArrayList();

    [SerializeField]
    int numSoldiers = 10;

    public enum formations
    {
        line,
        column,
        square,
        loose
    }

    public formations currentFormation = formations.line;

    [Header("line formation")]
    //variables for vertical and horizontal spacing
    public float verticalPaddingLine = 1.0f;
    public float horizontalPaddingLine = 1.0f;
    public int rows;

    [SerializeField]
    public int columns;
    Vector3 mouseDownPos;

    [SerializeField]
    GameObject RedCircle;
    GameObject circleContainer;

    ArrayList orders = new ArrayList();

    [SerializeField]
    bool engagingOrders = false;
    [SerializeField]
    bool soldiersCarryingOutOrder = false;
    [SerializeField]
    int orderIndex = -1;
    controlType lastControlType;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < numSoldiers; i++)
        {
            GameObject soldierR = Instantiate(SoldierToUse, gameObject.transform);
            soldiers.Add(soldierR);
        }

        if (currentFormation == formations.line)
        {
            //get the width and height of the soldier gameobject
            setSoldiersInLine();
        }
    }

    void Update(){

        if(engagingOrders){

            if(!soldiersCarryingOutOrder){
                orderIndex++;
                if((controlType)((object[])orders[orderIndex])[0] == controlType.rotate){
                    //Todo make it rotate using the AI path
                    

                }else if((controlType)((object[])orders[orderIndex])[0] == controlType.move){
                    //TODO make the soldiers move by moving the company
                }

            }

        }

    }


    public void selectedFormationUpdate(controlType type)
    {
        if (Input.GetMouseButtonDown(0))
        {
            mouseDownPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        else if (Input.GetMouseButton(0))
        {
            if (type == controlType.move)
            {

                lastControlType = controlType.move;

                //get the current mouse position to world position
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                //Get the number of columns that can be fit between the mouse down position and the current mouse position
                columns = (int)
                    Math.Ceiling(
                        (double)(mousePos.x - mouseDownPos.x)
                            / (double)(
                                SoldierToUse.GetComponent<Renderer>().bounds.size.x
                                + horizontalPaddingLine
                            )
                    );
                setPositionsToGoToLine(mouseDownPos);

                //Destroy the old red circles
                Destroy(circleContainer);

                GameObject circleContainerR = new GameObject("Circle container");
                circleContainer = circleContainerR;

                foreach (Vector3 pos in positionsToGoTo)
                {
                    Instantiate(RedCircle, pos, Quaternion.identity, circleContainer.transform);
                }
                //Get the dir vector from the center of the formation to the mouse pos
                float soldierWidth = SoldierToUse.GetComponent<Renderer>().bounds.size.x;
                float soldierHeight = SoldierToUse.GetComponent<Renderer>().bounds.size.y;

                float horizontalDistancing = soldierWidth + horizontalPaddingLine;
                float verticalDistancing = soldierHeight + verticalPaddingLine;

                Vector2 center = new Vector2(
                    (mouseDownPos.x + (horizontalDistancing * columns) / 2),
                    mouseDownPos.y + (verticalDistancing * rows) / 2
                );
                //Adjust the pivot of the circle container to center
                //Save the positions of all the red circles in the circleContainer to an arraylist
                ArrayList redCircles = new ArrayList();
                foreach (Transform child in circleContainer.transform)
                {
                    redCircles.Add(child.transform.position);
                }

                circleContainer.transform.position = center;

                //Set the positions of all the red circles back to their original positions
                for (int i = 0; i < redCircles.Count; i++)
                {
                    circleContainer.transform.GetChild(i).transform.position = (Vector3)
                        redCircles[i];
                }
                //IF there is an error where they are taking pointers rather than data, probably from here
                bool setFlag = false;
                foreach(object[] order in orders)
                {
                    if((controlType)((object[])order)[0] == controlType.move)
                    {
                        ((object[])order)[1] = positionsToGoTo.Clone();
                        setFlag = true;
                    }
                }
                if(!setFlag){
                    orders.Add(new object[] {controlType.move, positionsToGoTo.Clone()});
                }
            }
            else if (type == controlType.rotate)
            {
                lastControlType = controlType.rotate;
                //Get the circle container gameobject
                if (circleContainer == null)
                {
                    setPositionsToGoToLine(gameObject.transform.position);

                    //Destroy the old red circles

                    Destroy(circleContainer);

                    circleContainer = new GameObject("Circle container");
                    foreach (Vector3 pos in positionsToGoTo)
                    {
                        Instantiate(RedCircle, pos, Quaternion.identity, circleContainer.transform);
                    }
                    //Get the dir vector from the center of the formation to the mouse pos
                    float soldierWidthR = SoldierToUse.GetComponent<Renderer>().bounds.size.x;
                    float soldierHeightR = SoldierToUse.GetComponent<Renderer>().bounds.size.y;

                    float horizontalDistancingR = soldierWidthR + horizontalPaddingLine;
                    float verticalDistancingR = soldierHeightR + verticalPaddingLine;

                    Vector2 centerR = new Vector2(
                        (
                            gameObject.transform.position.x
                            + (horizontalDistancingR * (columns - 1f)) / 2
                        ),
                        gameObject.transform.position.y + (verticalDistancingR * (rows - 1f)) / 2
                    );
                    //Adjust the pivot of the circle container to center
                    //Save the positions of all the red circles in the circleContainer to an arraylist
                    ArrayList redCircles = new ArrayList();

                    foreach (Transform child in circleContainer.transform)
                    {
                        redCircles.Add(child.transform.position);
                    }

                    circleContainer.transform.position = centerR;

                    //Set the positions of all the red circles back to their original positions
                    for (int i = 0; i < redCircles.Count; i++)
                    {
                        circleContainer.transform.GetChild(i).transform.position = (Vector3)
                            redCircles[i];
                    }

                }

                //Get the current mouse position to world pos
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                Vector2 center = circleContainer.transform.position;

                float angle = getAngleBetweenPoints(center, mousePos);
                //Minus 90 as the sprite is facing down

                circleContainer.transform.rotation = Quaternion.Euler(0f, 0f, angle + 90f);

                bool setFlag = false;
                foreach(object[] order in orders)
                {
                    if((controlType)((object[])order)[0] == controlType.rotate)
                    {
                        ((object[])order)[1] = angle + 90f;
                        setFlag = true;
                    }
                }
                if(!setFlag){
                    orders.Add(new object[] {controlType.rotate, angle + 90f});
                }

            }

        }
    }

    public void engageOrders()
    {
        engagingOrders = true;
    }

    void setSoldiersInLine()
    {
        float soldierWidth = SoldierToUse.GetComponent<Renderer>().bounds.size.x;
        float soldierHeight = SoldierToUse.GetComponent<Renderer>().bounds.size.y;

        float horizontalDistancing = soldierWidth + horizontalPaddingLine;
        float verticalDistancing = soldierHeight + verticalPaddingLine;

        rows = (int)Math.Ceiling((double)numSoldiers / columns);
        int soldiersLeft = soldiers.Count;

        float heightOffset = 0f;

        for (int y = 0; y < rows; y++)
        {
            if (soldiersLeft >= columns)
            {
                float widthOffset = 0f;
                for (int x = 0; x < columns; x++)
                {
                    GameObject soldier = (GameObject)soldiers[soldiers.Count - soldiersLeft];
                    soldier.transform.position = new Vector3(
                        transform.position.x + widthOffset,
                        transform.position.y + heightOffset,
                        0f
                    );
                    widthOffset += horizontalDistancing;
                    soldiersLeft--;
                }
                heightOffset += verticalDistancing;
            }
            else
            {
                GameObject soldier = (GameObject)soldiers[soldiers.Count - soldiersLeft];
                float offsetCalculator = soldiersLeft % 2 == 1 ? columns - 1 : columns;
                soldier.transform.position = new Vector3(
                    (transform.position.x + (horizontalDistancing * (offsetCalculator)) / 2),
                    transform.position.y + heightOffset,
                    0f
                );

                float widthOffset = horizontalDistancing;
                for (int x = soldiersLeft - 1; x > 0; x -= 2)
                {
                    soldier = (GameObject)soldiers[soldiers.Count - x];
                    soldier.transform.position = new Vector3(
                        (transform.position.x + (horizontalDistancing * (offsetCalculator)) / 2)
                            - widthOffset,
                        transform.position.y + heightOffset,
                        0f
                    );
                    if (x > 1)
                    {
                        soldier = (GameObject)soldiers[soldiers.Count - x + 1];
                        soldier.transform.position = new Vector3(
                            (transform.position.x + (horizontalDistancing * (offsetCalculator)) / 2)
                                + widthOffset,
                            transform.position.y + heightOffset,
                            0f
                        );
                    }

                    widthOffset += horizontalDistancing;
                }
            }
        }
    }

    void setPositionsToGoToLine(Vector3 offsetPosition)
    {
        positionsToGoTo.Clear();

        float soldierWidth = SoldierToUse.GetComponent<Renderer>().bounds.size.x;
        float soldierHeight = SoldierToUse.GetComponent<Renderer>().bounds.size.y;

        float horizontalDistancing = soldierWidth + horizontalPaddingLine;
        float verticalDistancing = soldierHeight + verticalPaddingLine;

        rows = (int)Math.Ceiling((double)numSoldiers / columns);
        int soldiersLeft = soldiers.Count;

        float heightOffset = 0f;
        //get mouse pos in game world


        for (int y = 0; y < rows; y++)
        {
            if (soldiersLeft >= columns)
            {
                float widthOffset = 0f;
                for (int x = 0; x < columns; x++)
                {
                    GameObject soldier = (GameObject)soldiers[soldiers.Count - soldiersLeft];
                    positionsToGoTo.Add(
                        new Vector3(
                            offsetPosition.x + widthOffset,
                            offsetPosition.y + heightOffset,
                            0f
                        )
                    );
                    widthOffset += horizontalDistancing;
                    soldiersLeft--;
                }
                heightOffset += verticalDistancing;
            }
            else
            {
                GameObject soldier = (GameObject)soldiers[soldiers.Count - soldiersLeft];
                float offsetCalculator = soldiersLeft % 2 == 1 ? columns - 1 : columns;

                //Just adding the middle soldier
                positionsToGoTo.Add(
                    new Vector3(
                        (offsetPosition.x + (horizontalDistancing * (offsetCalculator)) / 2),
                        offsetPosition.y + heightOffset,
                        0f
                    )
                );

                float widthOffset = horizontalDistancing;
                for (int x = soldiersLeft - 1; x > 0; x -= 2)
                {
                    soldier = (GameObject)soldiers[soldiers.Count - x];
                    positionsToGoTo.Add(
                        new Vector3(
                            (offsetPosition.x + (horizontalDistancing * (offsetCalculator)) / 2)
                                - widthOffset,
                            offsetPosition.y + heightOffset,
                            0f
                        )
                    );
                    if (x > 1)
                    {
                        soldier = (GameObject)soldiers[soldiers.Count - x + 1];
                        positionsToGoTo.Add(
                            new Vector3(
                                (offsetPosition.x + (horizontalDistancing * (offsetCalculator)) / 2)
                                    + widthOffset,
                                offsetPosition.y + heightOffset,
                                0f
                            )
                        );
                    }

                    widthOffset += horizontalDistancing;
                }
            }
        }
    }

    public static float getAngleBetweenPoints(Vector2 pos1, Vector2 pos2)
    {
        Vector2 dir = pos2 - pos1;
        float alpha = (float)Math.Atan(Math.Abs(dir.y) / Math.Abs(dir.x)) * (float)(180 / Math.PI);
        if (dir.x > 0)
        {
            if (dir.y > 0)
            {
                //1st quadrant
                return alpha;
            }
            else
            {
                //4th quadrant
                return 360f - alpha;
            }
        }
        else
        {
            if (dir.y > 0)
            {
                //2nd quadrant
                return 180f - alpha;
            }
            else
            {
                //3rd quadrant
                return 180f + alpha;
            }
        }
    }
}
