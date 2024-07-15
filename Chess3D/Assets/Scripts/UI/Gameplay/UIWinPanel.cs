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
    [SerializeField] private UIPopupAnim uiPopupAnim;
    [SerializeField] private List<Image> stars, halos;
    [SerializeField] private Slider turnSlider;
    [SerializeField] private TMP_Text turnText, newRecordText;
    [SerializeField] private Button menuBtn, replayBtn, nextBtn;
    [SerializeField] private RectTransform starTurn2Rect;
    private Color haloColor;

    [Button]
    public void Show(bool isNewRecord)
    {
        gameObject.SetActive(true);

        SoundManager.Instance.PlaySound(AudioPlayer.SoundID.SFX_WIN);

        menuBtn.onClick.AddListener(OnMenu);
        replayBtn.onClick.AddListener(OnReplay);
        nextBtn.onClick.AddListener(OnNextLevel);

        haloColor = halos[0].color;
        uiPopupAnim.Show(false);
        //Debug.Log(GameplayManager.Instance.levelData.maxTurn);
        turnSlider.maxValue = GameplayManager.Instance.levelData.starTurn3;
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
        DOTween.Kill(newRecordText);
        newRecordText.rectTransform.localScale = Vector2.zero;
        float sliderWidth = turnSlider.GetComponent<RectTransform>().sizeDelta.x;
        starTurn2Rect.anchoredPosition = new Vector2((float)GameplayManager.Instance.levelData.starTurn2 / GameplayManager.Instance.levelData.starTurn3 * sliderWidth, 0);
        StartCoroutine(Cor_Show(isNewRecord));
    }

    IEnumerator Cor_Show(bool isNewRecord)
    {
        yield return new WaitForSeconds(0.5f);
        if (GameplayManager.Instance.remainTurn >= GameplayManager.Instance.levelData.starTurn3)
        {
            turnSlider.DOValue(GameplayManager.Instance.levelData.starTurn3, 3);
        }
        else
        {
            turnSlider.DOValue(GameplayManager.Instance.remainTurn, 3);
        }    
        int starNum = GameplayManager.Instance.GetStarOfCurrentLevel();
        for (int i = 0; i < starNum; i++)
        {
            stars[i].DOColor(Color.white, 0.2f);
            stars[i].rectTransform.DOScale(1, 0.3f).SetEase(Ease.OutBack);
            halos[i].rectTransform.DOScale(1, 0.5f).SetEase(Ease.OutBack);
            SoundManager.Instance.PlaySound(AudioPlayer.SoundID.SFX_STAR);
            yield return new WaitForSeconds(1);
        }
        if (isNewRecord)
        {
            //Congrats! It's a new record!
            newRecordText.color = Color.white;
            newRecordText.rectTransform.DOScale(1,0.3f).SetEase(Ease.OutBack).OnComplete(()=>
            {
                newRecordText.rectTransform.localScale = Vector2.one;
                newRecordText.rectTransform.DOScale(1.2f, 0.5f).SetLoops(-1, LoopType.Yoyo);
            });
        };
    }
    void OnMenu()
    {
        int curChapterIndex = GameplayManager.Instance.chapterData.id;
        GameManager.Instance.LoadSceneManually(
            GDC.Enums.SceneType.MAINMENU,
            GDC.Enums.TransitionType.IN,
            SoundType.NONE,
            cb: () =>
            {
                //    //GDC.Managers.GameManager.Instance.SetInitData(levelIndex);
                GameManager.Instance.LoadMenuLevel(curChapterIndex);
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
        int nextChapterIndex = curChapterData.id;
        if (curChapterData.levelDatas.Count <= nextLevelIndex)
        {
            nextLevelIndex = 0;
            ChapterData nextChapterData = GameUtils.GetChapterData(nextChapterIndex);
            if (nextChapterData == null)
            {
                Debug.Log("Da het level de choi");
                replayBtn.onClick.Invoke();
                return;
            }
            else
            {
                Debug.Log("End of chapter");

                GameManager.Instance.LoadSceneManually(
                    GDC.Enums.SceneType.MAINMENU,
                    GDC.Enums.TransitionType.IN,
                    SoundType.NONE,
                    cb: () =>
                    {
                        //    //GDC.Managers.GameManager.Instance.SetInitData(levelIndex);
                        GameManager.Instance.LoadMenuChapter();
                    },
                    true);
                return;
                //nextChapterIndex++;
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
