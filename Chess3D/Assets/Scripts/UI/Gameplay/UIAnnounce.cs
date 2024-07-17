using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIAnnounce : MonoBehaviour
{
    [SerializeField] UIPopupAnim uiPopupAnim;
    [SerializeField] TMP_Text announceText;
    [SerializeField] Button hideButton;
    public void Show(string text)
    {
        hideButton.onClick.RemoveAllListeners();
        hideButton.onClick.AddListener(Hide);
        uiPopupAnim.Show();
        announceText.text = text;
    }
    void Hide()
    {
        uiPopupAnim.Hide();
        Destroy(gameObject, 1);
    }
}
