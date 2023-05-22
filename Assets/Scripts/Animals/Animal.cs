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

    [SerializeField] private Environment environment;

    public float senseRange = 10f;
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
    private float currentReproduceUrge = 1;

    [SerializeField] private BasicBar hungryBar;
    [SerializeField] private BasicBar thirstyBar;
    [SerializeField] private BasicBar reproduceUrgeBar;
    public float getHungry() { return currentHungry; }
    public float getThirsty() { return currentThirsty; }
    public float getReproduceUrge() { return currentReproduceUrge; }
    public Actions getCurrentAction() { return currentAction; }

    Animator animation;

    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();

        animation = this.GetComponentInChildren<Animator>();

        hungryBar.UpdateValueBar(maxPropValue, currentHungry);
        thirstyBar.UpdateValueBar(maxPropValue, currentThirsty);
        reproduceUrgeBar.UpdateValueBar(maxPropValue, currentReproduceUrge);

        StartCoroutine(Awaiter());
    }

    // Update is called once per frame
    void Update()
    {

        //-----------------------------Behaviour---------------------------------

        if(currentAction == Actions.EXPLORING)
        {
            //Si esta explorando, que busque un punto aleatorio, y establece el nearestTarget a ese punto
            MoveToRandomPoint();
        }
        else
        {
            if (allTargets.Count > 0)
            {
                GetClosestTarget();
                Vector3 position = nearestTarget.transform.position;
                navMeshAgent.SetDestination(position);
            }
        }

        CheckAvailableTargets();

        //------------------------------Animal-----------------------------------
        UpdateValues();

        SetAction();

        //DoAction();

        SetAnimation();

    }

    //Funcion que comprueba en el environment los objetivos disponibles para el animal (en su campo de sentido)
    void CheckAvailableTargets()
    {
        List<GameObject> availableTargets = environment.GetTargets(this.transform.position, this.senseRange, this.targetTag);

        if(availableTargets.Count == 0) //Si no hay objetivos disponibles con ese tag
        {
            currentAction = Actions.EXPLORING;
        }
        else
        {
            currentAction = Actions.IDLE; //Si hay, pasa a IDLE, entrando en el bucle de búsqueda de nuevo
        }
    }

    void UpdateValues()
    {

        //Si esta comiendo, que llame a la funcion Eat?
        if (currentAction == Actions.EATING)
        {
            Eat(this.plantTarget);
        }
        else if(currentAction == Actions.DRINKING)
        {
            Drink();
        }
        else
        {
            currentReproduceUrge += 0.01f;
            

            currentHungry += 0.02f;
            currentThirsty += 0.015f;

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
        //if(currentReproduceUrge > currentHungry && currentReproduceUrge > currentThirsty && currentHungry < 50 && currentThirsty < 50)
        //{
        //    currentAction = Actions.SEARCHING_MATE;
        //}

        if (currentAction != Actions.EATING && currentAction != Actions.DRINKING)
        {
            currentAction = (currentHungry > currentThirsty) ? Actions.SEARCHING_FOOD : Actions.SEARCHING_WATER;
        }

        switch (currentAction)
        {
            case Actions.SEARCHING_FOOD: targetTag = "Food"; break;
            case Actions.SEARCHING_WATER: targetTag = "Water"; break;
            //case Actions.SEARCHING_MATE: targetTag = this.tag; break;
            default: targetTag = ""; break;
        }
    }

    void GetClosestTarget()
    {
        //importante la condicion de que el tag sea distinto, ya que si no siempre estará fija en el mismo target
        foreach (GameObject target in allTargets)
        {
            float distance = Vector3.Distance(transform.position, target.transform.position);
            Debug.Log("TARGET: " + target.name + ", DISTANCE: " + distance);
            if (distance <= senseRange)
            {
                if (distance < minDist && target != null)
                {
                    minDist = distance;
                    nearestTarget = target;
                }
            }
            else
            {
                Debug.Log("TARGET: " + target.name + " is too far.");
                //nearestTarget = null;
            }
        }
    }

    IEnumerator Awaiter() //Main Awaiter function
    {
        yield return new WaitForSeconds(waitTime);
        allTargets = GameObject.FindGameObjectsWithTag(targetTag).ToList();
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
        } else
        {
            //Cuando termine de comer, pasa a la accion IDLE para que en el ciclo pueda entrar en el if que establece la accion.
            this.currentAction = Actions.IDLE;
            StartCoroutine(Awaiter());
        }
        
    }

    void Drink()
    {
        if(currentThirsty > 1)
        {
            this.currentThirsty -= 0.04f;
        }
        else
        {
            this.currentAction = Actions.IDLE;
            StartCoroutine(Awaiter());
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("Animal: " + this.gameObject.name + " triggered with " + other.gameObject.name);
        //Aquí, dependiendo del tipo de objeto con el que se encuentre, hará una cosa u otra
        if (other.gameObject.tag == "Water" && currentAction == Actions.SEARCHING_WATER)
        {
            //currentThirsty = 0;
            currentAction = Actions.DRINKING;
            //StartCoroutine(Awaiter());
        }
        if (other.gameObject.tag == "Food" && currentAction == Actions.SEARCHING_FOOD)
        {
            //Como por ahora el unico tipo de comida es Plant, si el tag es food sabemos que es una planta, por lo que obtenemos el componente del padre del gameObject
            this.plantTarget = other.GetComponentInParent<Plant>(); //Guardamos la planta objetivo
            currentAction = Actions.EATING; //Actualizamos la accion
            //Debug.Log("Food amount: " + plantTarget.foodAmount);
            //Eat(plantTarget);
        }
    }

    void SetAnimation()
    {
        if (currentAction == Actions.SEARCHING_WATER || currentAction == Actions.SEARCHING_FOOD)
        {
            //Hay que declarar la "actual" a false antes de indicarle la nueva
            animation.SetBool("Eat", false);
            animation.SetBool("Walk", true);
        }
        if (currentAction == Actions.EATING || currentAction == Actions.DRINKING)
        {
            animation.SetBool("Walk", false);
            animation.SetBool("Eat", true);
        }
        if (currentAction == Actions.IDLE)
        {
            animation.SetBool("Idle", true);
        }
    }

    //Funciones movimiento aleatorio
    void MoveToRandomPoint()
    {
        //Genera un punto aleatorio dentro del NavMesh
        Vector3 randomPoint = RandomNavMeshPoint();
        
        //Establece el punto como destino del NavMesh
        navMeshAgent.SetDestination(randomPoint);
        GameObject randomPointObject = new GameObject("Random Point");
        randomPointObject.transform.transform.position = randomPoint; //ya que tenemos la posicion, se la damos al objeto vacio
        nearestTarget = randomPointObject;

    }
    Vector3 RandomNavMeshPoint()
    {
        // Genera un punto aleatorio dentro del NavMesh
        Vector3 randomDirection = Random.insideUnitSphere * 10.0f;
        randomDirection += transform.position;
        NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;
        if (NavMesh.SamplePosition(randomDirection, out hit, 10.0f, NavMesh.AllAreas))
        {
            finalPosition = hit.position;
        }
        return finalPosition;
    }

}

public enum Actions
{
    IDLE,
    SEARCHING_FOOD,
    EATING,
    SEARCHING_WATER,
    DRINKING,
    SEARCHING_MATE,
    EXPLORING,
}

