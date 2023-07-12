using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PopulationUI : MonoBehaviour
{
    public Label henPopulationNumber;
    public Label foxPopulationNumber;
    public Label averageSpeed;

    [SerializeField] GameObject simulator; 
    // Start is called before the first frame update
    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        henPopulationNumber = root.Q<Label>("HenPopulationNumber");
        foxPopulationNumber = root.Q<Label>("FoxPopulationNumber");
        averageSpeed = root.Q<Label>("AverageSpeed");
    }

    // Update is called once per frame
    void Update()
    {
        this.henPopulationNumber.text = GameObject.FindGameObjectsWithTag("Hen").Length.ToString();
        this.foxPopulationNumber.text = GameObject.FindGameObjectsWithTag("Fox").Length.ToString();
        this.averageSpeed.text = simulator.GetComponent<Simulation>().averageHenSpeed.ToString();
    }
}
