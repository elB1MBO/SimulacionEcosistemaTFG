using System.Collections;
using System.Collections.Generic;
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
            Instantiate(Hen, new Vector3(Random.RandomRange(30f, 50f), 1, Random.RandomRange(30f, 50f)), Quaternion.Euler(0, 0, 0), HenContainer.transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
