using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;
using System.Runtime.InteropServices.WindowsRuntime;

public class EnemySpawnerController : MonoBehaviour
{
    
    public enum startConditions{
        spawnOnSceneStart,
        spawnAfterSignalFromPrevious
    }
    
    public startConditions startCondition = startConditions.spawnOnSceneStart;
    public bool hasNext = false;
    //if has next
    public GameObject nextToContact;
    bool hasContactedNext = false;
    public enum contactNextConditions{
        immediate,
        afterSetTime,
        afterPercentageOfSpawnedKilled,
        doneSpawning
    }
    public contactNextConditions conditionToContactNext = contactNextConditions.immediate;
    //For after set time
    public float timeToContactNext = 1f;
    //For afterPercentageOfSpawnedKilled
    public float percentageKilled = 100f;

    int enemiesSpawned;

    public enum spawnerControllerTypes{
        spawnAfterTimeIntervalForCertainAmountOfTimes,
        scriptedSpawn,
        weightedRandomSpawn
    }
    
    public spawnerControllerTypes spawnType = spawnerControllerTypes.spawnAfterTimeIntervalForCertainAmountOfTimes;
    
    //For spawning after certain interval of time
    public int roundsToSpawn;
    public int numberToSpawn; 
    public GameObject toSpawn;
    public float timeInterval;
    //For scripted spawn
     
    //For rand
    public float spawnCredits = 10f;
    [SerializeField]
    internal Matrix toSpawnMatrix = new Matrix();

    [SerializeField]
    internal object[] toSpawnList = new object[]{new object[]{null, 1f}};
    public int amountOfEnemies;
    bool active = false;
    int roundNumber = 0;
    float timeSinceLastSpawn = 0f;
    float timeActive = 0f;
    bool spawning = true;

    // Start is called before the first frame update
    void Start()
    {
        if(startCondition == startConditions.spawnOnSceneStart){
            startup();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(active){
            timeSinceLastSpawn += Time.deltaTime;
            timeActive += Time.deltaTime;
            

            //Do anything that needs to be done
            switch (spawnType){
                case spawnerControllerTypes.spawnAfterTimeIntervalForCertainAmountOfTimes:
                    spawnAfterTimeUpdate();
                    break;
                case spawnerControllerTypes.scriptedSpawn:
                    scriptedSpawn();
                    break;
                case spawnerControllerTypes.weightedRandomSpawn:
                    weightedRandomSpawn();
                    break;
                
            }

            if(!hasContactedNext && hasNext){
                hasContactedNext = true;
                switch(conditionToContactNext){
                    case contactNextConditions.immediate:
                        contactNext();
                        break;
                    case contactNextConditions.afterSetTime:
                        if(timeActive >= timeToContactNext){
                            contactNext();
                        }else{
                            hasContactedNext = false;
                        }
                        break;
                    case contactNextConditions.afterPercentageOfSpawnedKilled:
                        if(spawnType == spawnerControllerTypes.spawnAfterTimeIntervalForCertainAmountOfTimes){
                            int amountOfEnemies = transform.childCount;
                            int numberToSpawn = 0;
                            for(int i = 0; i < toSpawnList.Length; i++){
                                numberToSpawn += (int)System.Math.Floor((float)(((object[])toSpawnList[i])[1]));
                                print(numberToSpawn);
                            }
                            float percentageKill = 100 - (amountOfEnemies + numberToSpawn * (roundsToSpawn - roundNumber))/(numberToSpawn * roundsToSpawn)  * 100;
                            

                            if(percentageKill >= percentageKilled){
                                contactNext();
                            }else{
                                hasContactedNext = false;
                            }

                        }else if(spawnType == spawnerControllerTypes.weightedRandomSpawn){

                            if(roundNumber == 1){
                                //already spawned everything
                                int amountOfEnemies = transform.childCount;
                                float percentageKill = 100 - amountOfEnemies/(enemiesSpawned) * 100;
                                if(percentageKill >= percentageKilled){
                                    contactNext();
                                }else{
                                    hasContactedNext = false;
                                }
                            }
                            
                        }         


                        break;
                    case contactNextConditions.doneSpawning:
                        if(!spawning){
                            contactNext();
                        }else{
                            hasContactedNext = false;
                        }

                        break;
                    default:
                        hasContactedNext = false;
                        break;
                }
            }

        }
    }

    public void activate(){
        print("Allo!");
        startup();
    }

    void startup(){
        active = true;
    }

    void spawnAfterTimeUpdate()
    {
        if ((roundNumber < roundsToSpawn && timeSinceLastSpawn >= timeInterval)||(roundNumber == 0))
        {
            roundNumber++;
            timeSinceLastSpawn = 0f;
            foreach (object[] objects in toSpawnList)
            {
                float spawnCount = (float)objects[1]; // Cast the object to float
                for (int i = 0; i < spawnCount; i++)
                {
                    spawnEnemy((GameObject)objects[0]);
                }
            }
        }
    }

    void weightedRandomSpawn(){
        if(roundNumber == 0){
            float[] costs = new float[toSpawnList.Length];
            int i = 0;
            foreach(object[] obj in toSpawnList){
                costs[i] = (float)obj[1];
                i++;
            }

            while(spawnCredits > 0 && Mathf.Max(costs) <= spawnCredits){
                int randomIndex = Random.Range(0, toSpawnList.Length);
                if(spawnCredits >= (float)((object[])toSpawnList[randomIndex])[1]){
                    spawnCredits -= (float)((object[])toSpawnList[randomIndex])[1];
                    spawnEnemy((GameObject)((object[])toSpawnList[randomIndex])[0]);
                    enemiesSpawned++;
                }
            }

            spawning = false;
        }
    }

    public virtual void scriptedSpawn(){
        
    }


    void contactNext(){
        print("hi!");
        nextToContact.GetComponent<EnemySpawnerController>().activate();
    }

    void spawnEnemy(GameObject enemyToSpawn){

        toSpawnList = toSpawnMatrix.getArray();
        GameObject.Instantiate(enemyToSpawn, transform.position, Quaternion.identity, transform);
    }

    

}

[System.Serializable]
public class Matrix
{

    public float[] floatArray;
    public GameObject[] gameObjectArray;
    public object[] getArray(){
        if(this.floatArray == null){
            setArray(new object[]{new object[]{null, 1f}});
        }
        object[] returnArray = new object[floatArray.Length];
        for(int i = 0; i < floatArray.Length;i ++){
            returnArray[i] = new object[]{gameObjectArray[i], floatArray[i]};
        }

        return returnArray;
    }

    public void setArray(object[] array)
    {
        this.floatArray = new float[array.Length];
        this.gameObjectArray = new GameObject[array.Length];
        int i = 0;
        foreach (object[] obj in array)
        {
            this.floatArray[i] = (float)obj[1];
            this.gameObjectArray[i] = (GameObject)obj[0];
            i++;
        }
    }
}

[CustomEditor(typeof(EnemySpawnerController))]
public class EnemySpawnerControllerEditor : Editor{
    public override void OnInspectorGUI()
    {
        var myScript = target as EnemySpawnerController;

        myScript.toSpawnList = myScript.toSpawnMatrix.getArray();

        myScript.startCondition = 
        (EnemySpawnerController.startConditions)
        EditorGUILayout.EnumPopup("startCondition", myScript.startCondition);

        myScript.hasNext = EditorGUILayout.Toggle("Has to contct next", myScript.hasNext);

        if(myScript.hasNext){
            myScript.nextToContact = 
            (GameObject)EditorGUILayout.ObjectField("Next to contact", myScript.nextToContact, typeof(GameObject), true);
            if(myScript.nextToContact != null){
                myScript.conditionToContactNext = 
                (EnemySpawnerController.contactNextConditions)
                EditorGUILayout.EnumPopup("Condition to contact next", myScript.conditionToContactNext);

                if(myScript.conditionToContactNext == EnemySpawnerController.contactNextConditions.afterSetTime){

                    myScript.timeToContactNext = EditorGUILayout.FloatField("Time to contact next", myScript.timeToContactNext);

                }else if(myScript.conditionToContactNext == EnemySpawnerController.contactNextConditions.afterPercentageOfSpawnedKilled){

                    myScript.percentageKilled = EditorGUILayout.FloatField("Percentage killed", myScript.percentageKilled);

                }
            }
        }
        
        EditorGUILayout.Space();
        myScript.spawnType = 
        (EnemySpawnerController.spawnerControllerTypes)
        EditorGUILayout.EnumPopup("Spawn type", myScript.spawnType);
        
        if(myScript.spawnType == EnemySpawnerController.spawnerControllerTypes.scriptedSpawn){
            


        }else if(myScript.spawnType == EnemySpawnerController.spawnerControllerTypes.spawnAfterTimeIntervalForCertainAmountOfTimes){

            myScript.roundsToSpawn = EditorGUILayout.IntField("Rounds to spawn", myScript.roundsToSpawn);
            myScript.timeInterval = EditorGUILayout.FloatField("Time interval", myScript.timeInterval);

            myScript.amountOfEnemies = myScript.toSpawnList.Length;
            int amountOfEnemiesClone = EditorGUILayout.IntField("Amount of enemies", myScript.amountOfEnemies);

            if (amountOfEnemiesClone > myScript.amountOfEnemies){

                object[] list = new object[amountOfEnemiesClone];
                for (int i = 0; i < myScript.toSpawnList.Length; i++){
                    list[i] = myScript.toSpawnList[i];
                }
                
                for (int i = 0; i < amountOfEnemiesClone - myScript.toSpawnList.Length; i++){
                    list[i + myScript.toSpawnList.Length] = new object[]{null, 1f}; 
                }

                myScript.toSpawnList = list;
                myScript.amountOfEnemies = amountOfEnemiesClone;

            }else if(myScript.amountOfEnemies > amountOfEnemiesClone){
                myScript.toSpawnList = myScript.toSpawnList[0..amountOfEnemiesClone];
                myScript.amountOfEnemies = amountOfEnemiesClone;
            }

            for (int i = 0; i < myScript.toSpawnList.Length; i++){
                EditorGUILayout.BeginHorizontal();
                ((object[])myScript.toSpawnList[i])[0] = 
                EditorGUILayout.ObjectField(
                    (GameObject)((object[])myScript.toSpawnList[i])[0], typeof(GameObject), false);

                ((object[])myScript.toSpawnList[i])[1] = 
                EditorGUILayout.FloatField("Amount", (float)((object[])myScript.toSpawnList[i])[1]);
                EditorGUILayout.EndHorizontal();
            }

        }else if (myScript.spawnType == EnemySpawnerController.spawnerControllerTypes.weightedRandomSpawn){
            
            myScript.spawnCredits = EditorGUILayout.FloatField("Spawn credits", myScript.spawnCredits);

            myScript.amountOfEnemies = myScript.toSpawnList.Length;
            int amountOfEnemiesClone = EditorGUILayout.IntField("Amount of enemies", myScript.amountOfEnemies);

            if (amountOfEnemiesClone > myScript.amountOfEnemies){

                object[] list = new object[amountOfEnemiesClone];
                for (int i = 0; i < myScript.toSpawnList.Length; i++){
                    list[i] = myScript.toSpawnList[i];
                }
                
                for (int i = 0; i < amountOfEnemiesClone - myScript.toSpawnList.Length; i++){
                    list[i + myScript.toSpawnList.Length] = new object[]{null, 1f}; 
                }

                myScript.toSpawnList = list;
                myScript.amountOfEnemies = amountOfEnemiesClone;

            }else if(myScript.amountOfEnemies > amountOfEnemiesClone){
                myScript.toSpawnList = myScript.toSpawnList[0..amountOfEnemiesClone];
                myScript.amountOfEnemies = amountOfEnemiesClone;
            }

            for (int i = 0; i < myScript.toSpawnList.Length; i++){
                EditorGUILayout.BeginHorizontal();
                ((object[])myScript.toSpawnList[i])[0] = 
                EditorGUILayout.ObjectField(
                    (GameObject)((object[])myScript.toSpawnList[i])[0], typeof(GameObject), false);

                ((object[])myScript.toSpawnList[i])[1] = 
                EditorGUILayout.FloatField("Cost", (float)((object[])myScript.toSpawnList[i])[1]);
                EditorGUILayout.EndHorizontal();
            }

        }

        myScript.toSpawnMatrix.setArray(myScript.toSpawnList);
        
    }
}
