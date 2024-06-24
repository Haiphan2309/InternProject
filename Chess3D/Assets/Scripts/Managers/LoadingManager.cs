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
        [SerializeField] Sprite slime, bigSlime, spikeSlime, fireSlime, poisonSlime, metalSlime, goldenSlime, iceSlime;
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
                //todo
            }    
        }
    }
}
