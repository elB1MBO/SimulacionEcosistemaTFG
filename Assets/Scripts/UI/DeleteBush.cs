using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DeleteBush : MonoBehaviour
{
    [SerializeField] private string selectableTag;
    [SerializeField] private Material highlightedMaterial;
    [SerializeField] private Material defaultMaterial;
    Ray ray;
    RaycastHit hit;
    Renderer selectedRenderer;

    [SerializeField] private GameObject deleteButton;
    public bool isEnabled = false;

    public GameObject canvas;

    [SerializeField] private Simulation simulationManager;

    public void EnableDelete()
    {
        if (!isEnabled)
        {
            deleteButton.GetComponent<Button>().Select();
            deleteButton.GetComponent<Image>().color = Color.yellow;
            isEnabled = true;
            DisableCanvasButton();
        }
        else
        {
            Disable();
        }
    }

    public void Disable()
    {
        EventSystem.current.SetSelectedGameObject(null);
        deleteButton.GetComponent<Image>().color = Color.white;
        isEnabled = false;
    }

    public void DisableCanvasButton()
    {
        canvas.GetComponent<SpawnBlueprint>().DisableAll();
    }

    private Transform selection;
    void Update()
    {
        if(isEnabled)
        {
            if (selection != null)
            {
                selection.gameObject.GetComponent<Renderer>().material = defaultMaterial;
                selection = null;
            }

            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 50000.0f, (1 << 0)))
            {
                if (hit.transform.CompareTag(selectableTag))
                {
                    selectedRenderer = hit.transform.gameObject.GetComponent<Renderer>();
                    if (selectedRenderer != null)
                    {
                        selectedRenderer.material = highlightedMaterial;
                        if (Input.GetMouseButtonDown(0))
                        {
                            simulationManager.RemoveBush(hit.transform.parent.gameObject);
                            Destroy(hit.transform.parent.gameObject);
                        }
                    }
                    selection = hit.transform;
                }
            }
        }
    }
}
