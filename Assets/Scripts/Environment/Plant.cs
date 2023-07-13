using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour
{
    // cantidad de comida que tiene aún el objeto
    [SerializeField] public float foodAmount = 100.0f;
    // si llega a 0, habrá que esperar a que crezca para que vuelva a ser comestible
    [SerializeField] private bool edible = true;
    [SerializeField] private int hensEating = 0;

    public bool IsEdible() { return edible; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Comprueba en cada frame la cantidad de comida que tiene el objeto
        CheckHensEating();
        CheckFoodAmount();
    }
    public void CheckHensEating()
    {
        if(hensEating > 5) 
        { 
            //Debug.Log("Limite superado"); 
        }
    }
    public void CheckFoodAmount()
    {
        if (foodAmount <= 5 || edible == false)
        {
            this.gameObject.transform.GetChild(0).tag = "Growing";
            edible = false; //cuando llega a 0, la marca como no comestible
            Restore();
        }
        else if(foodAmount < 100 && edible)
        {
            SetScale();
        }
    }

    public void Consume(float amount)
    {
        foodAmount -= amount/2;
    }

    public void Restore()
    {
        foodAmount += 0.01f;
        SetScale();
        if(foodAmount >= 100) {  // cuando llega a 100, la vuelve a marcar como comestible
            edible = true;
            this.gameObject.transform.GetChild(0).tag = "Food";
        } 
    }

    public void SetScale()
    {
        // conforme cambia el valor de comida del objeto, se modifica su tamaño (escala):
        // foodAmount = 100 -> scale = 1
        // foodAmount = 99 -> scale = 0.99
        float scale = foodAmount / 100;
        transform.localScale = new Vector3(scale, scale, scale);
    }

    public void OnBushCollision(int isHen) //1 for enter hen, -1 when exits a hen
    {
        hensEating+=isHen;
    }

}
