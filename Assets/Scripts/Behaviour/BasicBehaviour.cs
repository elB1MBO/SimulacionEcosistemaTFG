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
            StartCoroutine(Awaiter());
        }
        if (other.gameObject.tag == "Food")
        {
            Eat(other.gameObject);
        }
        //Reinicia el comportamiento del animal cuando alcanza un objetivo
        this.Start();
    }
    void Eat(GameObject gameObject)
    {
        Debug.Log("Animal: " + this.gameObject.name + " has eaten " + gameObject.name);
        allTargets.Remove(gameObject);
        Destroy(gameObject);
        GetClosestTarget();
    }


    //void MoveAt(GameObject target)
    //{
    //    if(target.transform.position != transform.position)
    //    {
    //        transform.position = Vector3.MoveTowards(transform.position, target.transform.position, movSpeed * Time.deltaTime);
    //    } else
    //    {
    //        Eat();
    //    }
    //}

    //void LookAt(GameObject target) 
    //{
    //    if(target.transform.position != transform.position)
    //    {
    //        // Rotate the forward vector towards the target direction by one step
    //        Vector3 newDirection = Vector3.RotateTowards(transform.position, target.transform.position, rotateSpeed * Time.deltaTime, 0.0f);

    //        // Draw a ray pointing at our target in
    //        Debug.DrawRay(transform.position, newDirection, Color.red);

    //        // Calculate a rotation a step closer to the target and applies rotation to this object
    //        transform.rotation = Quaternion.LookRotation(newDirection);
    //    }
    //}
}
