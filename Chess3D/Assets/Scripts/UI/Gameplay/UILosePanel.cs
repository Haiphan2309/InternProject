using GDC.Managers;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILosePanel : MonoBehaviour
{
    [SerializeField] UIPopupAnim uiPopupAnim;
    [SerializeField] Button menuBtn, replayBtn;

    [Button]
    public void Show()
    {
        gameObject.SetActive(true);

        SoundManager.Instance.PlaySound(AudioPlayer.SoundID.SFX_LOSE);

        menuBtn.onClick.AddListener(OnMenu);
        replayBtn.onClick.AddListener(OnReplay);

        uiPopupAnim.Show(false);
        
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
                GDC.Managers.GameManager.Instance.SetInitData(currentChapterIndex, currentLevelIndex);
            },
            true);
    }
}
