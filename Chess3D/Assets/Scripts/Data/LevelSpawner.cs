
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
    string levelDataPath = "Assets/Resources/ScriptableObjects/LevelData";
    string objectPrefabPath = "ObjectPrefabs";
    string chessPrefabPath = "ChessManPrefabs";

    Dictionary<int, GameObject> prefabDic = new Dictionary<int, GameObject>();
    public void Setup()
    {
        //levelDataPath = 
        //prefabPath = 
    }

    [Button]
    public void SpawnLevel()
    {
        GetLevelData();
        GetPrefabs();
        SpawnMap();
        
        
    }
    private void SpawnMap()
    {
        GameObject levelObject = new GameObject("Level");
        //

        TileInfo[,,] map = levelData.GetTileInfo();
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
                    GameObject tile = Instantiate(prefabDic[tileId], spawnPos, prefabDic[tileId].transform.rotation);
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
            prefabDic.Add(prefabId, prefab);
        }
        for (int i = 0; i < chessPrefabsList.Length; i++)
        {
            int prefabId = 0;
            if (!int.TryParse(chessPrefabsList[i].name, out prefabId))
            {
                Debug.LogError("Prefab name is not correct, must be an integer: " + chessPrefabsList[i].name);
            }
            GameObject prefab = chessPrefabsList[i] as GameObject;
            prefabDic.Add(prefabId, prefab);
        }

    }
}
