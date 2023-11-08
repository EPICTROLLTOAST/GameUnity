using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueSystem : MonoBehaviour
{
    TextMeshPro text;
    GameObject LeftImage;
    GameObject RightImage;
    


    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform child in this.transform)
        {
            if(child.gameObject.name == "Text"){
                text = child.gameObject.GetComponent<TextMeshPro>();
            }else if(child.gameObject.name == "LeftImage"){
                LeftImage = child.gameObject;
            }else if(child.gameObject.name == "RightImage"){
                RightImage = child.gameObject;
            }
        }  


    }

    // Update is called once per frame
    void Update()
    {
        
    }





}
