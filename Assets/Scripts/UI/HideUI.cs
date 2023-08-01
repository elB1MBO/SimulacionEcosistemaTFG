using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HideUI : MonoBehaviour
{
    public GameObject canvas;
    public Sprite hiddenSprite;
    public Sprite visibleSprite;
    private Image buttonImage;

    private void Start()
    {
        buttonImage = GetComponent<Image>();
    }

    public bool isHidden = false;
    public void HideAndShow()
    {
        if (!isHidden)
        {
            canvas.SetActive(false);
            buttonImage.sprite = hiddenSprite;
            isHidden = true;
        }
        else
        {
            canvas.SetActive(true);
            buttonImage.sprite = visibleSprite;
            isHidden = false;
        }
    }
}
