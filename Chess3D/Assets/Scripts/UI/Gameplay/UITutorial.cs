using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITutorial : MonoBehaviour
{
    [SerializeField] UIPopupAnim uiPopupAnim;
    [SerializeField] Image tutorialImage;

    public void Show(Sprite tutorialSprite)
    {
        gameObject.SetActive(true);
        uiPopupAnim.Show();
        tutorialImage.sprite = tutorialSprite;
    }
}
