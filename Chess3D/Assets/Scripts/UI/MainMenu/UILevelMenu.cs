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
        title.DOAnchorPosY(300f, _timer * 1.5f).SetEase(Ease.OutBack);
    }

    private void SetSlider()
    {
        RectTransform topBG = topSlider.Find("BG") as RectTransform;
        RectTransform topOverlay = topSlider.Find("Overlay") as RectTransform;
        topBG.DOAnchorPosY(-400, _timer).SetEase(Ease.OutBack);
        topOverlay.DOAnchorPosY(-330, _timer).SetEase(Ease.OutBack);

        RectTransform bottomBG = bottomSlider.Find("BG") as RectTransform;
        RectTransform bottomOverlay = bottomSlider.Find("Overlay") as RectTransform;
        bottomBG.DOAnchorPosY(400, _timer);
        bottomOverlay.DOAnchorPosY(330, _timer);
    }

    private void SetChessHolder()
    {
        StartCoroutine(Cor_AnimChessPieces(topChessContainer));
        RectTransform topHolderCircle = topChessHolder.Find("Circle") as RectTransform;
        topHolderCircle.DOAnchorPosY(500, _timer);
        topHolderCircle.DOScale(Vector3.right + Vector3.up, _timer).SetEase(Ease.OutBack);

        StartCoroutine(Cor_AnimChessPieces(bottomChessContainer));
        RectTransform bottomHolderCircle = bottomChessHolder.Find("Circle") as RectTransform;
        bottomHolderCircle.DOAnchorPosY(-500, _timer);
        bottomHolderCircle.DOScale(Vector3.right + Vector3.up, _timer).SetEase(Ease.OutBack);
    }

    IEnumerator Cor_AnimChessPieces(RectTransform container)
    {
        for (int i = 0; i < container.childCount; ++i)
        {
            RectTransform piece2 = container.GetChild(i) as RectTransform;
            piece2.DOAnchorPosY(-425, _timer);
        }
        yield return null;
    }

    private void SetButtonSystem()
    {
        startButton.DOAnchorPosX(600, _timer);
        returnButton.DOAnchorPosX(100, _timer);
    }
    private void SetLevelSystem()
    {
        UIManager.Instance.levelSystem.DOAnchorPosY(0, _timer).SetEase(Ease.OutBack);
    }

    [Button]
    void LoadLevelTest()
    {
        LoadLevel(0);
    }
    public void LoadLevel(int levelIndex)
    {
        GameManager.Instance.LoadSceneManually(
            GDC.Enums.SceneType.GAMEPLAY,
            GDC.Enums.TransitionType.IN,
            SoundType.NONE,
            cb: () =>
            {
                GameManager.Instance.SetInitData(levelIndex);
            }, 
            true);
    }
}
