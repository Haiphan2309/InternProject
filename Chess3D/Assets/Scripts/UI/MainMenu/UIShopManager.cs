using DG.Tweening;
using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GDC.Managers;

public class UIShopManager : MonoBehaviour
{
    [SerializeField] private ShopConfig shopConfig;
    [SerializeField] private DailyRewardConfig dailyRewardConfig;
    [SerializeField] private RectTransform rect, contentRect, haloDailyRewardRect, comeBackTomorrowRect;
    [SerializeField] private UIShopSlot shopSlotprefab;

    [SerializeField] private Button exitButton, removeAds, dailyRewardButton;
    [SerializeField] private LanguageDictionary removeAdsDict;
    [SerializeField] private Sprite dailyRewardOpenSprite, dailyRewardCloseSprite;

    [Button]
    public void Show()
    {
        exitButton.onClick.RemoveAllListeners();
        exitButton.onClick.AddListener(Hide);

        removeAds.onClick.RemoveAllListeners();
        removeAds.onClick.AddListener(RemoveAds);

        dailyRewardButton.onClick.RemoveAllListeners();
        dailyRewardButton.onClick.AddListener(DailyReward);

        if (SaveLoadManager.Instance.GameData.isPurchaseAds)
        {
            removeAds.interactable = false;
        }
        else
        {
            removeAds.interactable = true;
        }

        string dayReceive = DateTime.Now.Date.ToString();
        if (PlayerPrefs.HasKey("DailyRewardDate"))
        {
            string dayLastReceive = PlayerPrefs.GetString("DailyRewardDate", dayReceive);
            if (dayReceive.CompareTo(dayLastReceive) == 0)
            {
                SetOpenDailyReward(false);
            }
            else
            {
                SetOpenDailyReward(true);
            }
        }
        else
        {
            SetOpenDailyReward(true);
        }

        rect.localScale = Vector2.zero;
        rect.DOScale(1, 0.5f).SetEase(Ease.OutBack);

        for (int i = 0; i < contentRect.childCount; i++)
        {
            Destroy(contentRect.GetChild(i).gameObject);
        }
        foreach(var shopSlotData in shopConfig.shopSlotDatas)
        {
            UIShopSlot slot = Instantiate(shopSlotprefab, contentRect);
            slot.Setup(shopSlotData);
        }
    }
    [Button]
    public void Hide()
    {
        rect.DOScale(0, 0.5f).SetEase(Ease.InBack);
        if (UIManager.Instance != null) UIManager.Instance.ShowAllButtons();
    }

    private void RemoveAds()
    {
        PopupManager.Instance.ShowAnnounce(removeAdsDict[SaveLoadManager.Instance.GameData.language]);
        SaveLoadManager.Instance.GameData.isPurchaseAds = true;
        SaveLoadManager.Instance.Save();
        removeAds.interactable = false;
    }

    private void DailyReward()
    {
        PopupManager.Instance.ShowDailyReward(dailyRewardConfig);
        string dayReceive = DateTime.Now.Date.ToString();
        PlayerPrefs.SetString("DailyRewardDate", dayReceive);
        SetOpenDailyReward(false);
    }
    private void SetOpenDailyReward(bool isOpen)
    {
        if (isOpen)
        {
            dailyRewardButton.interactable = true;
            haloDailyRewardRect.gameObject.SetActive(true);
            comeBackTomorrowRect.gameObject.SetActive(false);
            dailyRewardButton.image.sprite = dailyRewardOpenSprite;
            dailyRewardButton.GetComponent<Animator>().enabled = true;
        }
        else
        {
            dailyRewardButton.interactable = false;
            haloDailyRewardRect.gameObject.SetActive(false);
            comeBackTomorrowRect.gameObject.SetActive(true);
            dailyRewardButton.image.sprite = dailyRewardCloseSprite;
            dailyRewardButton.GetComponent<Animator>().enabled = false;
        }
    }
}
