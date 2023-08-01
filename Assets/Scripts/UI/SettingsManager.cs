using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public Toggle toggle;
    // Start is called before the first frame update
    void Start()
    {
        if (Screen.fullScreen)
        {
            toggle.isOn = true;
        }
        else
        {
            toggle.isOn = false;
        }        
    }

    public void OpenMenu()
    {
        gameObject.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Resume()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1f;
    }

    public void ToggleFullscreen(bool toggleValue)
    {
        Screen.fullScreen = toggleValue;
    }

}
