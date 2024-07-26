using GDC.Managers;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdsManager : MonoBehaviour
{
    public static AdsManager Instance { get; private set; }

    [SerializeField] private AdsInitializer adsInitializer;
    [SerializeField] private InterstitialAdsButton interstitialAds;
    [SerializeField] private int initLoadRemainToAds = 3;
    [SerializeField, ReadOnly] private int loadRemainToAds;
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
}
