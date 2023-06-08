using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionText : MonoBehaviour
{
    public Text text;

    [SerializeField] private Animal animal;

    private void Update()
    {
        text.text = animal.GetCurrentAction().ToString();
    }
}
