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
    [SerializeField] private List<GameObject> irtargets;
    public GameObject nearestTarget;

    private Plant plantTarget;

    const float waitTime = 1;

    float minDist;
    [SerializeField] float speed;

    [SerializeField] float energyWasteValue;
    [SerializeField] float energyRestoreValue;

    public NavMeshAgent navMeshAgent;

    [SerializeField] private Actions currentAction = Actions.IDLE;

    [SerializeField] private GameObject animalContainer;
    public void SetAnimalContainer(GameObject animalContainer) { this.animalContainer = animalContainer; }

    //Properties
    private const float maxPropValue = 100;

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

    Animator animator;

    [SerializeField] Vector3 randomPoint;
    [SerializeField] bool randomPointSetted = false;
    [SerializeField] Vector3 destino;

    private ParticleSystem reproduceParticleSystem;

    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = this.speed;

        animator = this.GetComponentInChildren<Animator>();

        currentHunger = Random.Range(1, 20);
        currentThirst = Random.Range(1, 20);
        currentReproduceUrge = Random.Range(1, 20);

        hungryBar.UpdateValueBar(maxPropValue, currentHunger);
        thirstyBar.UpdateValueBar(maxPropValue, currentThirst);
        reproduceUrgeBar.UpdateValueBar(maxPropValue, currentReproduceUrge);

        this.reproduceParticleSystem = GetComponent<ParticleSystem>();

        StartCoroutine(Awaiter(waitTime));
    }

    // Update is called once per frame
    void Update()
    {
        GetAllTargets();

        SetTarget();

        UpdateValues();

        SetAction();

        SetAnimation();

        destino = this.navMeshAgent.destination;
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
            case Actions.SEARCHING_FOOD:
                if (this.gameObject.tag == "Fox") { targetTag = "Hen"; }
                else targetTag = "Food";
                break;
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
    void SetTarget()
    {
        List<GameObject> inRangeTargets = GetInRangeTargets();
        irtargets = inRangeTargets;

        if(inRangeTargets.Count == 0)
        {
            Explore();
        }
        else
        {
            randomPointSetted = false;
            GetClosestTarget(inRangeTargets);

            if (nearestTarget == null || nearestTarget.tag == "Growing")
            {
                StartCoroutine(Awaiter(waitTime));
            }
            else if (nearestTarget.tag == "Water")
            {
                Vector3 targetClosestPoint = nearestTarget.GetComponent<Collider>().ClosestPoint(this.transform.position);
                navMeshAgent.SetDestination(targetClosestPoint);
            }
            else if (nearestTarget.tag == "Food" || nearestTarget.tag == "Hen" || nearestTarget.tag == this.gameObject.tag) //Si es un bush, no importa, ya que se detiene al colisionar
            {
                Vector3 targetPosition = nearestTarget.transform.position;
                navMeshAgent.SetDestination(targetPosition);
            }
        }
        
    }
    List<GameObject> GetInRangeTargets()
    {
        List<GameObject> inRangeTargets = new List<GameObject>();

        //Busca en la lista de todos los objetivos con el tag los que están dentro del rango
        foreach (var target in this.allTargets)
        {
            float dist = Vector3.Distance(this.gameObject.transform.position, target.transform.position);
            if(dist <= this.senseRange)
            {
                inRangeTargets.Add(target);
            }
        }

        return inRangeTargets;
    }

    //Funcion que busca el target mas cercano de los diponibles      
    void GetClosestTarget(List<GameObject> inRangeTargets)
    {
        foreach (GameObject target in inRangeTargets)
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
        GetAllTargets();
        this.nearestTarget = null;
        this.minDist = Mathf.Infinity;
    }

    void GetAllTargets()
    {
        this.allTargets = GameObject.FindGameObjectsWithTag(targetTag).ToList();
        if (targetTag == this.gameObject.tag)
        {
            this.allTargets.Remove(this.gameObject);
        }
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

        if(!this.reproduceParticleSystem.isPlaying)
        {
            Debug.Log("Entra");
            this.reproduceParticleSystem.Play();
        }
        if (currentReproduceUrge > 1)
        {
            currentReproduceUrge -= energyRestoreValue*10;
        }
        else
        {
            currentAction = Actions.IDLE;
            //Add genetic factor

            GameObject newAnimal = Instantiate(this.gameObject, gameObject.transform.position, Quaternion.identity, animalContainer.transform);
            //this.reproduceParticleSystem.Stop();
        }
    }

    void Explore()
    {
        if (!randomPointSetted)
        {
            //Genera un punto aleatorio dentro del NavMesh
            randomPoint = RandomNavMeshPoint();
            navMeshAgent.SetDestination(randomPoint); 
            randomPointSetted = true;
        }
        if (Vector3.Distance(this.transform.position, randomPoint) <= 1)
        {
            randomPointSetted = false;
        }
        else if(Vector3.Distance(this.transform.position, destino) <= 1)
        {
            this.navMeshAgent.SetDestination(randomPoint);
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

    public void OnTriggerEvent(Collider other)
    {
        //Aquí, dependiendo del tipo de objeto con el que se encuentre, hará una cosa u otra
        if (other.gameObject.tag == "Water" && currentAction == Actions.SEARCHING_WATER)
        {
            currentAction = Actions.DRINKING;
        }
        if(this.gameObject.tag == "Fox")
        {
            if (other.gameObject.tag == "Hen" && currentAction == Actions.SEARCHING_FOOD)
            {
                Destroy(other.gameObject);
                this.currentHunger = 0;
            }
        }
        else
        {
            if (other.gameObject.tag == "Food" && currentAction == Actions.SEARCHING_FOOD)
            {
                //Como por ahora el unico tipo de comida es Plant, si el tag es food sabemos que es una planta, por lo que obtenemos el componente del padre del gameObject
                this.plantTarget = other.GetComponentInParent<Plant>(); //Guardamos la planta objetivo
                currentAction = Actions.EATING; //Actualizamos la accion
            }
        }
        if (other.gameObject.tag == this.gameObject.tag && currentAction == Actions.SEARCHING_MATE) // && other.GetComponent<Animal>().GetCurrentAction() == Actions.SEARCHING_MATE
        {
            currentAction = Actions.MATING;
        }
        if (other.gameObject.tag == "RandomPoint")
        {
            Destroy(other.gameObject);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        OnTriggerEvent(other);
    }

    public void OnTriggerStay(Collider other)
    {
        OnTriggerEvent(other);
    }


    void SetAnimation()
    {
        if (currentAction == Actions.SEARCHING_WATER || currentAction == Actions.SEARCHING_FOOD || currentAction == Actions.SEARCHING_MATE)
        {
            //Hay que declarar la "actual" a false antes de indicarle la nueva
            animator.SetBool("Eat", false);
            animator.SetBool("Walk", true);
        }
        if (currentAction == Actions.EATING || currentAction == Actions.DRINKING)
        {
            animator.SetBool("Walk", false);
            animator.SetBool("Eat", true);
        }
        if(currentAction == Actions.MATING)
        {
            animator.SetBool("Walk", false);
            animator.SetBool("Eat", false);
        }
        if (currentAction == Actions.IDLE)
        {
            animator.SetBool("Idle", true);
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
    SEARCHING_MATE,
    MATING,
    EXPLORING
}

