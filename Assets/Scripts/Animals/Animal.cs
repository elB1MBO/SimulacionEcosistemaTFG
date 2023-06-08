using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

/**
 * Animal basic class
 */
public class Animal : MonoBehaviour
{
    public float senseRange;
    public string targetTag;
    public List<GameObject> allTargets;
    public GameObject nearestTarget;

    private Plant plantTarget;

    float waitTime = 1;

    float minDist;

    public NavMeshAgent navMeshAgent;

    public Actions currentAction = Actions.IDLE;

    //Properties
    private float maxPropValue = 100;

    private float currentHungry = 1;
    private float currentThirsty = 1;
    private float currentReproduceUrge = 10;

    [SerializeField] private BasicBar hungryBar;
    [SerializeField] private BasicBar thirstyBar;
    [SerializeField] private BasicBar reproduceUrgeBar;

    public float getHungry() { return currentHungry; }
    public float getThirsty() { return currentThirsty; }
    public float getReproduceUrge() { return currentReproduceUrge; }
    public Actions getCurrentAction() { return currentAction; }

    Animator henAnimation;

    // Start is called before the first frame update
    void Start()
    {
        //SetGenre();

        navMeshAgent = GetComponent<NavMeshAgent>();

        henAnimation = this.GetComponentInChildren<Animator>();

        hungryBar.UpdateValueBar(maxPropValue, currentHungry);
        thirstyBar.UpdateValueBar(maxPropValue, currentThirsty);
        reproduceUrgeBar.UpdateValueBar(maxPropValue, currentReproduceUrge);

        StartCoroutine(Awaiter(waitTime));
    }

    // Update is called once per frame
    void Update()
    {

        if (allTargets.Count > 0)
        {
            GetClosestTarget();
            Vector3 position = nearestTarget.transform.position;
            navMeshAgent.SetDestination(position);
        }

        //------------------------------Animal-----------------------------------
        UpdateValues();

        SetAction();

        //DoAction();

        SetAnimation();

    }

    void UpdateValues()
    {

        //Si esta comiendo, que llame a la funcion Eat?
        if (currentAction == Actions.EATING)
        {
            Eat(this.plantTarget);
        }
        else if (currentAction == Actions.DRINKING)
        {
            Drink();
        }
        else
        {
            //currentReproduceUrge += 0.01f;

            currentHungry += 0.01f;
            currentThirsty += 0.01f;

            if (currentHungry >= maxPropValue || currentThirsty >= maxPropValue)
            {
                Destroy(gameObject);
            }
        }

        hungryBar.UpdateValueBar(maxPropValue, currentHungry);
        thirstyBar.UpdateValueBar(maxPropValue, currentThirsty);
        reproduceUrgeBar.UpdateValueBar(maxPropValue, currentReproduceUrge);
    }

    void SetAction()
    {
        if (currentAction != Actions.EATING && currentAction != Actions.DRINKING)
        {
            if (currentHungry > currentThirsty)
            {
                currentAction = Actions.SEARCHING_FOOD;
            }
            else
            {
                currentAction = Actions.SEARCHING_WATER;
            }
        }

        switch (currentAction)
        {
            case Actions.SEARCHING_FOOD: targetTag = "Food"; break;
            case Actions.SEARCHING_WATER: targetTag = "Water"; break;
            default: targetTag = ""; break;
        }
    }

    //Funcion que busca el target mas cercano de los diponibles      
    void GetClosestTarget()
    {
        foreach (GameObject target in allTargets)
        {
            float distance = Vector3.Distance(transform.position, target.transform.position);
            if (distance <= senseRange)
            {
                if (distance < minDist && target != null)
                {
                    minDist = distance;
                    nearestTarget = target;
                }
            }
        }
    }

    //Función principal de espera tras una accion, que devuelve al animal al ciclo de busqueda de un ojbetivo
    IEnumerator Awaiter(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        allTargets = GameObject.FindGameObjectsWithTag(targetTag).ToList();
        if (targetTag == this.gameObject.tag)
        {
            allTargets.Remove(this.gameObject);
        }
        nearestTarget = null;
        minDist = Mathf.Infinity;
    }

    void Eat(Plant plant)
    {
        //Debug.Log("Comiendose " + plant.name + ", currentHungry: " + currentHungry + ", isEdible: " + plant.IsEdible());
        navMeshAgent.SetDestination(this.transform.position);
        //Si se encuentra con una planta, hay que decirle al navigation que el nuevo objetivo es la posición actual, para que no se coloque en el centro de la planta:
        nearestTarget = this.gameObject;

        if (plant.IsEdible() && this.currentHungry > 1)
        {
            plant.Consume(0.02f);
            this.currentHungry -= 0.04f;
        }
        else
        {
            //Cuando termine de comer, pasa a la accion IDLE para que en el ciclo pueda entrar en el if que establece la accion.
            this.currentAction = Actions.IDLE;
            StartCoroutine(Awaiter(waitTime));
        }

    }

    void Drink()
    {
        if (currentThirsty > 1)
        {
            this.currentThirsty -= 0.04f;
        }
        else
        {
            this.currentAction = Actions.IDLE;
            StartCoroutine(Awaiter(waitTime));
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("Animal: " + this.gameObject.name + " triggered with " + other.gameObject.name);
        //Aquí, dependiendo del tipo de objeto con el que se encuentre, hará una cosa u otra
        if (other.gameObject.tag == "Water" && currentAction == Actions.SEARCHING_WATER)
        {
            currentAction = Actions.DRINKING;
        }
        if (other.gameObject.tag == "Food" && currentAction == Actions.SEARCHING_FOOD)
        {
            //Como por ahora el unico tipo de comida es Plant, si el tag es food sabemos que es una planta, por lo que obtenemos el componente del padre del gameObject
            this.plantTarget = other.GetComponentInParent<Plant>(); //Guardamos la planta objetivo
            currentAction = Actions.EATING; //Actualizamos la accion

        }
    }


    void SetAnimation()
    {
        if (currentAction == Actions.SEARCHING_WATER || currentAction == Actions.SEARCHING_FOOD)
        {
            //Hay que declarar la "actual" a false antes de indicarle la nueva
            henAnimation.SetBool("Eat", false);
            henAnimation.SetBool("Walk", true);
        }
        if (currentAction == Actions.EATING || currentAction == Actions.DRINKING)
        {
            henAnimation.SetBool("Walk", false);
            henAnimation.SetBool("Eat", true);
        }
        if (currentAction == Actions.IDLE)
        {
            henAnimation.SetBool("Idle", true);
        }
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

