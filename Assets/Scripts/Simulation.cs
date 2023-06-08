using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Simulation : MonoBehaviour
{
    [SerializeField] public float startHenNumber;
    [SerializeField] public GameObject Hen;
    [SerializeField] public GameObject HenContainer;
    // Start is called before the first frame update
    void Start()
    {
        //Spawn 10 hens at random positions
        for (int i = 0; i < startHenNumber; i++)
        {
            GameObject newHen = Instantiate(Hen, new Vector3(Random.RandomRange(30f, 50f), 1, Random.RandomRange(30f, 50f)), Quaternion.Euler(0, 0, 0), HenContainer.transform);
            newHen.GetComponent<Animal>().SetAnimalContainer(HenContainer);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
