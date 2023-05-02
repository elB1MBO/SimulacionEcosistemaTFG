using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAnimal : MonoBehaviour
{
    [SerializeField] private float maxValue = 100;
    
    private float currentHungry;
    private float currentThirsty;
    private float currentReproduceUrge;

    [SerializeField] private BasicBar hungryBar;
    [SerializeField] private BasicBar thirstyBar;
    [SerializeField] private BasicBar reproduceUrgeBar;

    private void Start()
    {
        currentHungry = 1;
        currentThirsty = 1;
        currentReproduceUrge = 1;

        hungryBar.UpdateValueBar(maxValue, currentHungry);
        thirstyBar.UpdateValueBar(maxValue, currentThirsty);
        reproduceUrgeBar.UpdateValueBar(maxValue, currentReproduceUrge);
    }

    void Update()
    {
        currentReproduceUrge += 0.01f;
        reproduceUrgeBar.UpdateValueBar(maxValue, currentReproduceUrge);

        currentHungry += 0.02f;
        currentThirsty += 0.01f;

        if (currentHungry >= maxValue || currentThirsty >= maxValue)
        {
            Destroy(gameObject);
        }
        else
        {
            hungryBar.UpdateValueBar(maxValue, currentHungry);
            thirstyBar.UpdateValueBar(maxValue, currentThirsty);
        }
    }
}
