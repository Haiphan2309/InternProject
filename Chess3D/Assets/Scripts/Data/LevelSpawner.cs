
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GDC.Enums;
public class LevelSpawner : MonoBehaviour
{
    // 
    [HideInInspector] public LevelData levelData;
    public string spawnLevelName = "";
    
    //
    string levelDataPath;
    string prefabPath;

    Dictionary<int, GameObject> prefabDic = new Dictionary<int, GameObject>();
    public void Setup()
    {
        levelDataPath = "Assets/Resources/ScriptableObjects/LevelData";
        prefabPath = "ObjectPrefabs";
    }

    [Button]
    public void SpawnLevel()
    {
        //TileInfo[,,] map = new TileInfo[30, 20, 30];
        //List<PlayerArmy> playerArmies = new List<PlayerArmy>();
        //List<EnemyArmy> enemyArmies = new List<EnemyArmy>();
        // Get Level Data
        GetLevelData();

        // Get all prefabs
        GetPrefabs();

        // Spawn map
        SpawnMap();
        
        
    }
    private void SpawnMap()
    {
        //
        //
        GameObject levelObject = new GameObject();
        levelObject.name = "Level";
        Instantiate(levelObject);
        //

        TileInfo[,,] map = levelData.GetTileInfo();
        int dim0 = map.GetLength(0);
        int dim1 = map.GetLength(1);
        int dim2 = map.GetLength(2);

        for (int j = 0; j < dim1; j++)
        {
            GameObject floor = new GameObject();
            floor.name = "Floor_" + j;
            floor.transform.parent = levelObject.transform;
            Instantiate(floor);
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
                    GameObject tile = Instantiate(prefabDic[tileId], spawnPos, Quaternion.identity);
                    tile.transform.parent = floor.transform;
                }
            }
        }
    }

    private void GetLevelData()
    {
        string loadPath = "ScriptableObjects/LevelData/" + spawnLevelName;
        levelData = Resources.Load<LevelData>(loadPath);
        
        if (levelData == null)
        {
            Debug.LogError($"Failed to load level: ");

        }
        else
        {
            Debug.Log($"Loading level {spawnLevelName} successfully");
        }
        Debug.Log(levelData.GetPlayerArmies()[0].chessManType);
    }
    private void GetPrefabs()
    {
        GameObject[] prefabsList =  Resources.LoadAll<GameObject>(prefabPath);
        for(int i = 0; i < prefabsList.Length; i++)
        {
            int prefabId = 0;
            if (!int.TryParse(prefabsList[i].name, out prefabId))
            {
                Debug.LogError("Prefab name is not correct, must be an integer: " + prefabsList[i].name);
            }
            GameObject prefab = prefabsList[i] as GameObject;
            prefabDic.Add(prefabId, prefab);
        }
        
    }
}
