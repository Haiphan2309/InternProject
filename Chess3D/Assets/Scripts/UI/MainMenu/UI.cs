using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UI : MonoBehaviour
{
    public RectTransform title;

    public RectTransform topSlider;
    public RectTransform topChessHolder;
    public RectTransform topChessContainer;

    public RectTransform bottomSlider;
    public RectTransform bottomChessHolder;
    public RectTransform bottomChessContainer;

    public RectTransform startButton;
    public RectTransform settingButton;
    public RectTransform returnButton;

    public float _timer = 1f;
    public bool isLoaded = false;

    public virtual void Preset()
    {
        title = UIManager.Instance.textSystem.Find("Title") as RectTransform;

        topSlider = UIManager.Instance.backgroundSystem.Find("TopSlider") as RectTransform;
        topChessHolder = topSlider.Find("ChessHolder") as RectTransform;
        topChessContainer = topChessHolder.Find("ChessContainer") as RectTransform;

        bottomSlider = UIManager.Instance.backgroundSystem.Find("BottomSlider") as RectTransform;
        bottomChessHolder = bottomSlider.Find("ChessHolder") as RectTransform;
        bottomChessContainer = bottomChessHolder.Find("ChessContainer") as RectTransform;

        startButton = UIManager.Instance.buttonSystem.Find("StartButton") as RectTransform;
        settingButton = UIManager.Instance.buttonSystem.Find("SettingButton") as RectTransform;
        returnButton = UIManager.Instance.buttonSystem.Find("ReturnButton") as RectTransform;
    }

    public virtual void Anim()
    {
        Preset();
        DisableButton();
        StartCoroutine(Cor_Anim());
    }

    public virtual IEnumerator Cor_Anim()
    {
        yield return null;
    }
    public void EnableButton()
    {
        startButton.GetComponent<Button>().interactable = true;
        settingButton.GetComponent<Button>().interactable = true;
        returnButton.GetComponent<Button>().interactable = true;
    }

    public void DisableButton()
    {
        startButton.GetComponent<Button>().interactable = false;
        settingButton.GetComponent<Button>().interactable = false;
        returnButton.GetComponent<Button>().interactable = false;
    }
}
