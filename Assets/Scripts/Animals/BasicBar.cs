using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BasicBar : MonoBehaviour
{

    [SerializeField] private Image barSprite;

    public void UpdateValueBar(float maxValue, float currentValue)
    {
        barSprite.fillAmount = currentValue / maxValue;
    }
}
