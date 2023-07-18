using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseResumeButtons : MonoBehaviour
{
    public Sprite pausedSprite;
    public Sprite resumeSprite;
    private Image buttonImage;

    private void Start()
    {
        buttonImage = GetComponent<Image>();
    }

    private void Update()
    {
        if(Time.timeScale == 0)
        {
            buttonImage.sprite = resumeSprite;
        }
        else
        {
            buttonImage.sprite = pausedSprite;
        }
    }

    public void ChangeTimeOnClick()
    {
        if (Time.timeScale == 0)
        {
            Time.timeScale = 1;
        }
        else
        {
            Time.timeScale = 0;
        }
    }
}
