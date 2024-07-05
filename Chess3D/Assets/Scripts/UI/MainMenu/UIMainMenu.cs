using DG.Tweening;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class UIMainMenu : UI
{
    public override IEnumerator Cor_Anim()
    {
        SetSlider();
        SetChessHolder();
        SetChapterSystem();
        SetLevelSystem();
        SetButtonSystem();
        SetText();
        yield return new WaitForSeconds(1.5f);
        UIManager.Instance.ShowAllButtons();
        yield return null;
    }

    private void SetText()
    {
        ShowTitleText();
        UIManager.Instance.chapter.GetComponent<RectTransform>().DOAnchorPosY(600, _timer);
    }

    private void SetSlider()
    {
        ShowSliderFullHeight(UIManager.Instance.topSlider);
        ShowSliderFullHeight(UIManager.Instance.bottomSlider);
    }

    private void SetChessHolder()
    {
        ShowChessHolder(UIManager.Instance.topChessHolder);
        ShowChessHolder(UIManager.Instance.bottomChessHolder);
    }

    private void SetButtonSystem()
    {
        UIManager.Instance.startButton.GetComponent<RectTransform>().DOAnchorPosX(-75, _timer);
        UIManager.Instance.settingButton.GetComponent<RectTransform>().DOAnchorPosX(75, _timer);
        UIManager.Instance.returnButton.GetComponent<RectTransform>().DOAnchorPosX(-600, _timer);
        UIManager.Instance.creditButton.GetComponent<RectTransform>().DOAnchorPosX(225, _timer);
    }

    private void SetChapterSystem()
    {
        UIManager.Instance.chapterHolder.DOAnchorPosY(-2000, _timer);
    }

    private void SetLevelSystem()
    {
        HideLevelHolder();
    }
}
