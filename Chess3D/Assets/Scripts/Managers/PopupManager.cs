using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupManager : MonoBehaviour
{
    public static PopupManager Instance { get; private set; }

    [SerializeField] Transform canvasTrans;
    [SerializeField] UIAnnounce uiAnnouncePrefab;
    [SerializeField] UIReward uiRewardPrefab;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public void ShowAnnounce(string text)
    {
        UIAnnounce uiAnnounce = Instantiate(uiAnnouncePrefab, canvasTrans);
        uiAnnounce.Show(text);
    }
    public void ShowShopReward(ShopSlotData shopSlotData, string des)
    {
        UIReward uiReward = Instantiate(uiRewardPrefab, canvasTrans);
        uiReward.Show(shopSlotData, des);
    }
    public void ShowDailyReward(DailyRewardConfig config)
    {
        UIReward uiReward = Instantiate(uiRewardPrefab, canvasTrans);
        uiReward.Show(config);
    }
}
