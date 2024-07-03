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
        title.DOAnchorPosY(-300, _timer * 1.5f).SetEase(Ease.OutBack);
    }

    private void SetSlider()
    {
        ShowSliderFullHeight(topSlider);
        ShowSliderFullHeight(bottomSlider);
    }

    private void SetChessHolder()
    {
        ShowChessHolder(topChessHolder);
        ShowChessHolder(bottomChessHolder);
    }

    IEnumerator Cor_AnimChessPieces(RectTransform container)
    {
        for (int i = 0; i < container.childCount; ++i)
        {
            RectTransform piece = container.GetChild(i) as RectTransform;
            piece.DOAnchorPosY(425, _timer);
            yield return new WaitForSeconds(_timer / 10f);
        }
    }

    private void SetButtonSystem()
    {
        startButton.DOAnchorPosX(-50, _timer);
        settingButton.DOAnchorPosX(50, _timer);
        returnButton.DOAnchorPosX(-600, _timer);
    }

    private void SetLevelSystem()
    {
        UIManager.Instance.levelSystem.DOAnchorPosY(-1200f, _timer).SetEase(Ease.OutBack);
    }
}
