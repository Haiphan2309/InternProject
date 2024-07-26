using DG.Tweening;
using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GDC.Managers;
using TMPro;

public class UIShopManager : MonoBehaviour
{
    [SerializeField] private ShopConfig shopConfig;
    [SerializeField] private DailyRewardConfig dailyRewardConfig;
    [SerializeField] private RectTransform rect, contentRect, haloDailyRewardRect, comeBackTomorrowRect, playerInfoRect, dailyAdsLimitRect;
    [SerializeField] private UIShopSlot shopSlotprefab;

    [SerializeField] private Button exitButton, removeAds, dailyRewardButton;
    [SerializeField] private LanguageDictionary removeAdsDict;
    [SerializeField] private Sprite dailyRewardOpenSprite, dailyRewardCloseSprite;
    //[SerializeField] private int maxAdsLimit;
    [SerializeField,ReadOnly] private int numAdsRemainCanWatch;
    [SerializeField] private TMP_Text adsLimitText;

    [Header("Player Item Info")]
    [SerializeField] private TMP_Text undoNumText;
    [SerializeField] private TMP_Text hintNumText, turnNumText;

    [Button]
    public void Show()
    {
        SoundManager.Instance.PlaySound(AudioPlayer.SoundID.SFX_UI_SHOW);
        exitButton.onClick.RemoveAllListeners();
        exitButton.onClick.AddListener(Hide);

        removeAds.onClick.RemoveAllListeners();
        removeAds.onClick.AddListener(RemoveAds);

        dailyRewardButton.onClick.RemoveAllListeners();
        dailyRewardButton.onClick.AddListener(OnClickDailyReward);

        if (SaveLoadManager.Instance.GameData.isPurchaseAds)
        {
            removeAds.interactable = false;
        }
        else
        {
            removeAds.interactable = true;
        }

        string dayReceive = DateTime.Now.Date.ToString();
        if (PlayerPrefs.HasKey("DailyRewardDate"))
        {
            string dayLastReceive = PlayerPrefs.GetString("DailyRewardDate", dayReceive);
            numAdsRemainCanWatch = PlayerPrefs.GetInt("AdsRewardRemain", shopConfig.maxDailyAdsLimit);
            if (dayReceive.CompareTo(dayLastReceive) == 0)
            {
                CheckAdsRemainCanWatch();
            }
            else
            {
                PlayerPrefs.SetInt("AdsRewardRemain", shopConfig.maxDailyAdsLimit);
                numAdsRemainCanWatch = PlayerPrefs.GetInt("AdsRewardRemain", shopConfig.maxDailyAdsLimit);
                SetOpenDailyReward(true);
            }
        }
        else
        {
            SetOpenDailyReward(true);
        }

        rect.localScale = Vector2.zero;
        rect.DOScale(1, 0.5f).SetEase(Ease.OutBack);

        playerInfoRect.DOAnchorPosY(-60, 0.5f);

        for (int i = 0; i < contentRect.childCount; i++)
        {
            Destroy(contentRect.GetChild(i).gameObject);
        }
        foreach(var shopSlotData in shopConfig.shopSlotDatas)
        {
            UIShopSlot slot = Instantiate(shopSlotprefab, contentRect);
            slot.Setup(shopSlotData, this);
        }
        UpdatePlayerInfo();
    }
    [Button]
    public void Hide()
    {
        SoundManager.Instance.PlaySound(AudioPlayer.SoundID.SFX_BUTTON_CLICK);
        playerInfoRect.DOAnchorPosY(200, 0.5f);
        rect.DOScale(0, 0.5f).SetEase(Ease.InBack);
        PopupManager.Instance.HideBlackBg();
        if (UIManager.Instance != null) UIManager.Instance.ShowAllButtons();
    }

    private void RemoveAds()
    {
        SoundManager.Instance.PlaySound(AudioPlayer.SoundID.SFX_BUTTON_CLICK);
        PopupManager.Instance.ShowAnnounce(removeAdsDict[SaveLoadManager.Instance.GameData.language]);
        SaveLoadManager.Instance.GameData.isPurchaseAds = true;
        SaveLoadManager.Instance.Save();
        removeAds.interactable = false;
    }

    public void UpdatePlayerInfo()
    {
        hintNumText.text = SaveLoadManager.Instance.GameData.solveNum.ToString();
        turnNumText.text = SaveLoadManager.Instance.GameData.turnNum.ToString();
        undoNumText.text = SaveLoadManager.Instance.GameData.undoNum.ToString();
    }    

    private void OnClickDailyReward()
    {
        string dayReceive = DateTime.Now.Date.ToString();
        if (PlayerPrefs.HasKey("DailyRewardDate"))
        {
            string dayLastReceive = PlayerPrefs.GetString("DailyRewardDate", dayReceive);
            if (dayReceive.CompareTo(dayLastReceive) != 0) //Neu nhan qua lan dau trong ngay
            {
                ReceiveDailyReward();
                PlayerPrefs.SetString("DailyRewardDate", dayReceive);
                PlayerPrefs.SetInt("AdsRewardRemain", shopConfig.maxDailyAdsLimit);
                numAdsRemainCanWatch = PlayerPrefs.GetInt("AdsRewardRemain", shopConfig.maxDailyAdsLimit);
            }
            else //Cac lan nhan sau do se phai xem quang cao cho toi khi het gioi han quang cao co the xem
            {
                AdsManager.Instance.ON_REWARD_DAILY_ADS += ReceiveDailyReward;
                AdsManager.Instance.ShowRewardAds();
                CheckAdsRemainCanWatch(true);
            }
        }
        else
        {
            ReceiveDailyReward();
            PlayerPrefs.SetString("DailyRewardDate", dayReceive);
            PlayerPrefs.SetInt("AdsRewardRemain", shopConfig.maxDailyAdsLimit);
            numAdsRemainCanWatch = PlayerPrefs.GetInt("AdsRewardRemain", shopConfig.maxDailyAdsLimit);
            CheckAdsRemainCanWatch();
        }
    }
    private void ReceiveDailyReward()
    {
        SoundManager.Instance.PlaySound(AudioPlayer.SoundID.SFX_OPEN_CHEST);
        PopupManager.Instance.ShowDailyReward(dailyRewardConfig);
        UpdatePlayerInfo();
        AdsManager.Instance.ON_REWARD_DAILY_ADS -= ReceiveDailyReward;
    }
    private void CheckAdsRemainCanWatch(bool isReduce = false)
    {
        if (isReduce)
        {
            PlayerPrefs.SetInt("AdsRewardRemain", numAdsRemainCanWatch-1);
            numAdsRemainCanWatch = PlayerPrefs.GetInt("AdsRewardRemain", shopConfig.maxDailyAdsLimit);
            //numAdsRemainCanWatch--;
        }

        Debug.Log("Num ads remain: "+ numAdsRemainCanWatch);
        if (numAdsRemainCanWatch <= 0)
        {
            SetOpenDailyReward(false);
        }
        else
        {
            SetOpenDailyReward(true, true);
        }
    }
    private void SetOpenDailyReward(bool isOpen, bool isWatchAds = false)
    {
        if (isOpen)
        {
            dailyRewardButton.interactable = true;
            haloDailyRewardRect.gameObject.SetActive(true);
            comeBackTomorrowRect.gameObject.SetActive(false);
            dailyRewardButton.image.sprite = dailyRewardOpenSprite;
            dailyRewardButton.GetComponent<Animator>().enabled = true;
            if (isWatchAds)
            {
                dailyAdsLimitRect.gameObject.SetActive(true);
                adsLimitText.text = numAdsRemainCanWatch.ToString();
            }    
            else
            {
                dailyAdsLimitRect.gameObject.SetActive(false);               
            }
        }
        else
        {
            dailyRewardButton.interactable = false;
            haloDailyRewardRect.gameObject.SetActive(false);
            comeBackTomorrowRect.gameObject.SetActive(true);
            dailyRewardButton.image.sprite = dailyRewardCloseSprite;
            dailyRewardButton.GetComponent<Animator>().enabled = false;
            dailyAdsLimitRect.gameObject.SetActive(false);
        }
    }
}
