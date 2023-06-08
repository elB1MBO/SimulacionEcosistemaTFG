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

    public NavMeshAgent navMeshAgent;

    [SerializeField] private Actions currentAction = Actions.IDLE;

    [SerializeField] private GameObject animalContainer;
    public void SetAnimalContainer(GameObject animalContainer) { this.animalContainer = animalContainer; }

    //Properties
    private float maxPropValue = 100;

    private float currentHungry = 1;
    private float currentThirsty = 1;
    private float currentReproduceUrge = 1;

    [SerializeField] private BasicBar hungryBar;
    [SerializeField] private BasicBar thirstyBar;
    [SerializeField] private BasicBar reproduceUrgeBar;

    public float GetHungry() { return currentHungry; }
    public float GetThirsty() { return currentThirsty; }
    public float GetReproduceUrge() { return currentReproduceUrge; }
    public Actions GetCurrentAction() { return currentAction; }

    Animator henAnimation;

    // Start is called before the first frame update
    void Start()
    {
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
            if (nearestTarget.tag == "Growing") //Más tarde habra que ampliar esta funcionalidad para cuando el objetivo deje de existir
            {
                //Como su objetivo ya no esta disponible, tiene que buscar otro
                StartCoroutine(Awaiter(waitTime)); 
            }
            if (nearestTarget.tag == "Water")
            {
                Vector3 targetClosestPoint = nearestTarget.GetComponent<Collider>().ClosestPoint(this.transform.position);
                navMeshAgent.SetDestination(targetClosestPoint);
            }else if(nearestTarget.tag == "Food") //Si es un bush, no importa, ya que se detiene al colisionar
            {
                Vector3 targetPosition = nearestTarget.transform.position;
                navMeshAgent.SetDestination(targetPosition);
            } else if(nearestTarget.tag == this.gameObject.tag)
            {
                Vector3 targetPosition = nearestTarget.transform.position;
                navMeshAgent.SetDestination(targetPosition);
            }
        }

        //------------------------------Animal-----------------------------------
        UpdateValues();

        SetAction();

        SetAnimation();
    }

    void UpdateValues()
    {
        if (currentAction == Actions.EATING)
        {
            Eat(this.plantTarget);
        }
        else if (currentAction == Actions.DRINKING)
        {
            Drink();
        }
        else if(currentAction == Actions.MATING)
        {
            Reproduce();
        }
        else
        {
            currentReproduceUrge += 0.015f;

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
        Actions action = currentAction;
        if (currentAction != Actions.EATING && currentAction != Actions.DRINKING && currentAction != Actions.MATING) //Acciones que no se pueden interrumpir
        {
            if(currentReproduceUrge > 40 && currentHungry < 40 && currentThirsty < 40)
            {
                Debug.Log("Ahora toca reproducirse");
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
            case Actions.SEARCHING_MATE: targetTag = this.gameObject.tag; break;
            default: targetTag = ""; break;
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

    //Función principal de espera tras una accion, que devuelve al animal al ciclo de busqueda de un ojbetivo
    IEnumerator Awaiter(float seconds)
    {
        yield return new WaitForSeconds(seconds);
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

        if (plant.IsEdible() && this.currentHungry > 1)
        {
            plant.Consume(0.02f);
            this.currentHungry -= 0.04f;
        }
        else
        {
            //Cuando termine de comer, pasa a la accion IDLE para que en el ciclo pueda entrar en el if que establece la accion.
            this.currentAction = Actions.IDLE;
            //StartCoroutine(Awaiter(waitTime));
        }

    }

    void Drink()
    {
        if (currentThirsty > 1)
        {
            currentThirsty -= 0.04f;
        }
        else
        {
            currentAction = Actions.IDLE;
            //StartCoroutine(Awaiter(waitTime));
        }
    }
    
    void Reproduce()
    {
        if(currentReproduceUrge > 1)
        {
            currentReproduceUrge -= 1;
            Debug.Log("FOLLANDOOO");
        }
        else
        {
            currentAction = Actions.IDLE;
            Instantiate(this.gameObject, gameObject.transform.position, Quaternion.identity, animalContainer.transform);
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
        if(other.gameObject.tag == "Hen" && currentAction == Actions.SEARCHING_MATE)
        {
            currentAction = Actions.MATING;
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
        if (currentAction == Actions.EATING || currentAction == Actions.DRINKING || currentAction == Actions.MATING)
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
    SEARCHING_MATE,
    MATING,
}

