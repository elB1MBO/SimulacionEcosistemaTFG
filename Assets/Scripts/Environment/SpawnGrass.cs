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

    public Vector3 size;

    public string objectTag = "Food";

    [SerializeField]
    private float foodAmount;

    void Start()
    {
        foodAmount = 0;
        //size = ground.transform.localScale;
        InvokeRepeating("SpawnObject", spawnTime, spawnDelay);
    }
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyUp(KeyCode.P)) 
        { 
            stopSpawning = !stopSpawning;
        }

        //Comprueba que la cantidad de comida no baja de cierto punto. Si lo hace, vuelve a empezar a spawnear
        UpdateFoodAmount();
        if (foodAmount <5) { 
            stopSpawning = false;
            InvokeRepeating("SpawnObject", spawnTime, spawnDelay);
        }
    }

    public void SpawnObject()
    {
        UpdateFoodAmount(); //Hay que llamar a este método aquí dentro tambien, ya que si no se repetirá 10 veces siempre, en lugar de x veces hasta 10.
        if (foodAmount < 10)
        {
            //Queremos que la posicion sea aleatoria en el terreno posible:
            Vector3 spawnPos = new Vector3(Random.Range(-size.x / 2, size.x / 2), 0.05f, Random.Range(-size.z / 2, size.z / 2));
            GameObject newGrass = Instantiate(grass, spawnPos, transform.rotation);
            newGrass.gameObject.tag = objectTag;

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

    //Funcion que va comprobando la cantidad de comida que hay por su tag
    private void UpdateFoodAmount()
    {
        foodAmount = GameObject.FindGameObjectsWithTag(objectTag).Length;
    }

}
