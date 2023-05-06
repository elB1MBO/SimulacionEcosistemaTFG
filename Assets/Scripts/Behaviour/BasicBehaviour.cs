using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class BasicBehaviour : MonoBehaviour
{
    //public int sense = 5;

    public string targetTag;

    public List<GameObject> allTargets;
    public GameObject nearestTarget; // nearest target

    [SerializeField] private float eatTime = 1;
   
    //Settings
    //[SerializeField]
    //float movSpeed = 1.0f;
    //float rotateSpeed = 1.0f;
    float minDist;

    public NavMeshAgent navigation;

    public Actions currentAction = Actions.IDLE;

    private void Start()
    {
        StartCoroutine(Awaiter());
    }

    // Update is called once per frame
    void Update()
    {
        if(allTargets.Count > 0)
        {
            GetClosestTarget();
            navigation.destination = nearestTarget.transform.position;
        }
    }

    void GetClosestTarget()
    {
        //importante la condicion de que el tag sea distinto, ya que si no siempre estará fija en el mismo target
        foreach (GameObject target in allTargets)
        {
            float distance = Vector3.Distance(transform.position, target.transform.position);
            if (distance < minDist && target != null) 
            {
                minDist = distance;
                nearestTarget = target;
            }
        }

        switch (nearestTarget.tag)
        {
            case "Food":
                currentAction = Actions.SEARCHING_FOOD; break;
            case "Water":
                currentAction = Actions.SEARCHING_WATER; break;
            default: currentAction = Actions.IDLE; break;
        }
    }

    IEnumerator Awaiter()
    {
        yield return new WaitForSeconds(eatTime);
        allTargets = GameObject.FindGameObjectsWithTag(targetTag).ToList();
        nearestTarget = null;
        minDist = Mathf.Infinity;
    }

    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("Animal: " + this.gameObject.name + " triggered with " + other.gameObject.name);
        //Aquí, dependiendo del tipo de objeto con el que se encuentre, hará una cosa u otra
        if(other.gameObject.tag == "Water")
        {
            this.targetTag = "Food";
            currentAction = Actions.DRINKING;
            StartCoroutine(Awaiter());
        }
        if(other.gameObject.tag == "Food")
        {
            this.targetTag = "Water";
            currentAction = Actions.EATING;
            StartCoroutine(Awaiter());
            
            //Eat(other.gameObject);
        }
        //Reinicia el comportamiento del animal cuando alcanza un objetivo
        //this.Start();
    }
    void Eat(GameObject gameObject)
    {
        Debug.Log("Animal: " + this.gameObject.name + " is eating " + gameObject.name);
        allTargets.Remove(gameObject);
        Destroy(gameObject);
        GetClosestTarget();
    }

}

public enum Actions
{
    IDLE,
    SEARCHING_FOOD,
    EATING,
    SEARCHING_WATER,
    DRINKING,
}
