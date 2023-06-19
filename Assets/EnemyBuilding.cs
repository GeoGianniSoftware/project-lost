using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBuilding : Entity
{

    //Temp prefab, change to resource.load
    public GameObject enemyPrefab;
    public float spawnInterval;
    float spawnTime;
    [Min(8)]
    public float spawnRange;
    public Transform rallyPoint;

    // Start is called before the first frame update
    void Start()
    {
        rallyPoint = transform.GetChild(0);
    }

    // Update is called once per frame
    void Update()
    {
        
        if(spawnTime <= 0) {
            GameObject enemySpawned = Instantiate(enemyPrefab, getSpawnPos() , Quaternion.identity);
            enemySpawned.GetComponent<BasicEnemyAI>().setRallyPoint(rallyPoint.position);
            spawnTime = spawnInterval;
        }
        else {
            spawnTime -= Time.deltaTime;
        }
    }

    Vector3 getSpawnPos() {
        float randX = 0, randZ = 0;

        randX = Random.Range(8f, spawnRange);
        randZ = Random.Range(8f, spawnRange);
        int negRandX = 1;
        int negRandZ = 1;

        int negTest = Random.Range(0, 4);
        switch (negTest) {
            default:
                negRandX = 1;
                negRandZ = 1;
                break;
            case 1:
                negRandX = -1;
                negRandZ = 1;
                break;
            case 2:
                negRandX = 1;
                negRandZ = -1;
                break;
            case 3:
                negRandX = -1;
                negRandZ = -1;
                break;
        }

        Vector3 spawnPos = transform.position + new Vector3(randX * negRandX, 0, randZ * negRandZ);
        return spawnPos;
    }
}
