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

    [SerializeField] private Sprite undoSprite, solveSprite, turnSprite;
    public void Setup(RewardData rewardData)
    {
        
        if (rewardData.undoAmount > 0)
        {
            iconImage.sprite = undoSprite;
            nameText.text = "Undo x" + rewardData.undoAmount.ToString();
        }
        else if (rewardData.solveAmount > 0)
        {
            iconImage.sprite = solveSprite;
            nameText.text = "Hint x" + rewardData.solveAmount.ToString();
        }
        else //if (rewardData.turnAmount > 0)
        {
            iconImage.sprite = turnSprite;
            nameText.text = "Turn x" + rewardData.turnAmount.ToString();
        }
    }

    //public void Setup(ShopSlotData shopSlotData)
    //{
    //    nameText.text = shopSlotData.slotName;
    //    iconImage.sprite = shopSlotData.icon;
    //}
}
