using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIChapterMenu : UI
{
    public override IEnumerator Cor_Anim()
    {
        SetText();
        SetButtonSystem();
        SetChessHolder();
        SetSlider();
        SetChapterSystem();
        SetLevelSystem();
        yield return new WaitForSeconds(1f);
        UIManager.Instance.ShowAllButtons();
        yield return null;
    }

    private void SetText()
    {
        HideTitleText();
        UIManager.Instance.chapter.GetComponent<RectTransform>().DOAnchorPosY(600, _timer);
    }

    private void SetSlider()
    {
        ShowSliderHalfHeight(UIManager.Instance.topSlider);
        ShowSliderHalfHeight(UIManager.Instance.bottomSlider);
    }

    private void SetChessHolder()
    {
        HideChessHolder(UIManager.Instance.topChessHolder);
        HideChessHolder(UIManager.Instance.bottomChessHolder);
    }

    private void SetButtonSystem()
    {
        UIManager.Instance.startButton.GetComponent<RectTransform>().DOAnchorPosX(600, _timer);
        UIManager.Instance.returnButton.GetComponent<RectTransform>().DOAnchorPosX(75, _timer);
        UIManager.Instance.creditButton.GetComponent<RectTransform>().DOAnchorPosX(-600, _timer);
    }

    private void SetChapterSystem()
    {
        UIManager.Instance.chapterHolder.DOAnchorPosY(150, _timer);
        //ShowLevelHolder();
    }

    private void SetLevelSystem()
    {
        HideLevelHolder();
    }
}
