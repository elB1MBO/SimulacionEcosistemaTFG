using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicBehaviour : MonoBehaviour
{
    //public int sense = 5;

    //Settings
    float movSpeed = 1.0f;
    float rotateSpeed = 1.0f;

    public string targetTag = "Food";

    public GameObject[] allTargets;
    public GameObject actualTarget;

    // Start is called before the first frame update
    void Start()
    {
        allTargets = GameObject.FindGameObjectsWithTag(targetTag);
    }

    // Update is called once per frame
    void Update()
    {
        GetClosestTarget();
        print(actualTarget.name);
        LookAt(actualTarget);
        MoveAt(actualTarget);
    }

    void GetClosestTarget()
    {
        float minDist = Mathf.Infinity;

        foreach (GameObject target in allTargets)
        {
            float distance = Vector3.Distance(transform.position, target.transform.position);
            if(distance < minDist)
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
            //Destroy(actualTarget);
            GetClosestTarget();
        }
    }
}
