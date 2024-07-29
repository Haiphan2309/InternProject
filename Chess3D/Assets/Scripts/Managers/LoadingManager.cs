using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using GDC.Events;
using GDC.Enums;
using NaughtyAttributes;
using UnityEngine.UI;
using DG.Tweening;

namespace GDC.Managers
{
    public class LoadingManager : MonoBehaviour
    {
        public static LoadingManager Instance {get; private set;}
        [SerializeField] GameObject loadingCanvas;
        [SerializeField] Image loadingIcon;
        [SerializeField] List<Sprite> loadingSprites;
        [SerializeField] TipConfig tipConfig;
        [SerializeField] TMP_Text loadingText, tipText;
        [SerializeField] LanguageDictionary loadingDict;
        List<TipData> tipDatas;
        void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }
        // Start is called before the first frame update
        void Start()
        {
            GameEvents.ON_LOADING += HandleLoading;   
        }
        void OnDestroy()
        {
            GameEvents.ON_LOADING -= HandleLoading;   
        }
        void HandleLoading(bool isLoading)
        {
            if (isLoading)
                Debug.Log("start loading");
            else Debug.Log("end loading");
            this.loadingCanvas.SetActive(isLoading);
            if (isLoading)
            {
                int rand = Random.Range(0, loadingSprites.Count);
                loadingIcon.sprite = loadingSprites[rand];
                loadingIcon.SetNativeSize();

                loadingText.text = loadingDict[SaveLoadManager.Instance.GameData.language];

                if (tipDatas == null)
                {
                    tipDatas = new List<TipData>();
                }
                tipDatas.Clear();
                foreach(var tipData in tipConfig.commonTipDatas)
                {
                    tipDatas.Add(tipData);
                }
                foreach(var tipData in tipConfig.tipChapterDatas[SaveLoadManager.Instance.CacheData.currentChapter].tipDatas)
                {
                    tipDatas.Add(tipData);
                }
                    
                int rand2 = Random.Range(0, tipDatas.Count);
                tipText.text = tipDatas[rand2].tipDict[SaveLoadManager.Instance.GameData.language];
            }    
        }
    }
}
