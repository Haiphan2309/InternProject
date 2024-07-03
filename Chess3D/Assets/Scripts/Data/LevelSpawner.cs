
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GDC.Enums;
using System;
using GDC.Constants;
public class LevelSpawner : MonoBehaviour
{
    // 
    [HideInInspector] public LevelData levelData;
    [HideInInspector] public List<ChessMan> playerArmy, enemyArmy;
    [HideInInspector] public string spawnLevelName = "";
    
    //
    string levelDataPath = "Assets/Resources/ScriptableObjects/LevelData";
    string objectPrefabPath = "ObjectPrefabs";
    string chessPrefabPath = "ChessManPrefabs";

    Dictionary<int, GameObject> tilePrefabDic = new Dictionary<int, GameObject>();
    Dictionary<int, GameObject> chessPrefabDic = new Dictionary<int, GameObject>();
    Dictionary<ChessManType, int> chessDic;
    public void Setup()
    {
        chessDic = new Dictionary<ChessManType, int> 
        {
            { ChessManType.PAWN ,   0 },
            { ChessManType.CASTLE,  1 },
            { ChessManType.KNIGHT,  2 },
            { ChessManType.BISHOP,  3 },
            { ChessManType.QUEEN,   4 },
            { ChessManType.KING,    5 }

        };
    }

    [Button]
    public void SpawnLevel(string levelName)
    {
        Setup();
        spawnLevelName = levelName;
        GetLevelData();
        GetPrefabs();
        SpawnTile();
        SpawnPlayerChess();
        SpawnEnemyChess();
    }
    private void SpawnTile()
    {
        GameObject levelObject = new GameObject("Level");
        //

        TileInfo[,,] map = levelData.GetTileInfo();
        
        List<EnemyArmy> enemyArmies = levelData.GetEnemyArmies();
        int dim0 = map.GetLength(0);
        int dim1 = map.GetLength(1);
        int dim2 = map.GetLength(2);

        for (int j = 0; j < dim1; j++)
        {
            GameObject floor = new GameObject("Floor_" + j);
            floor.transform.parent = levelObject.transform;
            //Instantiate(floor);
            for (int i = 0; i < dim0; i++) 
            {
                for (int k = 0; k < dim2; k++)
                {
                    TileInfo tileInfo = map[i, j, k];
                    int tileId = tileInfo.id;
                    Vector3 spawnPos = Vector3.up * j  + Vector3.right * i + Vector3.forward * k;
                    //
                    if (tileInfo == null) continue;
                    if (tileInfo.tileType == TileType.NONE) continue;
                    //
                    if (tileId < 0 || tileId > GameConstants.obstacleIdBoundary.y) continue;
                    GameObject tile = Instantiate(tilePrefabDic[tileId], spawnPos, tilePrefabDic[tileId].transform.rotation);

                    tile.transform.parent = floor.transform;
                }
            }
            // If Floor does not have block
            if (floor.transform.childCount <= 0)
            {
                Destroy(floor);
            }
        }
    }

    private void SpawnPlayerChess()
    {
        List<PlayerArmy> playerArmies = levelData.GetPlayerArmies();
        GameObject playerChessObject = new GameObject("PlayerChessObject");
        int index = 0;
        foreach (var army in playerArmies)
        {
            ChessManType armyType = army.chessManType;
            int armyId = chessDic[armyType] + (int)GameConstants.playerChessIdBoundary.x;
            Vector3 spawnPos = army.posIndex;
            if (army == null) continue;
            GameObject armyObject = Instantiate(chessPrefabDic[armyId], spawnPos, chessPrefabDic[armyId].transform.rotation);
            armyObject.transform.parent = playerChessObject.transform;
            // Setup Player Army
            armyObject.GetComponent<ChessMan>().Setup(army, index, army.posIndex);
            playerArmy.Add(armyObject.GetComponent<ChessMan>());
            //
            index++;
        }
    }
    private void SpawnEnemyChess()
    {
        List<EnemyArmy> enemyArmies = levelData.GetEnemyArmies();
        GameObject enemyChessObject = new GameObject("EnemyChessObject");
        int index = 0;
        foreach (var army in enemyArmies)
        {
            ChessManType armyType = army.chessManType;
            int armyId = chessDic[armyType] + (int)GameConstants.enemyChessIdBoundary.x;
            if (army.isAI) armyId += 6;
            Vector3 spawnPos = army.posIndex;
            if (army == null) continue;
            GameObject armyObject = Instantiate(chessPrefabDic[armyId], spawnPos, chessPrefabDic[armyId].transform.rotation);
            armyObject.transform.parent = enemyChessObject.transform;
            // Setup Player Army
            armyObject.GetComponent<ChessMan>().Setup(army, index, army.posIndex);
            enemyArmy.Add(armyObject.GetComponent<ChessMan>());
            //
            index++;
        }
    }
    private void GetLevelData()
    {
        string loadPath = "ScriptableObjects/LevelData/" + spawnLevelName;
        levelData = Resources.Load<LevelData>(loadPath);
        
        if (levelData == null)
        {
            Debug.LogError($"Failed to load level: " + spawnLevelName);

        }
        else
        {
            Debug.Log($"Loading level {spawnLevelName} successfully");
        }

    }
    private void GetPrefabs()
    {
        GameObject[] objectPrefabsList =  Resources.LoadAll<GameObject>(objectPrefabPath);
        GameObject[] chessPrefabsList = Resources.LoadAll<GameObject>(chessPrefabPath);
        for (int i = 0; i < objectPrefabsList.Length; i++)
        {
            int prefabId = 0;
            if (!int.TryParse(objectPrefabsList[i].name, out prefabId))
            {
                Debug.LogError("Prefab name is not correct, must be an integer: " + objectPrefabsList[i].name);
            }
            GameObject prefab = objectPrefabsList[i] as GameObject;
            tilePrefabDic.Add(prefabId, prefab);
        }
        for (int i = 0; i < chessPrefabsList.Length; i++)
        {
            int prefabId = 0;
            if (!int.TryParse(chessPrefabsList[i].name, out prefabId))
            {
                Debug.LogError("Prefab name is not correct, must be an integer: " + chessPrefabsList[i].name);
            }
            GameObject prefab = chessPrefabsList[i] as GameObject;
            //tilePrefabDic.Add(prefabId, prefab);
            chessPrefabDic.Add(prefabId, prefab);
        }



    }
}
