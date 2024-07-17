using DG.Tweening;
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

    [SerializeField] Button exitButton;

    [Button]
    public void Show()
    {
        exitButton.onClick.RemoveAllListeners();
        exitButton.onClick.AddListener(Hide);

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
}
