using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SpawnBlueprint : MonoBehaviour
{
    public GameObject henBlueprint;
    public GameObject foxBlueprint;
    public GameObject bushBlueprint;

    public GameObject henb;
    public GameObject foxb;
    public GameObject bushb;

    private bool henButtonActivated = false;
    private bool foxButtonActivated = false;
    private bool bushButtonActivated = false;

    public GameObject henButton;
    public GameObject foxButton;
    public GameObject bushButton;

    public void SpawnHenBlueprints()
    {
        if (!henButtonActivated)
        {
            henb = Instantiate(henBlueprint);
            henButtonActivated = true;
            foxButtonActivated = false;
            bushButtonActivated = false;
            if (foxb) { Destroy(foxb); }
            else if(bushb) { Destroy(bushb); }
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
            bushButtonActivated = false;
            if(henb) { Destroy(henb); }
            else if (bushb) { Destroy(bushb); }
        }
        else 
        { 
            foxButtonActivated = false; 
            Destroy(foxb);
        }
    }

    public void SpawnBushBlueprints()
    {
        if (!bushButtonActivated)
        {
            bushb = Instantiate(bushBlueprint);
            bushButtonActivated = true;
            henButtonActivated = false;
            foxButtonActivated = false;
            if (henb) { Destroy(henb); }
            else if (foxb) { Destroy(foxb); }
        }
        else
        {
            bushButtonActivated = false;
            Destroy(bushb);
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

        if (bushButtonActivated)
        {
            bushButton.GetComponent<Button>().Select();
        }
        else
        {
            EventSystem.current.SetSelectedGameObject(null);
        }

    }
}
