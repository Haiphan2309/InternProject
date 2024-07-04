using DG.Tweening;
using GDC.Managers;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILevelMenu : UI
{
    public override IEnumerator Cor_Anim()
    {
        SetTitle();
        SetButtonSystem();
        SetChessHolder();
        SetSlider();
        SetLevelSystem();
        yield return new WaitForSeconds(1f);
        EnableButton();
        yield return null;
    }

    private void SetTitle()
    {
        UIManager.Instance.title.DOAnchorPosY(300f, _timer * 1.5f).SetEase(Ease.OutBack);
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
        UIManager.Instance.returnButton.GetComponent<RectTransform>().DOAnchorPosX(100, _timer);
    }
    private void SetLevelSystem()
    {
        UIManager.Instance.levelSystem.DOAnchorPosY(0, _timer).SetEase(Ease.OutBack);
    }
}
