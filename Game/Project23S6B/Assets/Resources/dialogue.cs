using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml;
using UnityEngine.TextCore;
using TMPro;
using System;

public class dialogue : MonoBehaviour
{
    public GameObject textObject;
    TextMeshProUGUI text;
    public string curentText = "test";
    string path = "Assets/Resources/dialogue/";
    XmlDocument doc;
    public int lineNumber;
    public ArrayList dialogueLines = new ArrayList(){};
    bool displayed = false;
    string section;
    Dictionary<string, string> people = new Dictionary<string, string>{ };
    bool haveDialogue = false;

    XmlNodeList options;
    [SerializeField]
    GameObject buttons2;
    [SerializeField]
    GameObject buttons3;
    [SerializeField]
    GameObject buttons4;
    enum textType
    {
        None,
        Options,
        Dialog,
        function
    }

    float printSpeed = 150;
    string imageChar = "";
    int charNumber = 0;
    int printed = 0;
    float timeSinceLastPrint = 999f;
    XmlNode dLog;
    textType currentType = textType.None;
    bool terminateNext = false;
    bool lockBecauseOptions = false;

    // Start is called before the first frame update
    void Start()
    {
        section = "declarations";
        people.Add("n", "narrator");
        
        text = textObject.GetComponent<TextMeshProUGUI>();
        print(text.transform.parent.name);
        print("starting dialogue");
        startDialogue(curentText);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !lockBecauseOptions)
        {
            lineNumber += 1;
            nextLine();
            
        }
        if(currentType == textType.Dialog)
        {
            if (timeSinceLastPrint >= 999f && printed < charNumber)
            {
                text.text += dLog.InnerText.Substring(printed, 1);
                if (dLog.InnerText.Substring(printed, 1) == " ")
                {
                    printed += 1;
                    text.text += dLog.InnerText.Substring(printed, 1);
                }
                timeSinceLastPrint = 0f;
                printed += 1;
            }
            if (timeSinceLastPrint >= 1 / printSpeed && printed < charNumber)
            {
                text.text += dLog.InnerText.Substring(printed, 1);
                if (dLog.InnerText.Substring(printed, 1) == " ")
                {
                    printed += 1;
                    text.text += dLog.InnerText.Substring(printed, 1);
                }
                timeSinceLastPrint = 0f;
                printed += 1;
            }
            timeSinceLastPrint += Time.deltaTime;
        }
        
    }

    void nextLine()
    {
        
        text.text = ""; 
        dLog = (XmlNode)dialogueLines[lineNumber - 1];
        string type = dLog.Attributes.GetNamedItem("type").InnerText;
        charNumber = dLog.InnerText.Length;
        printed = 0;
        buttons2.SetActive(false);
        buttons3.SetActive(false);
        buttons4.SetActive(false);

        if (terminateNext)
        {
            terminateNext = false;
            EndDialogue();
        }
        

        try
        {
            printSpeed = (float) Int32.Parse(dLog.Attributes.GetNamedItem("speed").InnerText);
        }
        catch (System.Exception)
        {
            printSpeed = 10f;
            
        }
        

        if(type == "d")
        {
            lockBecauseOptions = false;
            string characterName = people[dLog.Attributes.GetNamedItem("chr").InnerText];
            text.text = "<b>" + characterName + "</b>" + "\n";
            currentType = textType.Dialog;

        }else if(type == "o")
        {
            lockBecauseOptions = true;
            currentType = textType.Options;
            options = dLog.ChildNodes;
            switch (options.Count)
            {
                case 2:
                    buttons2.SetActive(true);
                    setButtonValues(options.Count, buttons2);
                    break;
                case 3:
                    buttons3.SetActive(true);
                    setButtonValues(options.Count, buttons3);
                    break;
                case 4:
                    buttons4.SetActive(true);
                    setButtonValues(options.Count, buttons4);
                    break;

            }
        }else if(type == "t")
        {
            EndDialogue();
        }
    }

    void setButtonValues(int count, GameObject buttonGroup)
    {
        foreach(RectTransform toActivate in buttonGroup.GetComponentsInChildren<RectTransform>())
        {
            toActivate.gameObject.SetActive(true);
            foreach (RectTransform activating in toActivate.gameObject.GetComponentsInChildren<RectTransform>())
            {
                activating.gameObject.SetActive(true);
            }
            
        }
        for(int x = 0; x < count; x++)
        {
            RectTransform[] buttonAlter = buttonGroup.GetComponentsInChildren<RectTransform>();
            foreach (RectTransform rt in buttonAlter)
            {
                if(rt.gameObject.name == (x + 1).ToString())
                {
                    clickHandler cH = rt.gameObject.GetComponent<clickHandler>();
                    string jumpS, functionS;
                    bool terminateS;

                    try
                    {
                        jumpS = options[x].Attributes.GetNamedItem("jump").InnerText == null ? "" :
                        options[x].Attributes.GetNamedItem("jump").InnerText;
                    }
                    catch
                    {
                        jumpS = "";
                    }

                    try
                    {
                        functionS = options[x].Attributes.GetNamedItem("function").InnerText == null ? "" :
                        options[x].Attributes.GetNamedItem("function").InnerText;
                    }
                    catch
                    {
                        functionS = "";
                    }

                    try
                    {
                        terminateS = (options[x].Attributes.GetNamedItem("terminate").InnerText == null ? "" :
                        options[x].Attributes.GetNamedItem("terminate").InnerText) == "true" ? true : false;
                    }
                    catch
                    {
                        terminateS = false;
                    }

                    
                    
                        
                    cH.updateButton(options[x].InnerText, jumpS, functionS, terminateS, this);

                    rt.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = options[x].InnerText;
                }
            }
        }
    }


    void startDialogue(string text)
    {
        haveDialogue = true;
        lineNumber = 1;
        Resources.Load(path + text);
        doc = new XmlDocument();
        doc.Load(path + text + ".xml");
        XmlNodeList characters =  doc.DocumentElement.SelectNodes("//character");
        foreach(XmlNode node in characters)
        {
            string name = "";
            string denotion = "";
            XmlNodeList nameDenotion = node.ChildNodes;
            foreach(XmlNode thing in nameDenotion)
            {
                if(thing.Name == "name")
                {
                    name = thing.InnerText;
                }
                else if(thing.Name == "denotion")
                {
                    denotion = thing.InnerText;
                }
            }
            people.TryAdd(denotion,name);
        }

        XmlNodeList dLines = doc.DocumentElement.SelectNodes("//t");
        foreach (XmlNode line in dLines)
        {
            dialogueLines.Add(line);
        }
        nextLine();
    }
    
    public void EndDialogue()
    {
        dialogueLines = new ArrayList() { };
        people = new Dictionary<string, string>(){};
        people.Add("n", "Narrator");
        currentType = textType.None;
        buttons2.SetActive(false);
        buttons3.SetActive(false);
        buttons4.SetActive(false);
    }



    public void UpdateButtonDown(string jumpR, string functionR, bool terminateR)
    {
        lockBecauseOptions = false;
        if(functionR != "")
        {
            
            this.GetType().GetMethod(functionR).Invoke(this,new object[] {});
        }
        if (terminateR == true)
        {
            terminateNext = true;
            return;
        }
        if(jumpR != "")
        {
            bool jumpFound = false;
            int countLines = 0;
            foreach(XmlNode node in dialogueLines)
            {
                try
                {
                    if (node.Attributes.GetNamedItem("jmp").InnerText == jumpR)
                    {
                        jumpFound = true;
                        break;
                    }
                    else
                    {
                        countLines++;
                    }
                }
                catch
                {
                    countLines++;
                }
                
            }
            if (jumpFound)
            {
                lineNumber = countLines + 1;
            }
        }
        nextLine();
    }

    public static void printFunctionWorks()
    {
        print("1111");
    }
}
