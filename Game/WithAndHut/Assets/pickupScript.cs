using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pickupScript : MonoBehaviour
{
    GameObject player;
    public string type;
    //Both of the values below are inclusive
    public int minCount;
    public int maxCount;
    public int count;
    //How do you wan tto determine the number of items to pickup?
    //Do you want to display a different sprite for a different amount of items like minecraft?


    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public void setMinMax(int min, int max)
    {
        minCount = min;
        maxCount = max;
    }

    private void OnTriggerEnter(Collider other)
    {
        print("Enter");
        print(other.gameObject.name);
        if (other.gameObject == player)
        {
            count = Random.Range(minCount, maxCount + 1);
            //GetPickedUp
            player.GetComponent<inventory>().AddItem(type, count);
            Destroy(gameObject);
        }
    }
}
