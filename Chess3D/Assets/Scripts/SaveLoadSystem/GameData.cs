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
        public bool isPlayBefore;

        public PlayerLevelData(int star, int highScore, bool isPlayBefore)
        {
            this.star = star;
            this.highScore = highScore;
            this.isPlayBefore = isPlayBefore;
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

        public Language language;
        public int undoNum, solveNum;
        public bool isPurchaseAds;
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
                gameDataOrigin.isPlayBefores = new List<bool>();
            }
            if (gameDataOrigin.playerLevelStars.Count == 0) 
            {
                for (int i=0; i< GameConstants.MAX_CHAPTER * GameConstants.MAX_LEVEL; i++)
                {
                    gameDataOrigin.playerLevelStars.Add(0);
                    gameDataOrigin.playerLevelHighScores.Add(99);
                    gameDataOrigin.isPlayBefores.Add(false);
                }
            }

            if (playerLevelDatas == null)
                playerLevelDatas = new List<PlayerLevelData>();
            playerLevelDatas.Clear();

            for(int i=0; i<GameConstants.MAX_CHAPTER* GameConstants.MAX_LEVEL; i++)
            {
                playerLevelDatas.Add(new PlayerLevelData(gameDataOrigin.playerLevelStars[i], gameDataOrigin.playerLevelHighScores[i], gameDataOrigin.isPlayBefores[i]));
            }

            currentLevel = gameDataOrigin.currentLevel;
            currentChapter = gameDataOrigin.currentChapter;

            language = (Language)gameDataOrigin.languageId;
            undoNum = gameDataOrigin.undoNum;
            solveNum = gameDataOrigin.solveNum;

            isPurchaseAds = gameDataOrigin.isPurchaseAds;

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
            gameDataOrigin.isPlayBefores = new List<bool>();
            if (playerLevelDatas != null)
            {
                foreach(var playerLevelData in playerLevelDatas)
                {
                    gameDataOrigin.playerLevelStars.Add(playerLevelData.star);
                    gameDataOrigin.playerLevelHighScores.Add(playerLevelData.highScore);
                    gameDataOrigin.isPlayBefores.Add(playerLevelData.isPlayBefore);
                }
            }
            gameDataOrigin.currentLevel = currentLevel;
            gameDataOrigin.currentChapter = currentChapter;

            gameDataOrigin.languageId = (int)language;
            gameDataOrigin.undoNum = undoNum;
            gameDataOrigin.solveNum = solveNum;
            gameDataOrigin.isPurchaseAds = isPurchaseAds;

            IsSaveLoadProcessing = false;
            return gameDataOrigin;
        }

        #region support function
        public bool CheckNewRecord(int chapterId, int levelId, int remainTurn)
        {
            int index = chapterId * GameConstants.MAX_LEVEL + levelId;
            return playerLevelDatas[index].highScore > remainTurn;
        }
        public void SetLevelData(int chapterId, int levelId, int star, int remainTurn)
        {
            int index = chapterId * GameConstants.MAX_LEVEL + levelId;
            if (star > playerLevelDatas[index].star) 
            {
                playerLevelDatas[index].star = star;
            }
            if (CheckNewRecord(chapterId, levelId, remainTurn))
            {
                playerLevelDatas[index].highScore = remainTurn;
            }
        }
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
        public bool CheckPlayedLevelBefore(int chapterId, int levelId)
        {
            int index = chapterId * GameConstants.MAX_LEVEL + levelId;
            return playerLevelDatas[index].isPlayBefore;
        }
        public void SetPlayedLevelBefore(int chapterId, int levelId, bool value)
        {
            int index = chapterId * GameConstants.MAX_LEVEL + levelId;
            playerLevelDatas[index].isPlayBefore = value;
        }
        public void CheckSetCurrentLevel(int winChapterId, int winLevelId)
        {
            int oldIndex = this.currentChapter + GameConstants.MAX_LEVEL + this.currentLevel;

            winLevelId++;
            if (winLevelId == GameConstants.MAX_LEVEL)
            {
                winLevelId = 0;
                winChapterId++;
            }
            int index = winChapterId * GameConstants.MAX_LEVEL + winLevelId;

            if (index > oldIndex)
            {
                this.currentChapter = winChapterId;
                this.currentLevel = winLevelId;
            }
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
        public List<bool> isPlayBefores;
        public int currentLevel;
        public int currentChapter;

        public int languageId;
        public int undoNum, solveNum;
        public bool isPurchaseAds;
    }

    [Serializable]
    public struct CacheData
    {

    }
}
