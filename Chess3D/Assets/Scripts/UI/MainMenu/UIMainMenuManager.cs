using DG.Tweening;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class UIMainMenuManager : MonoBehaviour
{
    [SerializeField] RectTransform topSlider;
    private RectTransform topChessHolder;
    private RectTransform topChessContainer;

    [SerializeField] RectTransform bottomSlider;
    private RectTransform bottomChessHolder;
    private RectTransform bottomChessContainer;

    [SerializeField] RectTransform buttonSystem;
    private RectTransform startButton;
    private RectTransform settingButton;

    [Button]
    private void Anim()
    {
        StartCoroutine(Cor_AnimMenu());
    }
    private void Preset()
    {
        topChessHolder = topSlider.Find("ChessHolder") as RectTransform;
        topChessContainer = topChessHolder.Find("ChessContainer") as RectTransform;
        topSlider.anchoredPosition = Vector2.up * 600;

        bottomChessHolder = bottomSlider.Find("ChessHolder") as RectTransform;
        bottomChessContainer = bottomChessHolder.Find("ChessContainer") as RectTransform;
        bottomSlider.anchoredPosition = Vector2.down * 600;

        startButton = buttonSystem.Find("StartButton") as RectTransform;
        startButton.anchoredPosition = Vector2.right * 600;

        settingButton = buttonSystem.Find("SettingButton") as RectTransform;
        settingButton.anchoredPosition = Vector2.left * 600;
    }

    private void AnimSlider()
    {
        RectTransform topBG = topSlider.Find("BG") as RectTransform;
        RectTransform topOverlay = topSlider.Find("Overlay") as RectTransform;
        topBG.DOAnchorPosY(-600, 1f).SetEase(Ease.OutBack);
        topOverlay.DOAnchorPosY(-530, 1f).SetEase(Ease.OutBack);

        RectTransform bottomBG = bottomSlider.Find("BG") as RectTransform;
        RectTransform bottomOverlay = bottomSlider.Find("Overlay") as RectTransform;
        bottomBG.DOAnchorPosY(600, 1f);
        bottomOverlay.DOAnchorPosY(530, 1f);
    }

    private void AnimChessHolder()
    {
        RectTransform topHolderCircle = topChessHolder.Find("Circle") as RectTransform;
        topHolderCircle.DOAnchorPosY(-500, 1f);
        topHolderCircle.DOScale(Vector3.right * 20 + Vector3.up * 20, 1f).SetEase(Ease.OutBack);

        RectTransform bottomHolderCircle = bottomChessHolder.Find("Circle") as RectTransform;
        bottomHolderCircle.DOAnchorPosY(500, 1f);
        bottomHolderCircle.DOScale(Vector3.right * 20 + Vector3.up * 20, 1f).SetEase(Ease.OutBack);
    }

    private void AnimButton()
    {
        startButton.DOAnchorPos(Vector3.left * 50 + Vector3.up * 25, 1f);
        settingButton.DOAnchorPos(Vector3.right * 25 + Vector3.down * 25, 1f);
    }

    IEnumerator Cor_AnimChessPieces()
    {
        for (int i = 0; i < bottomChessContainer.childCount; ++i)
        {
            RectTransform piece2 = bottomChessContainer.GetChild(i) as RectTransform;
            piece2.DOAnchorPosY(425, 1f);
            yield return new WaitForSeconds(0.1f);
        }
        for (int i = 0; i < topChessContainer.childCount; ++i)
        {
            RectTransform piece1 = topChessContainer.GetChild(i) as RectTransform;
            piece1.DOAnchorPosY(425, 1f);
            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator Cor_AnimMenu()
    {
        Preset();
        AnimSlider();
        yield return new WaitForSeconds(1);
        AnimChessHolder();
        yield return new WaitForSeconds(1);
        StartCoroutine(Cor_AnimChessPieces());
        AnimButton();
    }
}
