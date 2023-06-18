using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bush : MonoBehaviour
{

    [SerializeField] private GameObject bushResource;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Hen")
        {
            bushResource.GetComponent<Plant>().OnBushCollision(1);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Hen")
        {
            bushResource.GetComponent<Plant>().OnBushCollision(-1);
        }
    }
}
