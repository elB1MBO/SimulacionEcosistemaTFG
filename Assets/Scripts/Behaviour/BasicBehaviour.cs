using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicBehaviour : MonoBehaviour
{
    //public int sense = 5;

    public string targetTag = "Food";

    public GameObject[] allTargets;
    public GameObject actualTarget;
   
    //Settings
    [SerializeField]
    float movSpeed = 1.0f;
    float rotateSpeed = 1.0f;
    float minDist = Mathf.Infinity;

    // Update is called once per frame
    void Update()
    {
        allTargets = GameObject.FindGameObjectsWithTag(targetTag);
        GetClosestTarget();
        if(actualTarget != null)
        {
            print(actualTarget.name);
            LookAt(actualTarget);
            MoveAt(actualTarget);
        } else
        {
            GameObject actualTarget;
            GetClosestTarget();
        }
    }

    void GetClosestTarget()
    {
       
        foreach (GameObject target in allTargets)
        {
            float distance = Vector3.Distance(transform.position, target.transform.position);
            if(distance < minDist && target != null)
            {
                minDist = distance;
                actualTarget = target;
            }
        }
    }

    void MoveAt(GameObject target)
    {
        if(target.transform.position != transform.position)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, movSpeed * Time.deltaTime);
        } else
        {
            Eat();
        }
    }

    void LookAt(GameObject target) 
    {
        if(target.transform.position != transform.position)
        {
            // Rotate the forward vector towards the target direction by one step
            Vector3 newDirection = Vector3.RotateTowards(transform.position, target.transform.position, rotateSpeed * Time.deltaTime, 0.0f);
            
            // Draw a ray pointing at our target in
            Debug.DrawRay(transform.position, newDirection, Color.red);

            // Calculate a rotation a step closer to the target and applies rotation to this object
            transform.rotation = Quaternion.LookRotation(newDirection);
        }
    }

    void Eat()
    {
        if(actualTarget != null)
        {
            Destroy(actualTarget);
        }
    }
}
