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
        SetHolderSystem();
        SetButtonSystem();
        SetText();
        yield return new WaitForSeconds(1.5f);
        UIManager.Instance.ShowAllButtons();
        yield return null;
    }

    private void SetText()
    {
        ShowTitleText();
        HideChapterText();
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
        MenuButtonSystem();
    }

    private void SetHolderSystem()
    {
        HideLevelHolder();
        HideChapterHolder();
    }
}
