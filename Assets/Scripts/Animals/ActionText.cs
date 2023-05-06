using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionText : MonoBehaviour
{
    public Text text;

    [SerializeField] private BasicAnimal hen;

    private void Start()
    {
        text.text = "Idle";
    }

    private void Update()
    {
        // cambiar texto basico
        if (hen.getHungry() > 45)
        {
            text.text = "Searching for food";
        }
    }
}
