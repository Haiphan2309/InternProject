using GDC.Managers;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIShopSlot : MonoBehaviour
{
    [SerializeField] RectTransform rect;
    [SerializeField] Image iconImage, bgImage;
    [SerializeField] TMP_Text amountText, costText;
    [SerializeField, ReadOnly] ShopSlotData shopSlotData;
    [SerializeField] LanguageDictionary thankYouDict, sorryDict;

    public void Setup(ShopSlotData shopSlotData)
    {
        this.shopSlotData = shopSlotData;
        iconImage.sprite = shopSlotData.icon;
        bgImage.sprite = shopSlotData.bg;

        amountText.text = shopSlotData.slotName;
        //if (shopSlotData.undoAmount > 0)
        //{
        //    amountText.text = shopSlotData.slotName + " x" + shopSlotData.undoAmount.ToString();
        //}
        //else
        //{
        //    amountText.text = shopSlotData.slotName + " x" + shopSlotData.solveAmount.ToString();
        //}   

        switch (SaveLoadManager.Instance.GameData.language)
        {
            case GDC.Enums.Language.English:
                costText.text = shopSlotData.costUSD.ToString() + " USD";
                break;
            case GDC.Enums.Language.Vietnamese:
                costText.text = shopSlotData.costVND.ToString() + " VND";
                break;
        }

    }

    public void OnDown()
    {
        rect.DOScale(0.9f, 0.2f);
    }
    public void OnExit()
    {
        rect.DOScale(1, 0.25f).SetEase(Ease.OutBack);
    }
    public void OnClick()
    {
        rect.DOScale(1, 0.25f).SetEase(Ease.OutBack);
        Debug.Log("Da thanh toan so tien " + costText.text + " va mua duoc " + amountText.text);
        //PopupManager.Instance.ShowAnnounce(thankYouDict[SaveLoadManager.Instance.GameData.language]);
        PopupManager.Instance.ShowShopReward(shopSlotData, thankYouDict[SaveLoadManager.Instance.GameData.language]);
        //SaveLoadManager.Instance.GameData.undoNum += shopSlotData.undoAmount;
        //SaveLoadManager.Instance.GameData.solveNum += shopSlotData.solveAmount;      
        //SaveLoadManager.Instance.Save();
    }    
}