using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SpawnBlueprint : MonoBehaviour
{
    public GameObject henBlueprint;
    public GameObject foxBlueprint;

    public GameObject henb;
    public GameObject foxb;

    private bool henButtonActivated = false;
    private bool foxButtonActivated = false;

    public GameObject henButton;
    public GameObject foxButton;

    public void SpawnHenBlueprints()
    {
        if (!henButtonActivated)
        {
            henb = Instantiate(henBlueprint);
            henButtonActivated = true;
            foxButtonActivated = false;
            if (foxb) { Destroy(foxb); }
        }
        else 
        { 
            henButtonActivated = false;
            Destroy(henb);
        }
    }
    public void SpawnFoxBlueprints()
    {
        if (!foxButtonActivated)
        {
            foxb = Instantiate(foxBlueprint);
            foxButtonActivated = true;
            henButtonActivated = false;
            if(henb) { Destroy(henb); }
        }
        else 
        { 
            foxButtonActivated = false; 
            Destroy(foxb);
        }
    }

    public void Update()
    {
        if (henButtonActivated)
        {
            henButton.GetComponent<Button>().Select();
        }
        else
        {
            EventSystem.current.SetSelectedGameObject(null);
        }

        if (foxButtonActivated)
        {
            foxButton.GetComponent<Button>().Select();
        }
        else
        {
            EventSystem.current.SetSelectedGameObject(null);
        }

    }
}
