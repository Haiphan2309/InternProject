using GDC.Managers;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIShopSlot : MonoBehaviour
{
    [SerializeField] Image iconImage, bgImage;
    [SerializeField] TMP_Text amountText, costText;
    [SerializeField, ReadOnly] ShopSlotData shopSlotData;

    public void Setup(ShopSlotData shopSlotData)
    {
        this.shopSlotData = shopSlotData;
        iconImage.sprite = shopSlotData.icon;
        bgImage.sprite = shopSlotData.bg;
        amountText.text = shopSlotData.slotName + " x" + shopSlotData.amount.ToString();
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
}
