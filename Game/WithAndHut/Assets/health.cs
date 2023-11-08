using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class health : MonoBehaviour
{
    public bool died = false;

    // Update is called once per frame
    public float maxHP = 10f;
    public float currentHP;
    public GameObject[] objectsToDrop;
    public int[] maxAndMinForObjectsRespectively;
    private ArrayList maxMin = new ArrayList();

    public void Start()
    {
        currentHP = maxHP;
        bool flag = false;
        for(int x = 0; x < maxAndMinForObjectsRespectively.Length; x++)
        {
            if (!flag)
            {
                flag = true;
                maxMin.Add(new int[] {maxAndMinForObjectsRespectively[x], maxAndMinForObjectsRespectively[x + 1]});
            }
            else
            {
                flag = false;
            }
        }
    }
    public void ChangeHP(float hp)
    {
        currentHP = Mathf.Min(maxHP, currentHP + hp);
        CheckDeath();
    }
    private void Update()
    {
        CheckDeath();
    }
    public void CheckDeath()
    {
        if (currentHP <= 0)
        {
            for (int x = 0; x < objectsToDrop.Length; x++)
            {
                //TODO this isnt dropping
                print("dropping");
                print(((int[])maxMin[x])[0]);
                GameObject toSpawn = objectsToDrop[x];
                GameObject spawned = Instantiate(toSpawn,gameObject.transform.position,Quaternion.identity);
                spawned.GetComponent<pickupScript>().setMinMax(((int[])maxMin[x])[0], ((int[])maxMin[x])[1]);
            }
            //Drop object
            Destroy(gameObject);
        }
    }
}
