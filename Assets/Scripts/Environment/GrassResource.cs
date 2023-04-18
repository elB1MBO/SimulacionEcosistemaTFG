using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassResource : MonoBehaviour
{
    // cantidad de comida que tiene aún el objeto
    [SerializeField] public float foodAmount = 100.0f;
    // si llega a 0, habrá que esperar a que crezca para que vuelva a ser comestible
    [SerializeField] public bool edible = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Comprueba en cada frame la cantidad de comida que tiene el objeto
        CheckFoodAmount();
        CheckInput();
    }

    public void CheckFoodAmount()
    {
        if (foodAmount <= 5 || edible == false)
        {
            edible = false; //cuando llega a 0, la marca como no comestible
            Debug.Log("Ahora crece");
            Restore();
        }
        else if(foodAmount < 100 && edible)
        {
            SetScale();
        }
    }

    public void Restore()
    {
        foodAmount += 0.05f;
        SetScale();
        if(foodAmount >= 100) { edible = true; } // cuando llega a 100, la vuelve a marcar como comestible
    }

    public void SetScale()
    {
        // conforme cambia el valor de comida del objeto, se modifica su tamaño (escala):
        // foodAmount = 100 -> scale = 1
        // foodAmount = 99 -> scale = 0.99
        float scale = foodAmount / 100;
        transform.localScale = new Vector3(scale, scale, scale);
    }

    //Para comprobar la funcionalidad del código, simularemos que esta siendo comida al presionar una tecla
    public void CheckInput()
    {
        if(Input.GetKey(KeyCode.E) && edible) {
            foodAmount -= 0.1f;
        }
    }
}
