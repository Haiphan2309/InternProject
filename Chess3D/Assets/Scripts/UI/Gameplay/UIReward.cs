using DG.Tweening;
using GDC.Managers;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIReward : MonoBehaviour
{
    [SerializeField] private RectTransform rect;
    
    [SerializeField] private UIRewardSlot uiRewardSlotPrefab;
    [SerializeField] private Transform contentTrans;
    [SerializeField] private TMP_Text desText;
    [SerializeField] private Button exitButton;
    public void Show(DailyRewardConfig dailyRewardConfig)
    {
        exitButton.onClick.RemoveAllListeners();
        exitButton.onClick.AddListener(Hide);

        rect.localScale = Vector2.zero;
        rect.DOScale(1, 0.5f).SetEase(Ease.OutBack);

        int rand = Random.Range(0, dailyRewardConfig.rewards.Count);
        UIRewardSlot slot = Instantiate(uiRewardSlotPrefab, contentTrans);
        slot.Setup(dailyRewardConfig.rewards[rand]);

        desText.gameObject.SetActive(false);
        SaveLoadManager.Instance.GameData.undoNum += dailyRewardConfig.rewards[rand].undoAmount;
        SaveLoadManager.Instance.GameData.solveNum += dailyRewardConfig.rewards[rand].solveAmount;
        SaveLoadManager.Instance.GameData.turnNum += dailyRewardConfig.rewards[rand].turnAmount;
        SaveLoadManager.Instance.Save();
    }
    public void Show(ShopSlotData shopSlotData, string des)
    {
        exitButton.onClick.RemoveAllListeners();
        exitButton.onClick.AddListener(Hide);
        rect.localScale = Vector2.zero;
        rect.DOScale(1, 0.5f).SetEase(Ease.OutBack);
        UIRewardSlot slot = Instantiate(uiRewardSlotPrefab, contentTrans);
        slot.Setup(shopSlotData);
        desText.gameObject.SetActive(true);
        desText.text = des;
        SaveLoadManager.Instance.GameData.undoNum += shopSlotData.undoAmount;
        SaveLoadManager.Instance.GameData.solveNum += shopSlotData.solveAmount;
        SaveLoadManager.Instance.GameData.turnNum += shopSlotData.turnAmount;
        SaveLoadManager.Instance.Save();
    }
    public void Hide()
    {
        rect.DOScale(0, 0.5f).SetEase(Ease.InBack);
    }
}
