using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq; //Biblioteca necesaria para el ToList del Find

public class Environment : MonoBehaviour
{

    [SerializeField] private List<GameObject> allTargets;
    [SerializeField] private List<GameObject> inRangeTargets;
    private float distance;

    [SerializeField] private Animal animal; //Animal que tiene asignado este objeto Environment --> básicamente sirve para evitar que se busque a sí mismo como animal más cercano al reproducirse

    public List<GameObject> GetTargets(Vector3 position, float range, string tag)
    {
        //Debug.Log("Position: " + position + ", Range: " + range + ", Tag: " + tag);
        allTargets = GameObject.FindGameObjectsWithTag(tag).ToList();
        allTargets.Remove(animal.gameObject);
        inRangeTargets = new List<GameObject>(); //Limpiamos el contenido de la lista
        foreach (GameObject target in allTargets)
        {
            distance = Vector3.Distance(transform.position, target.transform.position);
            
            if(distance <= range)
            {
                //Debug.Log("Distance: " + distance);
                inRangeTargets.Add(target);
            }
        }
        return inRangeTargets;
    }

}
