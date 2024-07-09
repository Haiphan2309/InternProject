using DG.Tweening;
using GDC.Managers;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIWinPanel : MonoBehaviour
{
    [SerializeField] UIPopupAnim uiPopupAnim;
    [SerializeField] List<Image> stars, halos;
    [SerializeField] Slider turnSlider;
    [SerializeField] TMP_Text turnText;
    [SerializeField] Button menuBtn, replayBtn, nextBtn;
    Color haloColor;

    [Button]
    public void Show()
    {
        gameObject.SetActive(true);

        SoundManager.Instance.PlaySound(AudioPlayer.SoundID.SFX_WIN);

        menuBtn.onClick.AddListener(OnMenu);
        replayBtn.onClick.AddListener(OnReplay);
        nextBtn.onClick.AddListener(OnNextLevel);

        haloColor = halos[0].color;
        uiPopupAnim.Show(false);
        //Debug.Log(GameplayManager.Instance.levelData.maxTurn);
        turnSlider.maxValue = GameplayManager.Instance.levelData.maxTurn;
        turnText.text = "Turn: " + GameplayManager.Instance.remainTurn.ToString();
        turnSlider.value = 0;
        foreach (var item in stars)
        {
            DOTween.Kill(item);
            Color fadeBlack = Color.black;
            fadeBlack.a = 0.5f;
            item.color = fadeBlack;
            item.rectTransform.localScale = Vector2.one * 0.9f;
        }
        foreach (var item in halos)
        {
            DOTween.Kill(item);
            item.color = haloColor;
            item.rectTransform.localScale = Vector2.zero;
        }
        StartCoroutine(Cor_Show());
    }

    IEnumerator Cor_Show()
    {
        yield return new WaitForSeconds(0.5f);
        turnSlider.DOValue(GameplayManager.Instance.remainTurn, 3);
        int starNum = GameplayManager.Instance.GetStarOfCurrentLevel();
        for (int i = 0; i < starNum; i++)
        {
            stars[i].DOColor(Color.white, 0.2f);
            stars[i].rectTransform.DOScale(1, 0.3f).SetEase(Ease.OutBack);
            halos[i].rectTransform.DOScale(1, 0.5f).SetEase(Ease.OutBack);
            yield return new WaitForSeconds(1);
        }
    }
    void OnMenu()
    {
        GameManager.Instance.LoadSceneManually(
            GDC.Enums.SceneType.MAINMENU,
            GDC.Enums.TransitionType.IN,
            SoundType.NONE,
            cb: () =>
            {
                //    //GDC.Managers.GameManager.Instance.SetInitData(levelIndex);
            },
            true);
    }
    void OnReplay()
    {
        int currentChapterIndex = GameplayManager.Instance.chapterData.id;
        int currentLevelIndex = GameplayManager.Instance.levelData.id;
        GameManager.Instance.LoadSceneManually(
            GDC.Enums.SceneType.GAMEPLAY,
            GDC.Enums.TransitionType.IN,
            SoundType.NONE,
            cb: () =>
            {
                GameManager.Instance.SetInitData(currentChapterIndex, currentLevelIndex);
            },
            true);
    }
    void OnNextLevel()
    {
        ChapterData curChapterData = GameplayManager.Instance.chapterData;
        int nextLevelIndex = GameplayManager.Instance.levelData.id + 1;
        int nextChapterIndex = curChapterData.id + 1;
        if (curChapterData.levelDatas.Count >= nextLevelIndex)
        {
            nextLevelIndex = 0;
            ChapterData nextChapterData = GameUtils.GetChapterData(nextChapterIndex);
            if (nextChapterData == null)
            {
                Debug.Log("Da het level de choi");
                replayBtn.onClick.Invoke();
                return;
            }
        }
        GameManager.Instance.LoadSceneManually(
            GDC.Enums.SceneType.GAMEPLAY,
            GDC.Enums.TransitionType.IN,
            SoundType.NONE,
            cb: () =>
            {
                GameManager.Instance.SetInitData(nextChapterIndex, nextLevelIndex);
            },
            true);
    }
}
