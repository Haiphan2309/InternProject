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

    private float titleTextPosition = 350f;

    private float chessHolderPosition = 250f;
    private float chessHolderScale = 15f;

    private float chessPiecePosition = 250f;
    private float chessPieceOffset = 20f;

    private float levelHolderPosition = 2000f;

    public virtual void Anim()
    {
        UIManager.Instance.HideAllButtons();
        StartCoroutine(Cor_Anim());
    }

    public virtual IEnumerator Cor_Anim()
    {
        yield return null;
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

    public void ShowLevelHolder()
    {
        UIManager.Instance.levelHolder.DOAnchorPosY(0, _timer).SetEase(Ease.OutBack);
    }

    public void HideLevelHolder()
    {
        UIManager.Instance.levelHolder.DOAnchorPosY(-levelHolderPosition, _timer).SetEase(Ease.OutBack);
    }

    public void ShowTitleText()
    {
        UIManager.Instance.title.DOAnchorPosY(-titleTextPosition, _timer * 1.5f).SetEase(Ease.OutBack);
    }

    public void HideTitleText()
    {
        UIManager.Instance.title.DOAnchorPosY(600, _timer * 1.5f).SetEase(Ease.OutBack);
    }
}
