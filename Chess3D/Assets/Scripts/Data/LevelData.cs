using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName ="Data/Level Data")]

public class TileInfo
{
    public int id;
    public GDC.Enums.TileType tileType;
}
public class LevelData : ScriptableObject
{
    public int xSize;
    public int ySize;
    public int zSize;
    public TileInfo[] map; // Flattened 3D array

    public void SetMap(int[,,] originalMap)
    {
    }

    public TileInfo[,,] GetMap()
    {
        TileInfo[,,] originalMap;
        return originalMap;
    }

    private int GetIndex(int x, int y, int z)
    {
        return x + (y * xSize) + (z * xSize * ySize);
    }
}
