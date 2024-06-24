using System.Collections.Generic;
using UnityEngine;
using System;
using GDC.Configuration;
using GDC.Enums;
using GDC.Constants;

namespace GDC.Managers
{

    [Serializable]
    public struct GameData
    {
        public bool IsSaveLoadProcessing;

        public int Coin;
        public string PlayerName;
        public void SetupData() //load
        {
            IsSaveLoadProcessing = true;
            GameDataOrigin gameDataOrigin = SaveLoadManager.Instance.GameDataOrigin;

            Coin = gameDataOrigin.Coin;
            PlayerName = gameDataOrigin.PlayerName;

            SaveLoadManager.Instance.GameDataOrigin = gameDataOrigin;
            IsSaveLoadProcessing = false;
        }
        public GameDataOrigin ConvertToGameDataOrigin() //save
        {
            IsSaveLoadProcessing = true;
            GameDataOrigin gameDataOrigin = new GameDataOrigin();

            gameDataOrigin.Coin = Coin;
            gameDataOrigin.PlayerName = PlayerName;

            IsSaveLoadProcessing = false;
            return gameDataOrigin;
        }

    }

    [Serializable]
    public struct GameDataOrigin
    {
        public bool IsHaveSaveData;
        public int Coin;
        public string PlayerName;
    }

    [Serializable]
    public struct CacheData
    {

    }
}
