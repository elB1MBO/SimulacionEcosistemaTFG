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

    // Start is called before the first frame update
    void Start()
    {

        simulatorManager = GameObject.FindGameObjectWithTag("SimulatorManager");

        switch (prefab.tag)
        {
            case "Hen":
                this.container = this.simulatorManager.GetComponent<Simulation>().HenContainer;
                break;
            case "Fox":
                this.container = this.simulatorManager.GetComponent<Simulation>().FoxContainer;
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
                this.container = this.simulatorManager.GetComponent<Simulation>().HenContainer;
                break;
            case "Fox":
                this.container = this.simulatorManager.GetComponent<Simulation>().FoxContainer;
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
                GameObject newAnimal = Instantiate(prefab, transform.position, transform.rotation, this.container.transform);
                newAnimal.GetComponent<Animal>().SetAnimalContainer(container);
            }
            else { Debug.LogWarning("No puedes colocarlo tan alto"); }
        }
    }
}
