using GDC.Constants;
using GDC.Managers;
using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdsManager : MonoBehaviour
{
    public static AdsManager Instance { get; private set; }

    [SerializeField] private AdsInitializer adsInitializer;
    [SerializeField] private InterstitialAdsButton interstitialAds;
    [SerializeField] private RewardedAdsButton rewardedAds;
    [SerializeField] private int initLoadRemainToAds = 3;
    [SerializeField, ReadOnly] private int loadRemainToAds;

    public Action<int> ON_REWARD_TURN;
    public Action ON_REWARD_DAILY_ADS;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        adsInitializer.InitializeAds();
    }
    private void Start()
    {
        loadRemainToAds = initLoadRemainToAds;
        Debug.Log("init ads manager");
    }
    public void CheckLoadToAds()
    {
        if (SaveLoadManager.Instance.GameData.isPurchaseAds) return;

        loadRemainToAds--;
        Debug.Log("CHECK ADS " + loadRemainToAds);
        if (loadRemainToAds <=0)
        {
            //show ads
            //interstitialAds.LoadAd();
            //interstitialAds.ShowAd();
            loadRemainToAds = initLoadRemainToAds;
        }
    }
    public void ShowRewardAds()
    {
        //rewardedAds.LoadAd();
        //rewardedAds.ShowAd();

        ON_REWARD_DAILY_ADS?.Invoke();
        ON_REWARD_TURN?.Invoke(GameConstants.TURN_REWARD);
    }
}
