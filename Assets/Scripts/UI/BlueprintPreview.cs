using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BlueprintPreview : MonoBehaviour
{
    private GameObject container;
    private GameObject simulatorManager;

    RaycastHit hit;
    public GameObject prefab;
    private DeathManager deathManager;
    private Simulation simulation;

    // Start is called before the first frame update
    void Start()
    {
        simulatorManager = GameObject.FindGameObjectWithTag("SimulatorManager");
        deathManager = simulatorManager.GetComponent<Simulation>().deathManager;
        simulation = this.simulatorManager.GetComponent<Simulation>();

        switch (prefab.tag)
        {
            case "Hen":
                this.container = simulation.HenContainer;
                break;
            case "Fox":
                this.container = simulation.FoxContainer;
                break;
            case "BushResource":
                this.container = simulation.BushContainer;
                break;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        if (Physics.Raycast(ray, out hit, 50000.0f, (1 << 0)))
        {
            transform.position = hit.point;
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (prefab.tag)
        {
            case "Hen":
                this.container = simulation.HenContainer;
                break;
            case "Fox":
                this.container = simulation.FoxContainer;
                break;
            case "BushResource":
                this.container = simulation.BushContainer;
                break;
        }

        //Para evitar que se generen si se clicka sobre un boton o algo que no sea el terreno
        if (EventSystem.current.IsPointerOverGameObject()) { return; }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        if (Physics.Raycast(ray, out hit, 50000.0f, (1 << 0)))
        {
            transform.position = hit.point;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (transform.position.y < 5)
            {
                if (prefab.CompareTag("BushResource"))
                {
                    GameObject bush = Instantiate(prefab, transform.position, Quaternion.identity, this.container.transform);
                    simulation.AddBush(bush);
                }
                else
                {
                    GameObject newAnimal = Instantiate(prefab, transform.position, transform.rotation, this.container.transform);
                    newAnimal.GetComponent<Animal>().SetAnimalContainer(container);
                    newAnimal.GetComponent<Animal>().SetDeathManager(deathManager);
                    newAnimal.GetComponent<Animal>().SetSimulationManager(simulation);

                    if (newAnimal.CompareTag("Hen")) { simulation.AddHen(newAnimal); }
                    else { simulation.AddFox(newAnimal); }
                }       
            }
            else { Debug.LogWarning("No puedes colocarlo tan alto"); }
        }
    }
}
