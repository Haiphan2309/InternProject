using DG.Tweening;
using GDC.Managers;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIReward : MonoBehaviour
{
    [SerializeField] private RectTransform rect, desContainRect;
    
    [SerializeField] private UIRewardSlot uiRewardSlotPrefab;
    [SerializeField] private Transform contentTrans;
    [SerializeField] private TMP_Text desText;
    [SerializeField] private Button exitButton;
    public void Show(DailyRewardConfig dailyRewardConfig)
    {
        SoundManager.Instance.PlaySound(AudioPlayer.SoundID.SFX_UI_SHOW);
        exitButton.onClick.RemoveAllListeners();
        exitButton.onClick.AddListener(Hide);

        rect.localScale = Vector2.zero;
        rect.DOScale(1, 0.5f).SetEase(Ease.OutBack);

        int rand = Random.Range(0, dailyRewardConfig.rewards.Count);
        UIRewardSlot slot = Instantiate(uiRewardSlotPrefab, contentTrans);
        slot.Setup(dailyRewardConfig.rewards[rand]);

        desContainRect.gameObject.SetActive(false);
        SaveLoadManager.Instance.GameData.undoNum += dailyRewardConfig.rewards[rand].undoAmount;
        SaveLoadManager.Instance.GameData.solveNum += dailyRewardConfig.rewards[rand].solveAmount;
        SaveLoadManager.Instance.GameData.turnNum += dailyRewardConfig.rewards[rand].turnAmount;
        SaveLoadManager.Instance.Save();
    }
    public void Show(ShopSlotData shopSlotData, string des)
    {
        SoundManager.Instance.PlaySound(AudioPlayer.SoundID.SFX_UI_SHOW);
        exitButton.onClick.RemoveAllListeners();
        exitButton.onClick.AddListener(Hide);
        rect.localScale = Vector2.zero;
        rect.DOScale(1, 0.5f).SetEase(Ease.OutBack);

        if (shopSlotData.undoAmount > 0)
        {
            RewardData rewardData = new RewardData();
            rewardData.undoAmount = shopSlotData.undoAmount;
            UIRewardSlot slot = Instantiate(uiRewardSlotPrefab, contentTrans);
            slot.Setup(rewardData);
        }
        if (shopSlotData.solveAmount > 0)
        {
            RewardData rewardData = new RewardData();
            rewardData.solveAmount = shopSlotData.solveAmount;
            UIRewardSlot slot = Instantiate(uiRewardSlotPrefab, contentTrans);
            slot.Setup(rewardData);
        }
        if (shopSlotData.turnAmount > 0)
        {
            RewardData rewardData = new RewardData();
            rewardData.turnAmount = shopSlotData.turnAmount;
            UIRewardSlot slot = Instantiate(uiRewardSlotPrefab, contentTrans);
            slot.Setup(rewardData);
        }

        desContainRect.gameObject.SetActive(true);
        desText.text = des;
        SaveLoadManager.Instance.GameData.undoNum += shopSlotData.undoAmount;
        SaveLoadManager.Instance.GameData.solveNum += shopSlotData.solveAmount;
        SaveLoadManager.Instance.GameData.turnNum += shopSlotData.turnAmount;
        SaveLoadManager.Instance.Save();
    }
    public void Hide()
    {
        SoundManager.Instance.PlaySound(AudioPlayer.SoundID.SFX_BUTTON_CLICK);
        PopupManager.Instance.HideBlackBg();
        rect.DOScale(0, 0.5f).SetEase(Ease.InBack);
    }
}
