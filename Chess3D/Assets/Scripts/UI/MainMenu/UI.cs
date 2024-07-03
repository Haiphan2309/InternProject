using DG.Tweening;
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

    private float sliderPositionFullHeight = 550f;
    private float sliderPositionHalfHeight = 350f;
    private float sliderOffset = -50f;

    public virtual void Preset()
    {
        title = UIManager.Instance.textSystem.GetChild(0).GetComponent<RectTransform>();

        topSlider = UIManager.Instance.backgroundSystem.GetChild(1).GetComponent<RectTransform>();
        topChessHolder = topSlider.GetChild(0).GetComponent<RectTransform>();
        topChessContainer = topChessHolder.GetChild(1).GetComponent<RectTransform>();

        bottomSlider = UIManager.Instance.backgroundSystem.GetChild(2).GetComponent<RectTransform>();
        bottomChessHolder = bottomSlider.GetChild(0).GetComponent<RectTransform>();
        bottomChessContainer = bottomChessHolder.GetChild(1).GetComponent<RectTransform>();

        startButton = UIManager.Instance.buttonSystem.GetChild(0).GetComponent<RectTransform>();
        settingButton = UIManager.Instance.buttonSystem.GetChild(1).GetComponent<RectTransform>();
        returnButton = UIManager.Instance.buttonSystem.GetChild(2).GetComponent<RectTransform>();
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

    public void ShowSliderFullHeight(RectTransform slider)
    {
        RectTransform topBG = slider.Find("BG") as RectTransform;
        RectTransform topOverlay = slider.Find("Overlay") as RectTransform;
        topBG.DOAnchorPosY(sliderPositionFullHeight, _timer).SetEase(Ease.OutBack);
        topOverlay.DOAnchorPosY(sliderPositionFullHeight + sliderOffset, _timer).SetEase(Ease.OutBack);
    }

    public void ShowSliderHalfHeight(RectTransform slider)
    {
        RectTransform topBG = slider.Find("BG") as RectTransform;
        RectTransform topOverlay = slider.Find("Overlay") as RectTransform;
        topBG.DOAnchorPosY(sliderPositionFullHeight, _timer).SetEase(Ease.OutBack);
        topOverlay.DOAnchorPosY(sliderPosition + sliderOffset, _timer).SetEase(Ease.OutBack);
    }
}
