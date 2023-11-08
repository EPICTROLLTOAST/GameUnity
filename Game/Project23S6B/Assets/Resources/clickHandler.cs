using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using TMPro;

public class clickHandler : EventTrigger
{
    public string jump;
    public string function;
    public bool terminate;
    public dialogue dialogueScript;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        gameObject.GetComponentInChildren<TextMeshProUGUI>().color = Color.red;
        
    }
    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        gameObject.GetComponentInChildren<TextMeshProUGUI>().color = Color.black;
    }
    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        dialogueScript.UpdateButtonDown(jump,function,terminate);
    }

    public void updateButton(string text, string jumpR, string functionR, bool terminateR, dialogue dialogueScriptR)
    {
        gameObject.GetComponentInChildren<TextMeshProUGUI>().text = text;
        jump = jumpR;
        function = functionR;
        terminate = terminateR;
        dialogueScript = dialogueScriptR;
    }

}
