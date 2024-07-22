using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupManager : MonoBehaviour
{
    public static PopupManager Instance { get; private set; }

    [SerializeField] private Transform canvasTrans;
    [SerializeField] private Image blackBgPrefab;
    private Stack<Image> blackBgStack = new Stack<Image>();
    [SerializeField] private UIAnnounce uiAnnouncePrefab;
    [SerializeField] private UIReward uiRewardPrefab;
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
    public void ShowBlackBg()
    {
        Image blackBgImage = Instantiate(blackBgPrefab, canvasTrans);
        blackBgImage.color = Color.clear;
        blackBgImage.DOFade(0.5f, 0.3f);
        if (blackBgStack == null)
        {
            blackBgStack = new Stack<Image>();
        }    
        blackBgStack.Push(blackBgImage);
    }
    public void HideBlackBg()
    {
        Image blackBgImage = blackBgStack?.Pop();
        blackBgImage.DOFade(0.5f, 0.3f).OnComplete(() => Destroy(blackBgImage.gameObject));
    }
    public void ShowAnnounce(string text)
    {
        ShowBlackBg();
        UIAnnounce uiAnnounce = Instantiate(uiAnnouncePrefab, canvasTrans);
        uiAnnounce.Show(text);
    }
    public void ShowShopReward(ShopSlotData shopSlotData, string des)
    {
        ShowBlackBg();
        UIReward uiReward = Instantiate(uiRewardPrefab, canvasTrans);
        uiReward.Show(shopSlotData, des);
    }
    public void ShowDailyReward(DailyRewardConfig config)
    {
        ShowBlackBg();
        UIReward uiReward = Instantiate(uiRewardPrefab, canvasTrans);
        uiReward.Show(config);
    }
}
