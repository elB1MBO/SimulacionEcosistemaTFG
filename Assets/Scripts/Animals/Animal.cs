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
    private bool randomPointSetted = false;
    private GameObject randomPointObject;
    private Vector3 randomPoint;

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
    private float currentReproduceUrge = 1;

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
        navMeshAgent = GetComponent<NavMeshAgent>();

        henAnimation = this.GetComponentInChildren<Animator>();

        hungryBar.UpdateValueBar(maxPropValue, currentHungry);
        thirstyBar.UpdateValueBar(maxPropValue, currentThirsty);
        reproduceUrgeBar.UpdateValueBar(maxPropValue, currentReproduceUrge);

        StartCoroutine(Awaiter());
    }

    // Update is called once per frame
    void Update()
    {

        //-----------------------------Behaviour---------------------------------
        if(currentAction != Actions.DRINKING && currentAction != Actions.EATING)
        {
            CheckAvailableTargets(); //Comprobamos si hay objetivos disponibles por el tag
        }
        

        if (currentAction != Actions.EXPLORING) //si la accion es exploring, significa que no hay, por lo que asigna un punto aleatorio
        {
            if (allTargets.Count > 0)
            {
                GetClosestTarget();
                Vector3 position = nearestTarget.transform.position;
                navMeshAgent.SetDestination(position);
            }
        }

        //CheckAvailableTargets();

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
            Debug.Log("AYUDA PLS");
            currentAction = Actions.EXPLORING;
            MoveToRandomPoint();
        } else
        {
            if (targetTag == "Food") currentAction = Actions.SEARCHING_FOOD;
            else if(targetTag == "Water") currentAction = Actions.SEARCHING_WATER;
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
        if(currentAction != Actions.EXPLORING)
        {
            if (currentAction != Actions.EATING && currentAction != Actions.DRINKING)
            {
                if (currentReproduceUrge > currentHungry && currentReproduceUrge > currentThirsty && currentHungry < 60 && currentThirsty < 60)
                {
                    currentAction = Actions.SEARCHING_MATE;
                }
                else if (currentHungry > currentThirsty)
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
                case Actions.SEARCHING_MATE: targetTag = "Hen"; break;
                default: targetTag = ""; break;
            }
        }
        
    }

    void GetClosestTarget()
    {
        //importante la condicion de que el tag sea distinto, ya que si no siempre estará fija en el mismo target
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

    IEnumerator Awaiter() //Main Awaiter function
    {
        yield return new WaitForSeconds(waitTime);
        allTargets = GameObject.FindGameObjectsWithTag(targetTag).ToList();
        if(targetTag == this.gameObject.tag)
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
        if (currentAction == Actions.SEARCHING_WATER || currentAction == Actions.SEARCHING_FOOD || currentAction == Actions.EXPLORING || currentAction == Actions.SEARCHING_MATE)
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

    //Funciones movimiento aleatorio
    void MoveToRandomPoint()
    {
        if (!randomPointSetted)
        {
            //Genera un punto aleatorio dentro del NavMesh
            randomPoint = RandomNavMeshPoint();

            //Establece el punto como destino del NavMesh
            navMeshAgent.SetDestination(randomPoint);
            randomPointObject = new GameObject("Random Point");
            randomPointObject.tag = "RandomPoint";
            randomPointObject.transform.transform.position = randomPoint; //ya que tenemos la posicion, se la damos al objeto vacio
            nearestTarget = randomPointObject;

            randomPointSetted = true;
        }
        if(this.transform.position == randomPoint)
        {
            Destroy(randomPointObject.gameObject);
            randomPointSetted = false;
        }
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

