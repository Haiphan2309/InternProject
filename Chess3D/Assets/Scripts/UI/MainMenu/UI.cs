using DG.Tweening;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UI : MonoBehaviour
{
    public float _timer = 1f;
    public bool isLoaded = false;

    private readonly float sliderPositionFullHeight = 600f;
    private readonly float sliderPositionHalfHeight = 325f;
    private readonly float sliderOffset = -50f;

    private readonly float titleTextPosition = 350f;
    private readonly float chapterTextPosition = -25f;
    private readonly float textHidePosition = 600f;

    private readonly float chessHolderPosition = 250f;
    private readonly float chessHolderScale = 15f;

    private readonly float chessPiecePosition = 250f;
    private readonly float chessPieceOffset = 10f;

    private readonly float holderHidePosition = -2000f;

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
        UIManager.Instance.levelHolder.DOAnchorPosY(holderHidePosition, _timer).SetEase(Ease.OutBack);
    }
    public void ShowChapterHolder()
    {
        UIManager.Instance.chapterHolder.DOAnchorPosY(0, _timer).SetEase(Ease.OutBack);
    }

    public void HideChapterHolder()
    {
        UIManager.Instance.chapterHolder.DOAnchorPosY(holderHidePosition, _timer).SetEase(Ease.OutBack);
    }

    public void ShowTitleText()
    {
        UIManager.Instance.title.DOAnchorPosY(-titleTextPosition, _timer * 1.5f).SetEase(Ease.OutBack);
    }

    public void HideTitleText()
    {
        UIManager.Instance.title.DOAnchorPosY(textHidePosition, _timer).SetEase(Ease.OutBack);
    }

    public void ShowChapterText()
    {
        UIManager.Instance.chapter.DOAnchorPosY(chapterTextPosition, _timer).SetEase(Ease.OutBack);
    }

    public void HideChapterText()
    {
        UIManager.Instance.chapter.DOAnchorPosY(textHidePosition, _timer).SetEase(Ease.OutBack);
    }

    public void MenuButtonSystem()
    {
        UIManager.Instance.startButton.GetComponent<RectTransform>().DOAnchorPosX(-75, _timer);
        UIManager.Instance.settingButton.GetComponent<RectTransform>().DOAnchorPosX(75, _timer);
        UIManager.Instance.shopButton.GetComponent<RectTransform>().DOAnchorPosX(75, _timer);
        UIManager.Instance.returnButton.GetComponent<RectTransform>().DOAnchorPosX(-600, _timer);
        UIManager.Instance.creditButton.GetComponent<RectTransform>().DOAnchorPosX(225, _timer);
    }

    public void OtherButtonSystem()
    {
        UIManager.Instance.startButton.GetComponent<RectTransform>().DOAnchorPosX(600, _timer);
        UIManager.Instance.settingButton.GetComponent<RectTransform>().DOAnchorPosX(75, _timer);
        UIManager.Instance.shopButton.GetComponent<RectTransform>().DOAnchorPosX(75, _timer);
        UIManager.Instance.returnButton.GetComponent<RectTransform>().DOAnchorPosX(75, _timer);
        UIManager.Instance.creditButton.GetComponent<RectTransform>().DOAnchorPosX(-600, _timer);
    }
}
