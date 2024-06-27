using UnityEngine;
using System.Collections.Generic;
using GDC.Enums;
using System;
using System.ComponentModel;
using GDC.Constants;

[Serializable]
public class TileInfo
{
    public int id;
    public TileType tileType;

    public TileInfo(int id = 0, TileType tileType = TileType.NONE)
    {
        this.id = id;
        this.tileType = tileType;
    }
}

[Serializable]
public class TileData
{
    public Vector3 pos;
    public TileInfo tileInfo;
    public TileData(Vector3 pos, TileInfo info)
    {
        this.pos = pos;
        this.tileInfo = info;
    }
}
[Serializable]
public abstract class Army
{
    public ChessManType chessManType;
    public Vector3 posIndex;
}

[Serializable]
public class PlayerArmy : Army
{
    public PlayerArmy(Vector3 posIndex, ChessManType chessManType)
    {
        this.posIndex = posIndex;
        this.chessManType = chessManType;
    }
}

[Serializable]
public class EnemyArmy : Army
{
    public int priority;
    public List<Vector3> movePosIndexs;
    public EnemyArmy(Vector3 posIndex, ChessManType chessManType)
    {

        this.posIndex = posIndex;
        this.chessManType = chessManType;
    }
}

[CreateAssetMenu(menuName = "Data/Level Data")]
[Serializable]
public class LevelData : ScriptableObject
{

    [SerializeField] private Vector3 center;
    [SerializeField] private int maxTurn;
    [SerializeField] private List<TileData> tileInfo;
    [SerializeField] private List<PlayerArmy> playerArmies;
    [SerializeField] private List<EnemyArmy> enemyArmies;
    

    public LevelData()
    {
        tileInfo = new List<TileData> ();
        playerArmies = new List<PlayerArmy>();
        enemyArmies = new List<EnemyArmy>();
    }

    public void SetData(TileInfo[,,] tileInfo, List<PlayerArmy> playerArmies, List<EnemyArmy> enemyArmies)
    {
        //this.tileInfo = DeepCopyArray(tileInfo);
        SetTileInfo(tileInfo);
        this.playerArmies.AddRange(playerArmies);
        this.enemyArmies.AddRange(enemyArmies);
    }
    
    public void SetTileInfo(TileInfo[,,] tileInfo)
    {
        int dim0 = tileInfo.GetLength(0);
        int dim1 = tileInfo.GetLength(1);
        int dim2 = tileInfo.GetLength(2);

        TileInfo[,,] newArray = new TileInfo[dim0, dim1, dim2];

        for (int i = 0; i < dim0; i++)
        {
            for (int j = 0; j < dim1; j++)
            {
                for (int k = 0; k < dim2; k++)
                {
                    Vector3 pos = new Vector3(i, j, k);
                    TileInfo newInfo = new TileInfo(tileInfo[i, j, k].id, tileInfo[i, j, k].tileType);
                    this.tileInfo.Add(new TileData(pos, newInfo));
                }
            }
        }

       
    }

    public TileInfo[,,] GetTileInfo()
    {
        
        TileInfo[,,] map = new TileInfo[GameConstants.MAX_X_SIZE, GameConstants.MAX_Y_SIZE, GameConstants.MAX_Z_SIZE];
        foreach(TileData data in this.tileInfo)
        {
            map[(int)data.pos.x, (int)data.pos.y, (int)data.pos.z] = data.tileInfo;
        }
        return DeepCopyArray(map);

    }

    public List<PlayerArmy> GetPlayerArmies()
    {
        return new List<PlayerArmy>(playerArmies);
    }
    public List<EnemyArmy> GetEnemyArmies()
    {
        return new List<EnemyArmy>(enemyArmies);
    }

    private TileInfo[,,] DeepCopyArray(TileInfo[,,] originalArray)
    {
        int dim0 = originalArray.GetLength(0);
        int dim1 = originalArray.GetLength(1);
        int dim2 = originalArray.GetLength(2);

        TileInfo[,,] newArray = new TileInfo[dim0, dim1, dim2];

        for (int i = 0; i < dim0; i++)
        {
            for (int j = 0; j < dim1; j++)
            {
                for (int k = 0; k < dim2; k++)
                {
                    newArray[i, j, k] = new TileInfo(originalArray[i, j, k].id, originalArray[i, j, k].tileType);
                }
            }
        }

        return newArray;
    }
}
