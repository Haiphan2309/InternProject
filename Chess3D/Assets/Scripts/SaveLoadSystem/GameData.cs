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
    public struct GameData //Chua cac data su dung trong game
    {
        public bool IsSaveLoadProcessing;

        public bool isHaveSaveData;

        //public int coin;
        //public string playerName;
        public List<PlayerLevelData> playerLevelDatas;

        public List<int> currentLevelOfChapters;

        public Language language;
        public int undoNum, solveNum, turnNum;
        public bool isPurchaseAds;
        public void SetupData() //load
        {
            IsSaveLoadProcessing = true;
            GameDataOrigin gameDataOrigin = SaveLoadManager.Instance.GameDataOrigin;

            //coin = gameDataOrigin.coin;
            //playerName = gameDataOrigin.playerName;

            isHaveSaveData = gameDataOrigin.IsHaveSaveData;

            if (gameDataOrigin.playerLevelStars == null)
            {
                gameDataOrigin.playerLevelStars = new List<int>();
                gameDataOrigin.playerLevelHighScores = new List<int>();
                gameDataOrigin.isPlayBefores = new List<bool>();
            }
            if (gameDataOrigin.playerLevelStars.Count < GameConstants.MAX_CHAPTER * GameConstants.MAX_LEVEL) 
            {
                for (int i= gameDataOrigin.playerLevelStars.Count; i< GameConstants.MAX_CHAPTER * GameConstants.MAX_LEVEL; i++)
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

            if (gameDataOrigin.currentLevelOfChapters == null)
            {
                gameDataOrigin.currentLevelOfChapters = new List<int>();
            }    
            if (gameDataOrigin.currentLevelOfChapters.Count < GameConstants.MAX_CHAPTER)
            {     
                for (int i = gameDataOrigin.currentLevelOfChapters.Count; i < GameConstants.MAX_CHAPTER; i++)
                {
                    gameDataOrigin.currentLevelOfChapters.Add(0);
                }
            }

            if (currentLevelOfChapters == null)
            {
                currentLevelOfChapters = new List<int>();
            }
            if (currentLevelOfChapters.Count < GameConstants.MAX_CHAPTER)
            {
                for (int i = currentLevelOfChapters.Count; i < GameConstants.MAX_CHAPTER; i++)
                {
                    currentLevelOfChapters.Add(gameDataOrigin.currentLevelOfChapters[i]);
                }
            }

            language = (Language)gameDataOrigin.languageId;
            if (isHaveSaveData)
            {
                undoNum = gameDataOrigin.undoNum;
                solveNum = gameDataOrigin.solveNum;
                turnNum = gameDataOrigin.turnNum;
            }
            else
            {
                ReceiveInitItem();
            }

            isPurchaseAds = gameDataOrigin.isPurchaseAds;

            SaveLoadManager.Instance.GameDataOrigin = gameDataOrigin;
            IsSaveLoadProcessing = false;
        }
        public GameDataOrigin ConvertToGameDataOrigin() //save
        {
            IsSaveLoadProcessing = true;
            GameDataOrigin gameDataOrigin = new GameDataOrigin();

            gameDataOrigin.IsHaveSaveData = true;

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
            gameDataOrigin.currentLevelOfChapters = currentLevelOfChapters;

            gameDataOrigin.languageId = (int)language;
            gameDataOrigin.undoNum = undoNum;
            gameDataOrigin.solveNum = solveNum;
            gameDataOrigin.turnNum = turnNum;
            gameDataOrigin.isPurchaseAds = isPurchaseAds;

            IsSaveLoadProcessing = false;
            return gameDataOrigin;
        }

        #region support function
        public void ReceiveInitItem()
        {
            solveNum = 3;
            turnNum = 5;
            undoNum = 5;
        }
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
            //int oldIndex = this.currentChapter * GameConstants.MAX_LEVEL + this.currentLevel;

            //winLevelId++;
            //if (winLevelId == GameUtils.GetChapterData(winChapterId).levelDatas.Count)
            //{
            //    winLevelId = 0;
            //    winChapterId++;
            //}
            //int index = winChapterId * GameConstants.MAX_LEVEL + winLevelId;
            //Debug.Log(winChapterId + " " + winLevelId + " " + index + " " + oldIndex);

            //if (index > oldIndex)
            //{
            //    this.currentChapter = winChapterId;
            //    this.currentLevel = winLevelId;
            //}

            Debug.Log(winLevelId + " " + (GameUtils.GetChapterData(winChapterId).levelDatas.Count - 1));
            if (winLevelId < GameUtils.GetChapterData(winChapterId).levelDatas.Count - 1)
            {
                winLevelId++;
                Debug.Log("Increase win level index");
            }
            if (currentLevelOfChapters[winChapterId] < winLevelId)
            {
                currentLevelOfChapters[winChapterId] = winLevelId;
            }
        }
        #endregion
    }

    [Serializable]
    public struct GameDataOrigin //Chua cac data luu xuong thanh dang .bin
    {
        public bool IsHaveSaveData;
        //public int coin;
        //public string playerName;
        public List<int> playerLevelHighScores;
        public List<int> playerLevelStars;
        public List<bool> isPlayBefores;
        public List<int> currentLevelOfChapters;

        public int languageId;
        public int undoNum, solveNum, turnNum;
        public bool isPurchaseAds;
    }

    [Serializable]
    public struct CacheData //Luu cac data tam thoi trong game, ko luu khi tat
    {
        public int currentChapter;
    }
}
