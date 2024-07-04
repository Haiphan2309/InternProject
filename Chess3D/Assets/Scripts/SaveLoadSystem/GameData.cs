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
        public int highScore;

        public PlayerLevelData(int star, int highScore)
        {
            this.star = star;
            this.highScore = highScore;
        }
    }

    [Serializable]
    public struct GameData
    {
        public bool IsSaveLoadProcessing;

        //public int coin;
        //public string playerName;
        public List<PlayerLevelData> playerLevelDatas;
        public int currentChapter;
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
                gameDataOrigin.playerLevelHighScores = new List<int>();
            }
            if (gameDataOrigin.playerLevelStars.Count == 0) 
            {
                for (int i=0; i<GameConstants.MAX_LEVEL; i++)
                {
                    gameDataOrigin.playerLevelStars.Add(0);
                    gameDataOrigin.playerLevelHighScores.Add(99);
                }
            }

            if (playerLevelDatas == null)
                playerLevelDatas = new List<PlayerLevelData>();
            for(int i=0; i<GameConstants.MAX_LEVEL; i++)
            {
                playerLevelDatas.Add(new PlayerLevelData(gameDataOrigin.playerLevelStars[i], gameDataOrigin.playerLevelHighScores[i]));
            }
            currentLevel = gameDataOrigin.currentLevel;
            currentChapter = gameDataOrigin.currentChapter;

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
            gameDataOrigin.playerLevelHighScores = new List<int>();
            if (playerLevelDatas != null)
            {
                foreach(var playerLevelData in playerLevelDatas)
                {
                    gameDataOrigin.playerLevelStars.Add(playerLevelData.star);
                    gameDataOrigin.playerLevelHighScores.Add(playerLevelData.highScore);
                }
            }
            gameDataOrigin.currentLevel = currentLevel;
            gameDataOrigin.currentChapter = currentChapter;

            IsSaveLoadProcessing = false;
            return gameDataOrigin;
        }

        #region support function
        public int GetLevelStar(int chapterId, int levelId)
        {
            return playerLevelDatas[chapterId * GameConstants.MAX_LEVEL + levelId].star;
        }
        public int GetLevelHighScore(int chapterId, int levelId)
        {
            return playerLevelDatas[chapterId * GameConstants.MAX_LEVEL + levelId].highScore;
        }
        public int GetAllStar()
        {
            int t = 0;
            for (int i = 0; i < GameConstants.MAX_CHAPTER; i++) 
            {
                for (int j = 0; j < GameConstants.MAX_LEVEL; j++)
                {
                    t += GetLevelStar(i, j);
                }
            }
            return t;
        }
        #endregion
    }

    [Serializable]
    public struct GameDataOrigin
    {
        public bool IsHaveSaveData;
        //public int coin;
        //public string playerName;
        public List<int> playerLevelHighScores;
        public List<int> playerLevelStars;
        public int currentLevel;
        public int currentChapter;
    }

    [Serializable]
    public struct CacheData
    {

    }
}
