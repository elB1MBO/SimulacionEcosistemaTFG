using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq; //Biblioteca necesaria para el ToList del Find

public class Environment : MonoBehaviour
{

    [SerializeField] private List<GameObject> allTargets;
    [SerializeField] private List<GameObject> inRangeTargets;
    private float distance;

    public List<GameObject> GetTargets(Vector3 position, float range, string tag)
    {
        Debug.Log("AAA " + position + " " + range + " " + tag);
        allTargets = GameObject.FindGameObjectsWithTag(tag).ToList();
        Debug.Log("EEEE " + allTargets.ToString());
        Debug.Log("IIII " + inRangeTargets.ToString());
        inRangeTargets = new List<GameObject>(); //Limpiamos el contenido de la lista
        foreach (GameObject target in allTargets)
        {
            distance = Vector3.Distance(transform.position, target.transform.position);
            if(distance <= range)
            {
                inRangeTargets.Add(target);
            }
        }
        return inRangeTargets;
    }

}
