using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class PopulationUI : MonoBehaviour
{
    public TextMeshProUGUI henPopulationNumber;
    public TextMeshProUGUI foxPopulationNumber;
    public TextMeshProUGUI averageHenSpeed;
    public TextMeshProUGUI averageFoxSpeed;

    [SerializeField] GameObject simulator; 
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("UpdateValues", 0f, 1f);
    }

    // Update is called once per frame
    void UpdateValues()
    {
        this.henPopulationNumber.text = GameObject.FindGameObjectsWithTag("Hen").Length.ToString();
        this.foxPopulationNumber.text = GameObject.FindGameObjectsWithTag("Fox").Length.ToString();
        this.averageHenSpeed.text = simulator.GetComponent<Simulation>().averageHenSpeed.ToString();
        this.averageFoxSpeed.text = simulator.GetComponent<Simulation>().averageFoxSpeed.ToString();
    }
}
