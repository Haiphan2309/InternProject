
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GDC.Enums;
using System;
using GDC.Constants;
using Unity.Burst.Intrinsics;
public class LevelSpawner : MonoBehaviour
{
    [HideInInspector] public LevelData levelData;
    [HideInInspector] public List<ChessMan> playerArmy, enemyArmy;
    [HideInInspector] public List<ToggleBlock> toggleBlockList;
    [HideInInspector] public List<ButtonObject> buttonList;
    //[HideInInspector] public string spawnLevelName = "";
    
    // chapter x, level y -> Level_x_y 
    //load chapter x -> level[y] -> id -> Level_x_id 
    //
    private string chapterDataPath = "Assets/Resources/ScriptableObjects/ChapterData";
    private string levelDataPath;
    private string objectPrefabPath = "ObjectPrefabs";
    private string chessPrefabPath = "ChessManPrefabs";

    private Dictionary<int, GameObject> tilePrefabDic = new Dictionary<int, GameObject>();
    private Dictionary<int, GameObject> chessPrefabDic = new Dictionary<int, GameObject>();
    private Dictionary<ChessManType, int> chessDic;
    private int boxIndexCount, boulderIndexCount, buttonIndexCount;
    void Setup(int chapterId, int levelId)
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
        levelData = GetLevelData(chapterId, levelId);
        toggleBlockList = new List<ToggleBlock>();
        buttonList = new List<ButtonObject>();
    }

    [Button]
    public void SpawnLevel(int chapterId, int levelId)
    {
        Setup(chapterId, levelId);
        //spawnLevelName = levelName;
        GetPrefabs();
        SpawnTile();
        SpawnPlayerChess();
        SpawnEnemyChess();
        SpawnEnemyDefaultMove();
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

        boxIndexCount = 0;
        boulderIndexCount = 0;
        buttonIndexCount = 0;

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
                    
                    if (tileInfo == null) continue;
                    if (tileId == 0) continue;
                    if (tileId < 0 || tileId > GameConstants.obstacleIdBoundary.y) continue;

                    GameObject tile = Instantiate(tilePrefabDic[tileId], spawnPos, tilePrefabDic[tileId].transform.rotation);
                    tile.transform.parent = floor.transform;
                    if (tileId == 200) // Box
                    {
                        tile.GetComponent<Box>().Setup(GameUtils.SnapToGrid(tile.transform.position),boxIndexCount);
                        boxIndexCount++;
                    }
                    else if (tileId == 201) // Boulder
                    {
                        tile.GetComponent<Boulder>().Setup(GameUtils.SnapToGrid(tile.transform.position),boulderIndexCount);
                        boulderIndexCount++;
                    }
                    else if(tileId == 203 || tileId == 204) // Button
                    {
                        buttonList.Add(tile.GetComponent<ButtonObject>());
                    }
                    else if (tileId >= 205 && tileId <= 208) // ToggleBlock
                    {
                        // 205, 207: Off
                        // 206, 208: On
                        bool isOn = (tileId == 206 || tileId == 208);
                        ToggleBlock block = tile.GetComponent<ToggleBlock>();
                        block.Setup(isOn);
                        toggleBlockList.Add(block);
                    }
                    
                }
            }

            //Set up button
            foreach(ButtonObject button in buttonList)
            {
                List<ToggleBlock> list;
                if (button.gameObject.name.Contains("203"))
                {
                    list = toggleBlockList.FindAll(
                        block => (block.gameObject.name.Contains("205") || block.gameObject.name.Contains("206"))
                    );
                }
                else
                {
                    list = toggleBlockList.FindAll(
                        block => (block.gameObject.name.Contains("207") || block.gameObject.name.Contains("208"))
                    );
                }
                button.Setup(list);
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
            Vector3 spawnPos = army.posIndex;

            int armyId = chessDic[armyType] + (int)GameConstants.playerChessIdBoundary.x;
            
            if (army == null) continue;
            GameObject armyObject = Instantiate(chessPrefabDic[armyId], spawnPos, chessPrefabDic[armyId].transform.rotation);
            armyObject.transform.parent = playerChessObject.transform;

            // Setup Player Army
            armyObject.GetComponent<ChessMan>().Setup(army, index, army.posIndex);
            playerArmy.Add(armyObject.GetComponent<ChessMan>());

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
            Vector3 spawnPos = army.posIndex;

            int armyId = chessDic[armyType] + (int)GameConstants.enemyChessIdBoundary.x;

            if (army.isAI) armyId += 6;
            
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
    void SpawnEnemyDefaultMove() //Spawn nhung cham trang bieu thi nuoc di mac dinh cua enemy
    {
        var defaultMovePointObj = Resources.Load<GameObject>( objectPrefabPath + "/999");
        List<Vector3> defaultMoves = levelData.GetAllDefaultMove();
        foreach (var move in defaultMoves) 
        {
            Vector3 posSpawn = move;
            switch (levelData.GetTileInfoNoDeep(move + Vector3.down).tileType)
            {
                case TileType.SLOPE_0:
                    posSpawn += new Vector3(0f, -0.45f, 0.1f);
                    break;
                case TileType.SLOPE_90:
                    posSpawn += new Vector3(-0.1f, -0.45f, 0f);
                    break;
                case TileType.SLOPE_180:
                    posSpawn += new Vector3(0f, -0.45f, -0.1f);
                    break;
                case TileType.SLOPE_270:
                    posSpawn += new Vector3(1f, -0.45f, 0f);
                    break;
            }
            Instantiate(defaultMovePointObj, posSpawn, Quaternion.identity);
        }
    }
    public ChapterData GetChapterData(int chapterId)
    {
        return GameUtils.GetChapterData(chapterId);
    }
    private LevelData GetLevelData(int chapterID, int levelID)
    {
        return GameUtils.GetLevelData(chapterID, levelID);
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
