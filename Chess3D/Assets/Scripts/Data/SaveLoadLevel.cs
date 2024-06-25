using NaughtyAttributes;
using RotaryHeart.Lib;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class SaveLoadLevel : MonoBehaviour
{
    // Just for testing, will be placed with Dynamic Load in future
    [SerializeField] Grid grid;

    public string levelDataPath;

    public string levelName = "";
    //
 
    void Start()
    {
        
    }

    [Button]
    public void ExtractLevel()
    {
        int[,,] map = new int[30, 20, 30];
        ExtractLevelToMap(map);
        SaveMapToLevelData(map);


    }

    private void SaveMapToLevelData(int[,,] map)
    {

        // Create new Level Data
        LevelData newLevelData = ScriptableObject.CreateInstance<LevelData>();
        newLevelData.SetMap(map);
        string path = $"{levelDataPath}/{levelName}.asset";
        
        AssetDatabase.CreateAsset(newLevelData, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        //
        Debug.Log("Level Data created successfully");

    }



    private void ExtractLevelToMap(int[,,] map)
    {
        //Loop all tilemap - Floor
        for (int i = 0; i < grid.transform.childCount; i++)
        {
            Transform floor = grid.transform.GetChild(i);
            for (int j = 0; j < floor.childCount; j++)
            {
                Transform block = floor.GetChild(j);
                // Check position > 0
                Vector3 blockPos = block.position;
                if (blockPos.x < 0 || blockPos.y < 0 || blockPos.z < -0)
                {
                    Debug.LogError("Block position(x,y,z) must be positive");
                }
                // Get block name as int
                int blockNumber;
                if (!int.TryParse(block.gameObject.name, out blockNumber))
                {
                    Debug.LogError("Block name is not correct, must be an integer: " + block.gameObject.name);
                }
                // Store to array
                map[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z] = blockNumber;
            }
        }
    }
}
