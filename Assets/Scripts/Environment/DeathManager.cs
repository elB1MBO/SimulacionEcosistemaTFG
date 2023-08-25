using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DeathManager : MonoBehaviour
{
    [SerializeField] private int thirst;
    [SerializeField] private int starvation;
    [SerializeField] private int devoured;
    [SerializeField] private Simulation simulationManager;

    private void Start()
    {
        thirst = 0;
        starvation = 0;
        devoured = 0;
    }

    public int GetDeathsByThirst() { return thirst; }
    public int GetDeathsByStarvation() { return starvation; }
    public int GetDeathsByDevoured() { return devoured; }

    public void Die(GameObject animal, CauseOfDeath causeOfDeath)
    {
        Destroy(animal);
        if (animal.CompareTag("Hen")) { simulationManager.RemoveHen(animal); }
        else{ simulationManager.RemoveFox(animal); }
        switch (causeOfDeath)
        {
            case CauseOfDeath.THIRST: thirst++; break;
            case CauseOfDeath.STARVATION: starvation++; break;
            case CauseOfDeath.DEVOURED: devoured++; break;
        }
    }

    public void FallDie(GameObject animal)
    {
        Destroy(animal);
        if(animal.CompareTag("Hen")) { simulationManager.RemoveHen(animal); }
        else { simulationManager.RemoveFox(animal); }
    }
}

