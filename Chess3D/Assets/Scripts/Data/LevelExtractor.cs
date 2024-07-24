using GDC.Constants;
using GDC.Enums;
using NaughtyAttributes;
using RotaryHeart.Lib;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class LevelExtractor : MonoBehaviour
{
    // Just for testing, will be placed with Dynamic Load in future
    [SerializeField] Grid grid;
    [SerializeField] TileTypeData tileTypeData;

    string levelDataPath;

    public string storeLevelName = "";
    
    //
    TileInfo[,,] map = new TileInfo[GameConstants.MAX_X_SIZE, GameConstants.MAX_Y_SIZE, GameConstants.MAX_Z_SIZE];
    List<PlayerArmy> playerArmies = new List<PlayerArmy>();
    List<EnemyArmy> enemyArmies = new List<EnemyArmy>();

    //
    Dictionary<int, ChessManType> chessTypeDic;
    void Start()
    {
        levelDataPath = "Assets/Resources/ScriptableObjects/LevelData";
        InitChessTypeDic();
        
    }
    private void InitChessTypeDic()
    {
        chessTypeDic = new Dictionary<int, ChessManType>
        {
            { 0, ChessManType.PAWN },
            { 1, ChessManType.CASTLE },
            { 2, ChessManType.KNIGHT },
            { 3, ChessManType.BISHOP },
            { 4, ChessManType.QUEEN },
            { 5, ChessManType.KING }
        };
    }
    private TileType GetTileTypeById(int id)
    {
        
        foreach (var data in tileTypeData.tileTypeDatas)
        {
            if (data.idList.Contains(id))
            {
                return data.tileType;
            }
        }
        return TileType.NONE;
    }
   

    [Button]
    private void ExtractLevel()
    {
        ClearLocalStorage();
        UpdateDataFromLevel();
        SaveMapToLevelData();


    }

    private void SaveMapToLevelData()
    {
        
        // Create new Level Data
        LevelData newLevelData = ScriptableObject.CreateInstance<LevelData>();

        // Assign attribute to levelData
        newLevelData.SetData(map, playerArmies, enemyArmies);
       
        // Store levelData to real file
        string path = $"{levelDataPath}/{storeLevelName}.asset";

#if UNITY_EDITOR
        AssetDatabase.CreateAsset(newLevelData, path);
        AssetDatabase.SaveAssets();
#endif

        //
        Debug.Log("Level Data created successfully: " + path);

    }



    private void UpdateDataFromLevel()
    {
        //Loop all tilemap - Floor
        for (int i = 0; i < grid.transform.childCount; i++)
        {
            Transform floor = grid.transform.GetChild(i);
            for (int j = 0; j < floor.childCount; j++)
            {
                Transform block = floor.GetChild(j);
                int blockNumber;
                Vector3 blockPos = block.position;
                Vector3 indexVector = new Vector3((int)blockPos.x, (int)blockPos.y, (int)blockPos.z);

                // Check index
                if (indexVector.x < 0 || indexVector.y < 0 || indexVector.z < -0)
                {
                    Debug.LogError("Block position(x,y,z) must be positive");
                }
                // Get block name as int
                
                if (!int.TryParse(block.gameObject.name, out blockNumber))
                {
                    Debug.LogError("Block name is not correct, must be an integer: " + block.gameObject.name);
                }

                
                
                TileInfo newTileInfo = new TileInfo(blockNumber, GetTileTypeById(blockNumber));
                map[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z] = newTileInfo;
                 
                if (blockNumber >= GameConstants.playerChessIdBoundary.x && blockNumber  <= GameConstants.playerChessIdBoundary.y)// Id 300 - 305 -> Player Chess
                {
                    PlayerArmy playerArmy = new PlayerArmy(indexVector, chessTypeDic[blockNumber % 100]);
                    playerArmies.Add(playerArmy);
                }
                else if (blockNumber >= GameConstants.enemyChessIdBoundary.x && blockNumber <= GameConstants.enemyChessIdBoundary.y)// Id 400 - 405 -> Enemy Chess
                {
                    int id = blockNumber % 100;
                    if (id > 5) id -= 6; // AI Enemy store as Default in Dic
                    EnemyArmy enemyArmy = new EnemyArmy(indexVector, chessTypeDic[id]);
                    if (blockNumber > 405) // AI Enemy
                    {
                        enemyArmy.isAI = true;
                    }
                    enemyArmies.Add(enemyArmy);
                }
            }
        }
    }


    void ClearLocalStorage()
    {
        ClearTileInfoArray(map);
        playerArmies.Clear();
        enemyArmies.Clear();
    }
    void ClearTileInfoArray(TileInfo[,,] arr, TileInfo tileInfo = null )
    {
        tileInfo = new TileInfo();
        for(int i = 0; i < arr.GetLength(0); i++)
        {
            for(int j = 0; j < arr.GetLength(1); j++)
            {
                for(int k = 0;  k < arr.GetLength(2); k++)
                {
                    arr[i, j, k] = tileInfo;
                }
            }
        }
    }


    
}
