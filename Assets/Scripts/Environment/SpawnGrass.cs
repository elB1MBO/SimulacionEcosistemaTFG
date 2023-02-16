using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnGrass : MonoBehaviour
{
    public GameObject grass;
    public GameObject ground;
    public float spawnTime;
    public float spawnDelay;
    public bool stopSpawning = false;

    public Vector3 groundSize;

    [SerializeField]
    private float foodAmount;

    void Start()
    {
        foodAmount = 0;
        InvokeRepeating("SpawnObject", spawnTime, spawnDelay);
    }
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyUp(KeyCode.P)) 
        { 
            stopSpawning = !stopSpawning;
        }
    }

    public void SpawnObject()
    {
        if (foodAmount < 10)
        {
            //Queremos que la posiciçon sea aleatoria en el terreno posible:
            Vector3 spawnPos = new Vector3(Random.Range(-groundSize.x / 2, groundSize.x / 2), 0, Random.Range(-groundSize.z / 2, groundSize.z / 2));
            GameObject newGrass = Instantiate(grass, spawnPos, transform.rotation);
            newGrass.gameObject.tag = "Food";
            foodAmount++;

            if (stopSpawning)
            {
                CancelInvoke("SpawnObject");
            }
        }
        else
        {
            stopSpawning = true;
        }
    }

}
