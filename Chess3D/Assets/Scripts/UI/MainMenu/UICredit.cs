using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICredit : MonoBehaviour
{
    [SerializeField] UIPopupAnim uiPopupAnim;

    [SerializeField] Button hideButton;
    public void Show()
    {
        hideButton.onClick.RemoveAllListeners();
        hideButton.onClick.AddListener(Hide);
        uiPopupAnim.Show();
    }
    public void Hide()
    {
        hideButton.onClick.RemoveAllListeners();
        PopupManager.Instance.HideBlackBg();
        uiPopupAnim.Hide();
    }
}
