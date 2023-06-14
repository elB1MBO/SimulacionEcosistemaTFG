using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

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
    float speed = 2.5f;

    [SerializeField] float energyWasteValue = 0.01f;
    [SerializeField] float energyRestoreValue = 0.02f;

    public NavMeshAgent navMeshAgent;

    [SerializeField] private Actions currentAction = Actions.IDLE;

    [SerializeField] private GameObject animalContainer;
    public void SetAnimalContainer(GameObject animalContainer) { this.animalContainer = animalContainer; }

    //Properties
    private float maxPropValue = 100;

    [SerializeField] private float currentHunger;
    private float currentThirst;
    private float currentReproduceUrge;

    [SerializeField] private BasicBar hungryBar;
    [SerializeField] private BasicBar thirstyBar;
    [SerializeField] private BasicBar reproduceUrgeBar;

    public float GetHunger() { return currentHunger; }
    public float GetThirst() { return currentThirst; }
    public float GetReproduceUrge() { return currentReproduceUrge; }
    public Actions GetCurrentAction() { return currentAction; }

    Animator henAnimation;

    Vector3 randomPoint;
    GameObject randomPointObject;
    bool randomPointSetted = false; 

    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = speed;

        henAnimation = this.GetComponentInChildren<Animator>();

        currentHunger = Random.Range(1, 20);
        currentThirst = Random.Range(1, 20);
        currentReproduceUrge = Random.Range(1, 20);

        hungryBar.UpdateValueBar(maxPropValue, currentHunger);
        thirstyBar.UpdateValueBar(maxPropValue, currentThirst);
        reproduceUrgeBar.UpdateValueBar(maxPropValue, currentReproduceUrge);

        StartCoroutine(Awaiter(waitTime));
    }

    // Update is called once per frame
    void Update()
    {
        if (allTargets.Count > 0)
        {
            GetClosestTarget();
            if(nearestTarget == null || nearestTarget.tag == "Growing")
            {
                StartCoroutine(Awaiter(waitTime));
            }
            else if (nearestTarget.tag == "Water")
            {
                Vector3 targetClosestPoint = nearestTarget.GetComponent<Collider>().ClosestPoint(this.transform.position);
                navMeshAgent.SetDestination(targetClosestPoint);
            }
            else if (nearestTarget.tag == "Food" || nearestTarget.tag == this.gameObject.tag) //Si es un bush, no importa, ya que se detiene al colisionar
            {
                Vector3 targetPosition = nearestTarget.transform.position;
                navMeshAgent.SetDestination(targetPosition);
            }
        }
        else
        {
            MoveToRandomPoint();
        }

        //------------------------------Animal-----------------------------------
        UpdateValues();

        SetAction();

        SetAnimation();
    }

    void UpdateValues()
    {
        switch (currentAction)
        {
            case Actions.EATING: Eat(this.plantTarget); break;
            case Actions.DRINKING: Drink(); break;
            case Actions.MATING: Reproduce(); break;
            default: { 
                    currentHunger += energyWasteValue; 
                    currentThirst += energyWasteValue; 
                    if (currentReproduceUrge < maxPropValue) { currentReproduceUrge += energyWasteValue; } 
                } break;
        }

        if (currentHunger >= maxPropValue || currentThirst >= maxPropValue)
        {
            Destroy(gameObject);
        }


        hungryBar.UpdateValueBar(maxPropValue, currentHunger);
        thirstyBar.UpdateValueBar(maxPropValue, currentThirst);
        reproduceUrgeBar.UpdateValueBar(maxPropValue, currentReproduceUrge);
    }

    void SetAction()
    {
        Actions action = currentAction;
        if(allTargets.Count == 0) { currentAction = Actions.EXPLORING; }
        if (currentAction != Actions.EATING && currentAction != Actions.DRINKING && currentAction != Actions.MATING) //Acciones que no se pueden interrumpir
        {
            if (currentReproduceUrge > 40 && currentHunger < 40 && currentThirst < 40 && currentAction != Actions.MATING)
            {
                currentAction = Actions.SEARCHING_MATE;
            }
            else if (currentHunger > currentThirst)
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
            case Actions.SEARCHING_MATE: targetTag = this.gameObject.tag; break;
            default: break;
        }

        //SI HA CAMBIADO LA ACCION, QUE VUELVA A LLAMAR AL AWAITER
        if (currentAction != action)
        {
            StartCoroutine(Awaiter(waitTime));
        }
    }

    //Funcion que busca el target mas cercano de los diponibles      
    void GetClosestTarget()
    {
        foreach (GameObject target in this.allTargets)
        {
            if (target != null)
            {
                float distance = Vector3.Distance(transform.position, target.transform.position);
                if (distance <= senseRange)
                {
                    if (distance < this.minDist && target != null)
                    {
                        this.minDist = distance;
                        this.nearestTarget = target;
                    }
                }
            }
        }
    }

    //Función principal de espera tras una accion, que devuelve al animal al ciclo de busqueda de un ojbetivo
    IEnumerator Awaiter(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        //if (targetTag == null || targetTag == string.Empty) { this.gameObject.name = "Gallina"; }
        this.allTargets = GameObject.FindGameObjectsWithTag(targetTag).ToList();
        if (targetTag == this.gameObject.tag)
        {
            this.allTargets.Remove(this.gameObject);
        }
        this.nearestTarget = null;
        this.minDist = Mathf.Infinity;
    }

    void Eat(Plant plant)
    {
        navMeshAgent.SetDestination(this.transform.position);
        //Si se encuentra con una planta, hay que decirle al navigation que el nuevo objetivo es la posición actual, para que no se coloque en el centro de la planta:
        nearestTarget = this.gameObject; // -------> IMPORTANTE

        if (plant.IsEdible() && this.currentHunger > 1)
        {
            plant.Consume(energyRestoreValue);
            this.currentHunger -= energyRestoreValue;
        }
        else
        {
            //Cuando termine de comer, pasa a la accion IDLE para que en el ciclo pueda entrar en el if que establece la accion.
            this.currentAction = Actions.IDLE;
        }

    }

    void Drink()
    {
        if (currentThirst > 1)
        {
            currentThirst -= energyRestoreValue;
        }
        else
        {
            currentAction = Actions.IDLE;
        }
    }

    void Reproduce()
    {
        if (currentReproduceUrge > 1)
        {
            currentReproduceUrge -= energyRestoreValue*10;
        }
        else
        {
            currentAction = Actions.IDLE;
            //Add genetic factor

            GameObject newAnimal = Instantiate(this.gameObject, gameObject.transform.position, Quaternion.identity, animalContainer.transform);
            float scale = this.gameObject.transform.localScale.x;
            newAnimal.transform.localScale = new Vector3(scale * 1.2f, scale * 1.2f, scale * 1.2f);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
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
        if (other.gameObject.tag == this.gameObject.tag && currentAction == Actions.SEARCHING_MATE) // && other.GetComponent<Animal>().GetCurrentAction() == Actions.SEARCHING_MATE
        {
            currentAction = Actions.MATING;
        }
        if(other.gameObject.tag == "RandomPoint")
        {
            Destroy(other.gameObject);
        }
    }


    void SetAnimation()
    {
        if (currentAction == Actions.SEARCHING_WATER || currentAction == Actions.SEARCHING_FOOD || currentAction == Actions.SEARCHING_MATE || currentAction == Actions.EXPLORING)
        {
            //Hay que declarar la "actual" a false antes de indicarle la nueva
            henAnimation.SetBool("Eat", false);
            henAnimation.SetBool("Walk", true);
            henAnimation.SetBool("Turn Head", false);
        }
        if (currentAction == Actions.EATING || currentAction == Actions.DRINKING)
        {
            henAnimation.SetBool("Walk", false);
            henAnimation.SetBool("Eat", true);
            henAnimation.SetBool("Turn Head", false);
        }
        if(currentAction == Actions.MATING || currentAction == Actions.EXPLORING)
        {
            henAnimation.SetBool("Walk", false);
            henAnimation.SetBool("Eat", false);
            henAnimation.SetBool("Turn Head", true);
        }
        if (currentAction == Actions.IDLE)
        {
            henAnimation.SetBool("Idle", true);
        }
    }

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
        if (this.transform.position == randomPoint)
        {
            Destroy(randomPointObject.gameObject);
            randomPointSetted = false;
        }
    }
    Vector3 RandomNavMeshPoint()
    {
        Vector3 randomDirection = Random.insideUnitSphere * 10.0f;
        randomDirection += this.transform.position;
        Vector3 finalPosition = Vector3.zero;
        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, 10f, NavMesh.AllAreas))
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
    MATING,
    EXPLORING
}

