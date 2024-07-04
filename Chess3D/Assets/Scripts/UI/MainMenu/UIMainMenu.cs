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
        SetLevelSystem();
        SetSlider();
        SetChessHolder();
        SetButtonSystem();
        SetTitle();
        yield return new WaitForSeconds(1.5f);
        EnableButton();
        yield return null;
    }

    private void SetTitle()
    {
        UIManager.Instance.title.DOAnchorPosY(-300, _timer * 1.5f).SetEase(Ease.OutBack);
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
        UIManager.Instance.startButton.GetComponent<RectTransform>().DOAnchorPosX(-50, _timer);
        UIManager.Instance.settingButton.GetComponent<RectTransform>().DOAnchorPosX(50, _timer);
        UIManager.Instance.returnButton.GetComponent<RectTransform>().DOAnchorPosX(-600, _timer);
    }

    private void SetLevelSystem()
    {
        HideLevelHolder();
    }
}
