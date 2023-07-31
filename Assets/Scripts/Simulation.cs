using System.Collections;
using System.Collections.Generic;
using System.IO;
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
    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 200;
        //Spawn x hens at random positions
        for (int i = 0; i < startHenNumber; i++)
        {
            GameObject newHen = Instantiate(Hen, new Vector3(Random.Range(30f, 50f), 1, Random.Range(30f, 50f)), Quaternion.Euler(0, 0, 0), HenContainer.transform);
            newHen.GetComponent<Animal>().SetAnimalContainer(HenContainer);
            newHen.GetComponent<Animal>().SetDeathManager(deathManager);
        }

        //Spawn x foxes at random positions
        for (int i = 0; i < startFoxNumber; i++)
        {
            GameObject newFox = Instantiate(Fox, new Vector3(Random.Range(30f, 50f), 1, Random.Range(30f, 50f)), Quaternion.Euler(0, 0, 0), FoxContainer.transform);
            newFox.GetComponent<Animal>().SetAnimalContainer(FoxContainer);
            newFox.GetComponent<Animal>().SetDeathManager(deathManager);
        }

        InvokeRepeating(nameof(CalculateAverageSpeed), 0f, 1f);
        //InvokeRepeating(nameof(SaveData), 0f, 1f);
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

    void SaveData()
    {
        using (StreamWriter writerH = new("Assets/Data/hens.txt", true))
        {
            writerH.WriteLine(GameObject.FindGameObjectsWithTag("Hen").Length.ToString());
        }

        using (StreamWriter writerF = new("Assets/Data/foxes.txt", true))
        {
            writerF.WriteLine(GameObject.FindGameObjectsWithTag("Fox").Length.ToString());
        }
    }
    
}
