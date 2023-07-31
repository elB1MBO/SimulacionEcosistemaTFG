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
    public TextMeshProUGUI thirstDeaths;
    public TextMeshProUGUI starvationDeaths;
    public TextMeshProUGUI devouredDeaths;


    [SerializeField] private Simulation simulator;
    [SerializeField] private DeathManager deathManager;
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating(nameof(UpdateValues), 0f, 1f);
    }

    // Update is called once per frame
    void UpdateValues()
    {
        this.henPopulationNumber.text = GameObject.FindGameObjectsWithTag("Hen").Length.ToString();
        this.foxPopulationNumber.text = GameObject.FindGameObjectsWithTag("Fox").Length.ToString();
        this.averageHenSpeed.text = simulator.averageHenSpeed.ToString();
        this.averageFoxSpeed.text = simulator.averageFoxSpeed.ToString();
        this.thirstDeaths.text = deathManager.GetDeathsByThirst().ToString();
        this.starvationDeaths.text = deathManager.GetDeathsByStarvation().ToString();
        this.devouredDeaths.text = deathManager.GetDeathsByDevoured().ToString();
    }
}
