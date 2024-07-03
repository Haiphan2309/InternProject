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

    public RectTransform bottomSlider;
    public RectTransform bottomChessHolder;

    public RectTransform startButton;
    public RectTransform settingButton;
    public RectTransform returnButton;

    public float _timer = 1f;
    public bool isLoaded = false;

    private float sliderPositionFullHeight = 550f;
    private float sliderPositionHalfHeight = 350f;
    private float sliderOffset = -50f;

    private float chessHolderPosition = 500f;
    private float chessHolderScale = 20;

    public virtual void Preset()
    {
        title = UIManager.Instance.textSystem.GetChild(0).GetComponent<RectTransform>();

        topSlider = UIManager.Instance.backgroundSystem.GetChild(1).GetComponent<RectTransform>();
        topChessHolder = topSlider.GetChild(0).GetComponent<RectTransform>();

        bottomSlider = UIManager.Instance.backgroundSystem.GetChild(2).GetComponent<RectTransform>();
        bottomChessHolder = bottomSlider.GetChild(0).GetComponent<RectTransform>();

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
        slider.GetChild(1).GetComponent<RectTransform>().DOAnchorPosY(sliderPositionFullHeight, _timer).SetEase(Ease.OutBack);
        slider.GetChild(2).GetComponent<RectTransform>().DOAnchorPosY(sliderPositionFullHeight + sliderOffset, _timer).SetEase(Ease.OutBack);
    }

    public void ShowSliderHalfHeight(RectTransform slider)
    {
        slider.GetChild(1).GetComponent<RectTransform>().DOAnchorPosY(sliderPositionHalfHeight, _timer).SetEase(Ease.OutBack);
        slider.GetChild(2).GetComponent<RectTransform>().DOAnchorPosY(sliderPositionHalfHeight + sliderOffset, _timer).SetEase(Ease.OutBack);
    }

    public void ShowChessHolder(RectTransform chessHolder)
    {
        RectTransform holderCircle = chessHolder.GetChild(0).GetComponent<RectTransform>();
        holderCircle.DOAnchorPosY(chessHolderPosition, _timer);
        holderCircle.DOScale(Vector3.one * chessHolderScale, _timer).SetEase(Ease.OutBack);

        RectTransform container = holderCircle.GetChild(1).GetComponent<RectTransform>();
        StartCoroutine(Cor_AnimChessPieces(container));
    }
    private IEnumerator Cor_AnimChessPieces(RectTransform container)
    {
        for (int i = 0; i < container.childCount; ++i)
        {
            RectTransform piece = container.GetChild(i) as RectTransform;
            piece.DOAnchorPosY(425, _timer);
            yield return new WaitForSeconds(_timer / 10f);
        }
    }
}
