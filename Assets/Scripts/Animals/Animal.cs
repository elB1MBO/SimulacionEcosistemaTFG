using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace AnimalNamespace
{
    /**
     * Animal basic class
     */
    public class Animal : MonoBehaviour
    {
        public string targetTag;
        public List<GameObject> allTargets;
        public GameObject nearestTarget;

        float drinkTime = 1;
        float eatTime = 2;

        float minDist;

        public NavMeshAgent navigation;

        public Actions currentAction = Actions.IDLE;

        //Properties
        private float maxPropValue = 100;

        private float currentHungry = 1;
        private float currentThirsty = 1;
        private float currentReproduceUrge = 1;

        [SerializeField] private BasicBar hungryBar;
        [SerializeField] private BasicBar thirstyBar;
        [SerializeField] private BasicBar reproduceUrgeBar;
        public float getHungry() { return currentHungry; }
        public float getThirsty() { return currentThirsty; }
        public float getReproduceUrge() { return currentReproduceUrge; }
        public Actions getCurrentAction() { return currentAction; }


        // Start is called before the first frame update
        void Start()
        {
            hungryBar.UpdateValueBar(maxPropValue, currentHungry);
            thirstyBar.UpdateValueBar(maxPropValue, currentThirsty);
            reproduceUrgeBar.UpdateValueBar(maxPropValue, currentReproduceUrge);

            StartCoroutine(Awaiter());
        }

        // Update is called once per frame
        void Update()
        {

            //-----------------------------Behaviour---------------------------------

            if (allTargets.Count > 0)
            {
                GetClosestTarget();
                navigation.destination = nearestTarget.transform.position;
            }

            //------------------------------Animal-----------------------------------
            currentReproduceUrge += 0.01f;
            reproduceUrgeBar.UpdateValueBar(maxPropValue, currentReproduceUrge);

            currentHungry += 0.02f;
            currentThirsty += 0.015f;

            if (currentHungry >= maxPropValue || currentThirsty >= maxPropValue)
            {
                Destroy(gameObject);
            }
            
            //if(currentReproduceUrge > currentHungry && currentReproduceUrge > currentThirsty && currentHungry < 50 && currentThirsty < 50)
            //{
            //    currentAction = Actions.SEARCHING_MATE;
            //}

            currentAction = (currentHungry > currentThirsty) ? Actions.SEARCHING_FOOD : Actions.SEARCHING_WATER;

            switch (currentAction)
            {
                case Actions.SEARCHING_FOOD: targetTag = "Food"; break;
                case Actions.SEARCHING_WATER: targetTag = "Water"; break;
                //case Actions.SEARCHING_MATE: targetTag = this.tag; break;
                default: targetTag=""; break;
            }

            hungryBar.UpdateValueBar(maxPropValue, currentHungry);
            thirstyBar.UpdateValueBar(maxPropValue, currentThirsty);
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

        IEnumerator Eat()
        {
            yield return new WaitForSeconds(eatTime);
        }

        public void OnTriggerEnter(Collider other)
        {
            Debug.Log("Animal: " + this.gameObject.name + " triggered with " + other.gameObject.name);
            //Aquí, dependiendo del tipo de objeto con el que se encuentre, hará una cosa u otra
            if (other.gameObject.tag == "Water")
            {
                currentThirsty = 0;
                currentAction = Actions.DRINKING;
            }
            if (other.gameObject.tag == "Food")
            {
                currentHungry = 0;
                currentAction = Actions.EATING;
                StartCoroutine(Eat());
            }

            StartCoroutine(Awaiter());
            //Reinicia el comportamiento del animal cuando alcanza un objetivo
            //this.Start();
        }
        //void Eat(GameObject gameObject)
        //{
        //    Debug.Log("Animal: " + this.gameObject.name + " is eating " + gameObject.name);
        //    allTargets.Remove(gameObject);
        //    Destroy(gameObject);
        //    GetClosestTarget();
        //}

    }


    public enum Actions
    {
        IDLE,
        SEARCHING_FOOD,
        EATING,
        SEARCHING_WATER,
        DRINKING,
        SEARCHING_MATE,
    }

}
