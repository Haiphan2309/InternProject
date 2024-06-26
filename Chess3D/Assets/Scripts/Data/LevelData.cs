using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName ="Data/Level Data")]
public class LevelData : ScriptableObject
{
    public int xSize;
    public int ySize;
    public int zSize;
    public int[] map; // Flattened 3D array

    public void SetMap(int[,,] originalMap)
    {
        xSize = originalMap.GetLength(0);
        ySize = originalMap.GetLength(1);
        zSize = originalMap.GetLength(2);
        map = new int[xSize * ySize * zSize];

        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                for (int z = 0; z < zSize; z++)
                {
                    map[GetIndex(x, y, z)] = originalMap[x, y, z];
                }
            }
        }
    }

    public int[,,] GetMap()
    {
        int[,,] originalMap = new int[xSize, ySize, zSize];

        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                for (int z = 0; z < zSize; z++)
                {
                    originalMap[x, y, z] = map[GetIndex(x, y, z)];
                }
            }
        }

        return originalMap;
    }

    private int GetIndex(int x, int y, int z)
    {
        return x + (y * xSize) + (z * xSize * ySize);
    }
}
