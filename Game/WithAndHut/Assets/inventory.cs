using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEditor.Progress;

public class inventory : MonoBehaviour
{
    //The following code is extremely unoptimised, but since its only being called a few times and that 
    //too to parse through lists < 100, it should be fine


    //Inventoru is structured as follows: playerIncventory{{item,count},{item,count},{item,count}}
    public ArrayList playerInventory = new ArrayList(){};
    // Start is called before the first frame update
    
    void Start()
    {
        //Load inventory if have
    }

    public void AddItem(string item, int count)
    {
        if(count <= 0)
        {
            print("Count cannot be negative");
            return;
        }

        if(FindItem(item) == -1)
        {
            playerInventory.Add(new ArrayList() { item, count });
        }
        else
        {
            //Does anyone know how to get it to ignore the fucking type errors? 
            //Without all this stupid typecasting
            ((ArrayList)playerInventory[FindItem(item)])[1] = (int)((ArrayList)playerInventory[FindItem(item)])[1] + count;
        }
        print(((ArrayList)playerInventory[0])[1]);
    }

    public void RemoveItem(string item, int count)
    {
        if (count <= 0)
        {
            print("Count cannot be negative");
            return;
        }

        if (FindItem(item) == -1)
        {
            print("Item not found");
        }
        else
        {
            //Does anyone know how to get it to ignore the fucking type errors? 
            //Without all this stupid typecasting
            if ((int)((ArrayList)playerInventory[FindItem(item)])[1] < count)
            {
                print("You're taking more than actually is there!");
                return;
            }
            else if ((int)((ArrayList)playerInventory[FindItem(item)])[1] == count)
            {
                //removing key
                playerInventory.RemoveAt(FindItem(item));
            }
            else
            {
                ((ArrayList)playerInventory[FindItem(item)])[1] = (int)((ArrayList)playerInventory[FindItem(item)])[1] - count;
            }
            
        }
    }

    //returns -1 if there is no item of the sort stored
    public int GetItemCount(string itemCheck)
    {
        for (int x = 0; x < playerInventory.Count; x++)
        {
            if (((ArrayList)(playerInventory[x]))[0] == itemCheck)
            {
                return (int)((ArrayList)playerInventory[x])[1];
            }
        }
        return -1;
    }

    //returns -1 if there is no item of the sort stored
    public int FindItem(string itemCheck)
    {
        for (int x = 0; x < playerInventory.Count; x++)
        {
            if (((ArrayList)(playerInventory[x]))[0] == itemCheck)
            {
                return x;
            }
        }
        return -1;
    }

    
    //Save when the scenes change or smthg

}
