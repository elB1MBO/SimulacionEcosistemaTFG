using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PopulationUI : MonoBehaviour
{
    public Label henPopulationNumber; 
    // Start is called before the first frame update
    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        henPopulationNumber = root.Q<Label>("HenPopulationNumber");
    }

    // Update is called once per frame
    void Update()
    {
        this.henPopulationNumber.text = GameObject.FindGameObjectsWithTag("Hen").Length.ToString();
    }
}
