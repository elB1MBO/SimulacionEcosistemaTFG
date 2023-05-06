using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BasicBar : MonoBehaviour
{

    [SerializeField] private Image barSprite;
    
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        //transform.rotation = Quaternion.LookRotation(transform.position - cam.transform.position);
    }

    public void UpdateValueBar(float maxValue, float currentValue)
    {
        barSprite.fillAmount = currentValue / maxValue;
    }
}
