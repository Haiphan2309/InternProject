using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using GDC.Managers;

public class UIRewardSlot : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private Image iconImage;

    [SerializeField] private Sprite undoSprite, solveSprite;
    public void Setup(RewardData rewardData)
    {
        nameText.text = rewardData.rewardName;
        if (rewardData.undoAmount > 0)
        {
            iconImage.sprite = undoSprite;
        }
        else //if (rewardData.solveAmount > 0)
        {
            iconImage.sprite = solveSprite;
        }
    }

    public void Setup(ShopSlotData shopSlotData)
    {
        nameText.text = shopSlotData.slotName;
        iconImage.sprite = shopSlotData.icon;
    }
}
