using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{
    private int maxAnimals;
    [SerializeField] private int numAnimals;
    private string objectTag;
    [SerializeField] Plant plant;

    // Start is called before the first frame update
    void Start()
    {
        maxAnimals = 4;
        numAnimals = 0;
        objectTag = gameObject.tag;
    }

    // Update is called once per frame
    void Update()
    {
        if(objectTag == "BushResource")
        {
            if (maxAnimals <= numAnimals || !plant.IsEdible())
            {
                gameObject.tag = "Full";
            }
            else if (numAnimals < maxAnimals && plant.IsEdible())
            {
                gameObject.tag = objectTag;
            }
        }
        else
        {
            if (maxAnimals <= numAnimals)
            {
                gameObject.tag = "Full";
            }
            else
            {
                gameObject.tag = objectTag;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Hen"))
        {
            numAnimals++;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Hen"))
        {
            numAnimals--;
        }
    }
}
