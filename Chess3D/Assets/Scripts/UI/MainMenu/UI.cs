using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UI : MonoBehaviour
{
    public float _timer = 1f;
    public bool isLoaded = false;

    private float sliderPositionFullHeight = 630f;
    private float sliderPositionHalfHeight = 315f;
    private float sliderOffset = -50f;

    private float chessHolderPosition = 350f;
    private float chessHolderScale = 20f;

    private float chessPiecePosition = 250f;
    private float chessPieceOffset = 20f;

    public virtual void Anim()
    {
        DisableButton();
        StartCoroutine(Cor_Anim());
    }

    public virtual IEnumerator Cor_Anim()
    {
        yield return null;
    }
    public void EnableButton()
    {
        UIManager.Instance.startButton.GetComponent<Button>().interactable = true;
        UIManager.Instance.settingButton.GetComponent<Button>().interactable = true;
        UIManager.Instance.returnButton.GetComponent<Button>().interactable = true;
    }

    public void DisableButton()
    {
        UIManager.Instance.startButton.GetComponent<Button>().interactable = false;
        UIManager.Instance.settingButton.GetComponent<Button>().interactable = false;
        UIManager.Instance.returnButton.GetComponent<Button>().interactable = false;
    }

    public void ShowSliderFullHeight(RectTransform slider)
    {
        slider.GetChild(0).GetComponent<RectTransform>().DOAnchorPosY(sliderPositionFullHeight, _timer).SetEase(Ease.OutBack);
        slider.GetChild(1).GetComponent<RectTransform>().DOAnchorPosY(sliderPositionFullHeight + sliderOffset, _timer).SetEase(Ease.OutBack);
    }

    public void ShowSliderHalfHeight(RectTransform slider)
    {
        slider.GetChild(0).GetComponent<RectTransform>().DOAnchorPosY(sliderPositionHalfHeight, _timer).SetEase(Ease.OutBack);
        slider.GetChild(1).GetComponent<RectTransform>().DOAnchorPosY(sliderPositionHalfHeight + sliderOffset, _timer).SetEase(Ease.OutBack);
    }

    public void ShowChessHolder(RectTransform chessHolder)
    {
        chessHolder.gameObject.SetActive(true);
        RectTransform holderCircle = chessHolder.GetChild(0).GetComponent<RectTransform>();
        holderCircle.DOAnchorPosY(chessHolderPosition, _timer);
        holderCircle.DOScale(Vector3.one * chessHolderScale, _timer).SetEase(Ease.OutBack);

        RectTransform container = chessHolder.GetChild(1).GetComponent<RectTransform>();
        StartCoroutine(Cor_AnimShowChessPieces(container));
    }
    private IEnumerator Cor_AnimShowChessPieces(RectTransform container)
    {
        for (int i = 0; i < container.childCount; ++i)
        {
            RectTransform piece = container.GetChild(i) as RectTransform;
            piece.DOAnchorPosY(chessPiecePosition + chessPieceOffset * i, _timer);
            yield return new WaitForSeconds(_timer / 10f);
        }
    }

    public void HideChessHolder(RectTransform chessHolder)
    {
        RectTransform holderCircle = chessHolder.GetChild(0).GetComponent<RectTransform>();
        holderCircle.DOScale(Vector3.one, _timer).SetEase(Ease.OutBack);
        holderCircle.DOAnchorPosY(-chessHolderPosition, _timer);

        RectTransform container = chessHolder.GetChild(1).GetComponent<RectTransform>();
        StartCoroutine(Cor_AnimHideChessPieces(container));
        chessHolder.gameObject.SetActive(false);
    }
    IEnumerator Cor_AnimHideChessPieces(RectTransform container)
    {
        for (int i = 0; i < container.childCount; ++i)
        {
            RectTransform piece2 = container.GetChild(i) as RectTransform;
            piece2.DOAnchorPosY(-chessPiecePosition - chessPieceOffset * i, _timer);
        }
        yield return null;
    }
}
