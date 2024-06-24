using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GDC.Managers;
using NaughtyAttributes;

namespace GDC.Managers
{
    public class SaveLoadManager : MonoBehaviour
    {
        static public SaveLoadManager Instance { get; private set; }
        public GameDataOrigin GameDataOrigin;
        public GameData GameData;
        public CacheData CacheData;

        //[SerializeField] SO_Item so_defaultArmor, so_defaultShoe;

        //public SaveLoadSystem saveLoadSystem;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        [Button]
        public void Save()
        {
            //GameData.IsHaveSaveData = true;
            GameDataOrigin = GameData.ConvertToGameDataOrigin();            
            //StartCoroutine(Cor_SaveLoadProgress("Save success"));
            //print("SAVE game");
        }
        [Button]
        public void Load()
        {
            SaveLoadSystem.LoadData(GameDataOrigin);
            GameData.SetupData();
            
            //StartCoroutine(Cor_LoadPlayer());
            //StartCoroutine(Cor_SaveLoadProgress("load success"));
            //Debug.Log("Load successed");
        }
        [Button]
        public void ResetData()
        {
            GameDataOrigin gamedataOrigin = new GameDataOrigin();
            GameDataOrigin = gamedataOrigin;
            //GameDataOrigin.ArmorEquipID = so_defaultArmor.ID;
            //GameDataOrigin.ShoeEquipID = so_defaultShoe.ID;
            //if (GameDataOrigin.SeenItemIDList == null)
            //    GameDataOrigin.SeenItemIDList = new List<string>();
            //GameDataOrigin.SeenItemIDList.Add(so_defaultArmor.ID);
            //GameDataOrigin.SeenItemIDList.Add(so_defaultShoe.ID);
            SaveLoadSystem.SaveData(GameDataOrigin);
            //GameData.SetupData();
            //StartCoroutine(Cor_ResetData());
            //Debug.Log("Reset data successed");
        }
       
        //IEnumerator Cor_SaveLoadProgress(string progressStr)
        //{
        //    yield return new WaitUntil(()=>this.GameData.IsSaveLoadProcessing == false);
        //    SaveLoadSystem.SaveData(GameDataOrigin);
        //    Debug.Log(progressStr);
        //}    
        //IEnumerator Cor_ResetData()
        //{
        //    //Save();
        //    //yield return new WaitUntil(() => this.GameData.IsSaveLoadProcessing == false);
        //    Load();
        //    yield return new WaitUntil(() => this.GameData.IsSaveLoadProcessing == false);
        //    StartCoroutine(Cor_LoadPlayer());
        //    StartCoroutine(Cor_SaveLoadProgress("reset data success"));
        //}    
    }
}
