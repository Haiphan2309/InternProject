using DG.Tweening;
using GDC.Managers;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIShopManager : MonoBehaviour
{
    [SerializeField] ShopConfig shopConfig;
    [SerializeField] RectTransform rect, contentRect;
    [SerializeField] UIShopSlot shopSlotprefab;

    [SerializeField] Button exitButton, removeAds;
    [SerializeField] LanguageDictionary removeAdsDict;

    [Button]
    public void Show()
    {
        exitButton.onClick.RemoveAllListeners();
        exitButton.onClick.AddListener(Hide);

        removeAds.onClick.RemoveAllListeners();
        removeAds.onClick.AddListener(RemoveAds);

        if (SaveLoadManager.Instance.GameData.isPurchaseAds)
        {
            removeAds.interactable = false;
        }
        else
        {
            removeAds.interactable = true;
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
    }

    void RemoveAds()
    {
        PopupManager.Instance.ShowAnnounce(removeAdsDict[SaveLoadManager.Instance.GameData.language]);
        SaveLoadManager.Instance.GameData.isPurchaseAds = true;
        SaveLoadManager.Instance.Save();
        removeAds.interactable = false;
    }
}
