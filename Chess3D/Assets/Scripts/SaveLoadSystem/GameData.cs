using System.Collections.Generic;
using UnityEngine;
using System;
using GDC.Configuration;
using GDC.Enums;
using GDC.Constants;
using Unity.VisualScripting;

namespace GDC.Managers
{
    [Serializable]
    public class PlayerLevelData
    {
        public int star;
        public int turn;

        public PlayerLevelData(int star, int turn)
        {
            this.star = star;
            this.turn = turn;
        }
    }
    [Serializable]
    public struct GameData
    {
        public bool IsSaveLoadProcessing;

        //public int coin;
        //public string playerName;
        public List<PlayerLevelData> playerLevelDatas;
        public int currentLevel; //start with 0
        public void SetupData() //load
        {
            IsSaveLoadProcessing = true;
            GameDataOrigin gameDataOrigin = SaveLoadManager.Instance.GameDataOrigin;

            //coin = gameDataOrigin.coin;
            //playerName = gameDataOrigin.playerName;

            if (gameDataOrigin.playerLevelStars == null)
            {
                gameDataOrigin.playerLevelStars = new List<int>();
                gameDataOrigin.playerLevelTurns = new List<int>();
            }
            if (gameDataOrigin.playerLevelStars.Count == 0) 
            {
                for (int i=0; i<GameConstants.MAX_LEVEL; i++)
                {
                    gameDataOrigin.playerLevelStars.Add(0);
                    gameDataOrigin.playerLevelTurns.Add(99);
                }
            }

            if (playerLevelDatas == null)
                playerLevelDatas = new List<PlayerLevelData>();
            for(int i=0; i<GameConstants.MAX_LEVEL; i++)
            {
                playerLevelDatas.Add(new PlayerLevelData(gameDataOrigin.playerLevelStars[i], gameDataOrigin.playerLevelTurns[i]));
            }
            currentLevel = gameDataOrigin.currentLevel;

            SaveLoadManager.Instance.GameDataOrigin = gameDataOrigin;
            IsSaveLoadProcessing = false;
        }
        public GameDataOrigin ConvertToGameDataOrigin() //save
        {
            IsSaveLoadProcessing = true;
            GameDataOrigin gameDataOrigin = new GameDataOrigin();

            //gameDataOrigin.coin = coin;
            //gameDataOrigin.playerName = playerName;
            gameDataOrigin.playerLevelStars = new List<int>();
            gameDataOrigin.playerLevelTurns = new List<int>();
            if (playerLevelDatas != null)
            {
                foreach(var playerLevelData in playerLevelDatas)
                {
                    gameDataOrigin.playerLevelStars.Add(playerLevelData.star);
                    gameDataOrigin.playerLevelTurns.Add(playerLevelData.turn);
                }
            }
            gameDataOrigin.currentLevel = currentLevel;

            IsSaveLoadProcessing = false;
            return gameDataOrigin;
        }

    }

    [Serializable]
    public struct GameDataOrigin
    {
        public bool IsHaveSaveData;
        //public int coin;
        //public string playerName;
        public List<int> playerLevelTurns;
        public List<int> playerLevelStars;
        public int currentLevel;
    }

    [Serializable]
    public struct CacheData
    {

    }
}
