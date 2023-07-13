using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class Simulation : MonoBehaviour
{
    [SerializeField] public float startHenNumber;
    [SerializeField] public GameObject Hen;
    [SerializeField] public GameObject HenContainer;

    [SerializeField] public float startFoxNumber;
    [SerializeField] public GameObject Fox;
    [SerializeField] public GameObject FoxContainer;

    public float averageHenSpeed;
    // Start is called before the first frame update
    void Start()
    {
        //Spawn x hens at random positions
        for (int i = 0; i < startHenNumber; i++)
        {
            GameObject newHen = Instantiate(Hen, new Vector3(Random.Range(30f, 50f), 1, Random.Range(30f, 50f)), Quaternion.Euler(0, 0, 0), HenContainer.transform);
            newHen.GetComponent<Animal>().SetAnimalContainer(HenContainer);
        }

        //Spawn x foxes at random positions
        for (int i = 0; i < startFoxNumber; i++)
        {
            GameObject newFox = Instantiate(Fox, new Vector3(Random.Range(30f, 50f), 1, Random.Range(30f, 50f)), Quaternion.Euler(0, 0, 0), FoxContainer.transform);
            newFox.GetComponent<Animal>().SetAnimalContainer(FoxContainer);
        }

        InvokeRepeating("CalculateAverageSpeed", 0f, 1f);
        InvokeRepeating("SaveData", 0f, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Time.timeScale *= 2;
        }
        else if (Input.GetKeyDown(KeyCode.L))
        {
            Time.timeScale *= 0.5f;
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

        averageHenSpeed = totalSpeeds / hensNum;
    }

    void SaveData()
    {
        using (StreamWriter writerH = new StreamWriter("Assets/Data/hens.txt", true))
        {
            writerH.WriteLine(GameObject.FindGameObjectsWithTag("Hen").Length.ToString());
        }

        using (StreamWriter writerF = new StreamWriter("Assets/Data/foxes.txt", true))
        {
            writerF.WriteLine(GameObject.FindGameObjectsWithTag("Fox").Length.ToString());
        }
    }
    
}
