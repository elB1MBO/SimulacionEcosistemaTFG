using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueprintPreview : MonoBehaviour
{
    private GameObject container;
    private GameObject simulatorManager;

    RaycastHit hit;
    Vector3 movePoint;
    public GameObject prefab;

    // Start is called before the first frame update
    void Start()
    {

        simulatorManager = GameObject.FindGameObjectWithTag("SimulatorManager");

        switch (prefab.gameObject.tag)
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
        switch (prefab.gameObject.tag)
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

        if (Input.GetMouseButtonDown(0))
        {
            if (transform.position.y < 5)
            {
                GameObject newAnimal = Instantiate(prefab, transform.position, transform.rotation, this.container.transform);
                newAnimal.GetComponent<Animal>().SetAnimalContainer(container);
                Destroy(gameObject);
            }
            else { Debug.LogWarning("No puedes colocarlo sobre un �rbol"); }
            //Instantiate(prefab, transform.position, transform.rotation);
        }
    }
}
