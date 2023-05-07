using AnimalNamespace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionText : MonoBehaviour
{
    public Text text;

    [SerializeField] private Animal animal;

    private Actions currentAction;

    private void Start()
    {
        currentAction = animal.getCurrentAction();
    }

    private void Update()
    {
        text.text = currentAction.ToString();
    }
}
