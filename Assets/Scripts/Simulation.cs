using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Simulation : MonoBehaviour
{
    public float startHenNumber;
    public GameObject Hen;
    public GameObject HenContainer;

    public float startFoxNumber;
    public GameObject Fox;
    public GameObject FoxContainer;

    public GameObject BushContainer;

    public DeathManager deathManager;

    public float averageHenSpeed = 0f;
    public float averageFoxSpeed = 0f;

    public GameObject[] waterTiles; // Los tiles son estáticos? 
    public List<GameObject> bushesList;
    public List<GameObject> hensList;
    public List<GameObject> foxesList;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 200;

        bushesList  = new List<GameObject>();
        hensList    = new List<GameObject>();
        foxesList   = new List<GameObject>();

        UpdateBushes();
        UpdateWaterTilesList();

        InvokeRepeating(nameof(CalculateAverageSpeed), 0f, 1f);
    }

    //Cada función se llamará cuando se cree o borre un objeto correspondiente a su etiqueta
    public void AddBush(GameObject bush)
    {
        bushesList.Add(bush);
    }
    public void RemoveBush(GameObject bush)
    {
        bushesList.Remove(bush);
    }
    public void AddHen(GameObject hen)
    {
        hensList.Add(hen);
    }
    public void RemoveHen(GameObject hen)
    {
        hensList.Remove(hen);
    }
    public void AddFox(GameObject fox)
    {
        foxesList.Add(fox);
    }
    public void RemoveFox(GameObject fox)
    {
        foxesList.Remove(fox);
    }

    //Para evitar aglomeraciones, se debe ir comprobando si no están a Full
    private void UpdateBushes()
    {
        bushesList = GameObject.FindGameObjectsWithTag("BushResource").ToList();
    }
    void UpdateWaterTilesList()
    {
        waterTiles = GameObject.FindGameObjectsWithTag("Water");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Time.timeScale *= 2f;
        }
        else if (Input.GetKeyDown(KeyCode.L))
        {
            Time.timeScale *= 0.5f;
        }
        else if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (Time.timeScale == 0)
            {
                Time.timeScale = 1;
            }
            else
            {
                Time.timeScale = 0;
            }
        }

        UpdateBushes();
    }

    void CalculateAverageSpeed()
    {
        float hensNum = HenContainer.transform.childCount;
        float totalSpeeds = 0f;

        foreach (Transform hen in HenContainer.transform)
        {
            totalSpeeds += hen.GetComponent<Animal>().GetSpeed();
        }

        if (hensNum > 0) { averageHenSpeed = totalSpeeds / hensNum; }

        float foxesNum = FoxContainer.transform.childCount;
        totalSpeeds = 0f;
        foreach (Transform fox in FoxContainer.transform)
        {
            totalSpeeds += fox.GetComponent<Animal>().GetSpeed();
        }
        if(foxesNum > 0) { averageFoxSpeed = totalSpeeds / foxesNum; }
    }
}
