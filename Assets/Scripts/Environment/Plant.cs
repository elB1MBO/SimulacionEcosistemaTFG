using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour
{
    // cantidad de comida que tiene aún el objeto
    [SerializeField] public float foodAmount;
    // si llega a 0, habrá que esperar a que crezca para que vuelva a ser comestible
    [SerializeField] private bool edible;
    [SerializeField] private int hensEating;

    public bool IsEdible() { return edible; }

    // Start is called before the first frame update
    void Start()
    {
        foodAmount = 100;
        edible = true;
        hensEating = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Comprueba en cada frame la cantidad de comida que tiene el objeto
        //CheckHensEating();
        CheckFoodAmount();
    }
    //public void CheckHensEating()
    //{
    //    if(hensEating > 5) 
    //    { 
    //        //Debug.Log("Limite superado"); 
    //    }
    //}
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
        foodAmount -= amount/4;
    }

    public void Restore()
    {
        foodAmount += 0.05f;
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
