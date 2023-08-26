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
    public float senseRangeSquare;
    public string targetTag;
    public List<GameObject> allTargets;
    public GameObject nearestTarget;

    private Plant plantTarget;
    private Animal mate;

    const float waitTime = 1;

    [SerializeField] private float speed;

    private float energyWasteValue;
    private float energyRestoreValue;

    public NavMeshAgent navMeshAgent;

    private Actions currentAction = Actions.IDLE;

    [SerializeField] private GameObject animalContainer;
    
    //Properties
    private const float maxPropValue = 100;

    private float currentHunger;
    private float currentThirst;
    private float currentReproduceUrge;

    [SerializeField] private BasicBar hungryBar;
    [SerializeField] private BasicBar thirstyBar;
    [SerializeField] private BasicBar reproduceUrgeBar;

    [SerializeField] Animator animator;

    Vector3 randomPoint;
    bool randomPointSetted = false;
    Vector3 destino;

    [SerializeField] private ParticleSystem reproduceParticleSystem;

    public GameObject model;
    private GameObject predator;

    [SerializeField] private DeathManager deathManager;
    [SerializeField] private Simulation simulationManager;

    //Getters y Setters
    public float GetSpeed() { return speed; }
    public float GetHunger() { return currentHunger; }
    public float GetThirst() { return currentThirst; }
    public float GetReproduceUrge() { return currentReproduceUrge; }
    public Actions GetCurrentAction() { return currentAction; }
    public void SetAnimalContainer(GameObject animalContainer) { this.animalContainer = animalContainer; }
    public void SetDeathManager(DeathManager dm) { deathManager = dm; }
    public void SetSimulationManager(Simulation sim) { simulationManager = sim; }

    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent.speed = this.speed;
        //navMeshAgent.stoppingDistance = 0.1f;

        energyRestoreValue = 0.2f;
        energyWasteValue = 0.04f * (this.speed / 2f);

        senseRangeSquare = senseRange * senseRange;

        //animator = this.GetComponentInChildren<Animator>();

        currentHunger = Random.Range(1, 20);
        currentThirst = Random.Range(1, 20);
        currentReproduceUrge = Random.Range(1, 20);

        hungryBar.UpdateValueBar(maxPropValue, currentHunger);
        thirstyBar.UpdateValueBar(maxPropValue, currentThirst);
        reproduceUrgeBar.UpdateValueBar(maxPropValue, currentReproduceUrge);
        
        InvokeRepeating(nameof(GetOrderedTargets), 0f, 1f);

        StartCoroutine(Awaiter(waitTime));
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //if(transform.position.y < -10f) { deathManager.FallDie(gameObject); }

        SetTarget();

        UpdateValues();

        SetAction();

        SetAnimation();

        if (gameObject.CompareTag("Hen"))
        {
            CheckPredators();
        }

        destino = this.navMeshAgent.destination;
    }

    void CheckPredators()
    {
        if (this.navMeshAgent == null || !this.navMeshAgent.enabled) { return; }

        // Huye del depredador más cercano
        if(this.simulationManager.foxesList.Count == 0) { return; }
        predator = this.simulationManager.foxesList.OrderBy(o => (o.transform.position - transform.position).sqrMagnitude).FirstOrDefault();
        if (predator != null)
        {
            float distance = (predator.transform.position - transform.position).sqrMagnitude;
            float sqrSenseRange = senseRangeSquare * 0.25f; // 0.25 = 0.5 * 0.5
            if (distance < sqrSenseRange)
            {
                Vector3 dirToPredator = transform.position - predator.transform.position;
                Vector3 newPos = transform.position + dirToPredator;
                currentAction = Actions.FLEEING;
                this.navMeshAgent.SetDestination(newPos);
            }
        }
    }

    void UpdateValues()
    {
        switch (currentAction)
        {
            case Actions.EATING: if (this.plantTarget != null) { Eat(this.plantTarget); } break;
            case Actions.DRINKING: Drink(); break;
            case Actions.MATING: Reproduce(); break;
            default: {
                    currentHunger += energyWasteValue * Time.timeScale; 
                    currentThirst += energyWasteValue * Time.timeScale; 
                    if (currentReproduceUrge < maxPropValue) 
                    {
                        if (gameObject.CompareTag("Fox")) { currentReproduceUrge += energyWasteValue * 0.5f * Time.timeScale; }
                        else { currentReproduceUrge += energyWasteValue * Time.timeScale; }
                    } 
                } break;
        }

        if(maxPropValue <= currentHunger) { deathManager.Die(gameObject, CauseOfDeath.STARVATION); }
        else if (maxPropValue <= currentThirst) { deathManager.Die(gameObject, CauseOfDeath.THIRST); }

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
                if (this.gameObject.CompareTag("Fox")) { targetTag = "Hen"; }
                else targetTag = "BushResource";
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
        // Como la lista esta ordenada, solo hay que comprobar si el primero esta en rango o no
        if (allTargets.Count == 0)
        {
            Explore();
        }
        else if (allTargets[0] == null || (allTargets[0].transform.position - transform.position).sqrMagnitude > (senseRangeSquare))
        {
            Explore();
        }
        else
        {
            randomPointSetted = false;
            nearestTarget = allTargets[0];

            if (nearestTarget == null || nearestTarget.CompareTag("Growing"))
            {
                StartCoroutine(Awaiter(waitTime));
            }
            else if (nearestTarget.CompareTag("Water"))
            {
                if (navMeshAgent.isOnNavMesh)
                {
                    Vector3 targetClosestPoint = nearestTarget.GetComponent<Collider>().ClosestPoint(this.transform.position);
                    navMeshAgent.SetDestination(targetClosestPoint);
                }
            }
            else if (nearestTarget.CompareTag("BushResource") || nearestTarget.CompareTag("Hen") || this.gameObject.CompareTag(nearestTarget.tag)) //Si es un bush, no importa, ya que se detiene al colisionar
            {
                if (navMeshAgent.isOnNavMesh)
                {
                    Vector3 targetPosition = nearestTarget.transform.position;
                    navMeshAgent.SetDestination(targetPosition);
                }
            }
        }
    }

    void GetOrderedTargets()
    {
        if (targetTag == "Hen") { this.allTargets = this.simulationManager.hensList.OrderBy(o => (o.transform.position - transform.position).sqrMagnitude).ToList(); }
        else if (targetTag == "Fox") { this.allTargets = this.simulationManager.foxesList.OrderBy(o => (o.transform.position - transform.position).sqrMagnitude).ToList(); }
        else if (targetTag == "BushResource") { this.allTargets = this.simulationManager.bushesList.OrderBy(o => (o.transform.position - transform.position).sqrMagnitude).ToList(); }
        else if (targetTag == "Water") { this.allTargets = this.simulationManager.waterTiles.OrderBy(o => (o.transform.position - transform.position).sqrMagnitude).ToList(); }

        if (this.gameObject.CompareTag(targetTag))
        {
            this.allTargets.Remove(this.gameObject);
        }
        //SetTarget();
    }

    //Función principal de espera tras una accion, que devuelve al animal al ciclo de busqueda de un ojbetivo
    IEnumerator Awaiter(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        this.nearestTarget = null;
    }

    void Eat(Plant plant)
    {
        if (this.transform == null) { return; }
        navMeshAgent.SetDestination(this.transform.position);
        //Si se encuentra con una planta, hay que decirle al navigation que el nuevo objetivo es la posición actual, para que no se coloque en el centro de la planta:
        //nearestTarget = this.gameObject;

        if (plant.IsEdible() && this.currentHunger > 1)
        {
            plant.Consume(energyRestoreValue);
            this.currentHunger -= energyRestoreValue * Time.timeScale;
        }
        else
        {
            //Cuando termine de comer, pasa a la accion IDLE para que en el ciclo pueda entrar en el if que establece la accion.
            this.currentAction = Actions.IDLE;
        }

    }

    void Drink()
    {
        if(this.transform == null) { return; }
        navMeshAgent.SetDestination(this.transform.position);
        //Si se encuentra con una planta, hay que decirle al navigation que el nuevo objetivo es la posición actual, para que no se coloque en el centro de la planta:
        //nearestTarget = this.gameObject;
        if (currentThirst > 1)
        {
            currentThirst -= energyRestoreValue * 2 * Time.timeScale;
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
            this.reproduceParticleSystem.Play();
        }
        if (currentReproduceUrge > 1)
        {
            currentReproduceUrge -= (energyRestoreValue * 10) * Time.timeScale;
        }
        else
        {
            currentAction = Actions.IDLE;

            //Espera 2 segs y crea al nuevo animal
            Invoke(nameof(SpawnAnimal), 0f);            
        }
    }

    void SpawnAnimal()
    {
        Vector3 spawnPos = new Vector3(transform.position.x, transform.position.y, transform.position.z); //Position i want to spawn it

        NavMeshHit closestHit;
        if(NavMesh.SamplePosition(spawnPos, out closestHit, 100, 1))
        {
            GameObject newAnimal = Instantiate(this.gameObject, closestHit.position, Quaternion.identity, animalContainer.transform);
            newAnimal.GetComponent<Animal>().SetDeathManager(deathManager);
            newAnimal.GetComponent<Animal>().SetSimulationManager(simulationManager);

            if (newAnimal.CompareTag("Hen")) { simulationManager.AddHen(newAnimal); }
            else { simulationManager.AddFox(newAnimal); }

            float newSpeed = (this.speed + this.mate.speed + Random.Range(-0.5f, 0.5f)) / 2f;

            newAnimal.GetComponentInChildren<Animal>().speed = newSpeed;

            SetColor(newAnimal, newSpeed);
        } else { Debug.Log("Algo ha fallado al crear animal nuevo"); }
    }

    void SetColor(GameObject animal, float speed)
    {
        Color newColor;
        float dif;
        if (this.CompareTag("Fox"))
        {
            dif = (speed - 4f) * 0.5f;

            newColor = new Color(0.8867924f, 0.3650719f, 0.05437878f); // color base del zorro
        }
        else
        {
            dif = (speed - 2f) * 0.75f;
            newColor = new Color(1, 1, 1); // color base de la gallina
        }

        if (dif > 0f)
        {
            //Si es mayor, significa que es mas rapido, luego hay que bajar los valores de green y blue para que sea mas rojo (el zorro sera más amarillo para tener mayor margen de cambio)
            if (this.CompareTag("Fox"))
            {
                newColor.g += dif;
                newColor.b -= dif;
            }
            else
            {
                newColor.g -= dif;
                newColor.b -= dif;
            }
        }
        else
        {
            //Si es menor, entonces hay que bajar el valor de red para que sea mas azul
            newColor.r += dif;
        }

        animal.GetComponent<Animal>().SetModelColor(newColor);
    }

    public GameObject GetModel()
    {
        return this.model;
    }

    void SetModelColor(Color color)
    {
        this.model.GetComponent<Renderer>().material.color = color;
    }

    void Explore()
    {
        if (!randomPointSetted)
        {
            //Genera un punto aleatorio dentro del NavMesh
            randomPoint = RandomNavMeshPoint();
            if (navMeshAgent.isOnNavMesh)
            {
                navMeshAgent.SetDestination(randomPoint);
            } 
            randomPointSetted = true;
        }
        if ((transform.position - randomPoint).sqrMagnitude <= 1)
        {
            randomPointSetted = false;
        }
        else if ((transform.position - destino).sqrMagnitude <= 1)
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
        if (other.gameObject.CompareTag("Water") && currentAction == Actions.SEARCHING_WATER)
        {
            currentAction = Actions.DRINKING;
        }

        if (gameObject.CompareTag("Hen") && other.gameObject.CompareTag("Food") && currentAction == Actions.SEARCHING_FOOD)
        {
            //Como por ahora el unico tipo de comida es Plant, si el tag es food sabemos que es una planta, por lo que obtenemos el componente del padre del gameObject
            this.plantTarget = other.GetComponentInParent<Plant>(); //Guardamos la planta objetivo
            currentAction = Actions.EATING; //Actualizamos la accion
        } 

        if (this.gameObject.CompareTag(other.gameObject.tag) && currentAction == Actions.SEARCHING_MATE) // && other.GetComponent<Animal>().GetCurrentAction() == Actions.SEARCHING_MATE
        {
            this.mate = other.gameObject.GetComponentInChildren<Animal>();
            currentAction = Actions.MATING;
        }
        if (other.gameObject.CompareTag("RandomPoint"))
        {
            Destroy(other.gameObject);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (this.gameObject.CompareTag("Fox"))
        {
            if (other.gameObject.CompareTag("Hen") && currentAction == Actions.SEARCHING_FOOD)
            {
                deathManager.Die(other.gameObject, CauseOfDeath.DEVOURED);
                nearestTarget = null;
                this.currentHunger = 3; //el zorro satisface su hambre si se come una gallina
            }
        }
        OnTriggerEvent(other);
    }

    public void OnTriggerStay(Collider other)
    {
        OnTriggerEvent(other);
    }


    void SetAnimation()
    {
        if(this.CompareTag("Fox")) { return; }
        if (currentAction == Actions.SEARCHING_WATER || currentAction == Actions.SEARCHING_FOOD || currentAction == Actions.SEARCHING_MATE || currentAction == Actions.FLEEING)
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
    FLEEING
}

