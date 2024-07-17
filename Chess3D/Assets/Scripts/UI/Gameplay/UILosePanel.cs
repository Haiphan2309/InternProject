using GDC.Managers;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UILosePanel : MonoBehaviour
{
    [SerializeField] UIPopupAnim uiPopupAnim;
    [SerializeField] Button menuBtn, replayBtn, rewardAdsBtn;
    [SerializeField] RewardedAdsButton rewardedAdsButton;
    [SerializeField] TMP_Text turnRewardText;
    [SerializeField] int turnRewardNum;
    bool isRewarded;

    [Button]
    public void Show()
    {
        gameObject.SetActive(true);

        SoundManager.Instance.PlaySound(AudioPlayer.SoundID.SFX_LOSE);

        menuBtn.onClick.AddListener(OnMenu);
        replayBtn.onClick.AddListener(OnReplay);
        rewardAdsBtn.onClick.AddListener(OnRewardAds);

        turnRewardText.text = "Turn +" + turnRewardNum.ToString();

        uiPopupAnim.Show(false);

        if (isRewarded == false && GameplayManager.Instance.remainTurn <= 0)
        {
            Debug.Log("Show reward btn");
            rewardedAdsButton.LoadAd();
        }
        else
        {
            Debug.Log("Hide reward btn");
            uiPopupAnim.AddButtonDisable(rewardAdsBtn);
        }
        
    }
    public void Hide()
    {
        uiPopupAnim.Hide();
        menuBtn.onClick.RemoveAllListeners();
        replayBtn.onClick.RemoveAllListeners();
        rewardAdsBtn.onClick.RemoveAllListeners();
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
    void OnRewardAds()
    {
        Hide();
        GameplayManager.Instance.RewardTurn(turnRewardNum);
        isRewarded = true;
    }
}
