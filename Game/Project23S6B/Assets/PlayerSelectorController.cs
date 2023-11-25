using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSelectorController : MonoBehaviour
{
    [SerializeField]
    GameObject selectedFormation;
    [SerializeField]
    controlType type = controlType.move;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //call the selected formation update
        selectedFormation.GetComponent<IFormation>().selectedFormationUpdate(type);

        //Switch control type to rotate if r is pressed
        if(Input.GetKeyDown(KeyCode.R)){
            type = controlType.rotate;
        }
        //Switch control type to move if m is pressed
        if(Input.GetKeyDown(KeyCode.M)){
            type = controlType.move;
        }
        //Switch control type to select if s is pressed
        if(Input.GetKeyDown(KeyCode.S)){
            type = controlType.select;
        }
        //Switch control type to attack if a is pressed
        if(Input.GetKeyDown(KeyCode.A)){
            type = controlType.attack;
        }

        //If the enter key is pressed, call the engage orders function of the company controller of the selected formation
        if(Input.GetKeyDown(KeyCode.Return)){
            selectedFormation.GetComponent<CompanyController>().engageOrders();
        }
        
    }
}
