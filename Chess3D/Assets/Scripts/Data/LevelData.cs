﻿using UnityEngine;
using System.Collections.Generic;
using GDC.Enums;
using System;
using System.ComponentModel;
using GDC.Constants;
using Unity.VisualScripting;
using RotaryHeart.Lib.SerializableDictionary;
using NaughtyAttributes;

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
    public bool isAI;
    public int priority;
    public List<Vector3> movePosIndexs;
    public EnemyArmy(Vector3 posIndex, ChessManType chessManType)
    {
        isAI = false;
        this.posIndex = posIndex;
        this.chessManType = chessManType;
    }
}

[Serializable]
public class HintMove
{
    public PlayerArmy playerArmy;
    public Vector3 position;
    public bool isPromote;
    public ChessManType promoteType;

    public HintMove(PlayerArmy playerArmy, Vector3 position, ChessManType promoteType, bool isPromote = false)
    {
        this.playerArmy = playerArmy;
        this.position = position;
        this.isPromote = isPromote;
        this.promoteType = promoteType;
    }
}

[CreateAssetMenu(menuName = "Data/Level Data")]
[Serializable]
public class LevelData : ScriptableObject
{
    public int id; //id của level
    public Vector3 center; //vị trí trung tâm cho camera
    public float distance; //khoảng cách từ camera đến map
    public int maxTurn; //Số lượt chơi tối đa của màn
    public int starTurn2; //Số lượt chơi còn lại ít nhất để đạt 2 sao
    public int starTurn3; //Số lượt chơi còn lại ít nhất để đạt 3 sao
    public List<TileData> tileInfo; //Chứa list các tile (dữ liệu chính của map)
    [SerializeField] private List<PlayerArmy> playerArmies;
    [SerializeField] private List<EnemyArmy> enemyArmies;

    public List<HintMove> hintMoves; //Chứa cách giải puzzle của màn

    public Sprite thumbnail;

    public LevelData()
    {
        tileInfo = new List<TileData> ();
        playerArmies = new List<PlayerArmy>();
        enemyArmies = new List<EnemyArmy>();
        hintMoves = new List<HintMove>();
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

    public void SetTileInfoNoDeep(Vector3 posIndex, int idValue, TileType tileTypeValue) //Not for scriptable object
    {
        foreach (TileData data in this.tileInfo)
        {
            if (GameUtils.CompareVector3(posIndex, data.pos))
            {
                data.tileInfo.id = idValue;
                data.tileInfo.tileType = tileTypeValue;
            }
        }
    }
    //public void SetTileInfoNoDeep(int x, int y, int z, TileInfo tileInfoValue) //Not for scriptable object
    //{
    //    foreach (TileData data in this.tileInfo)
    //    {
    //        if (x == (int)data.pos.x && y == (int)data.pos.y && z == (int)data.pos.z)
    //        {
    //            data.tileInfo = tileInfoValue;
    //        }
    //    }
    //}
    public TileInfo GetTileInfoNoDeep(Vector3 posIndex) //Not for scriptable object
    {
        foreach (TileData data in this.tileInfo)
        {
            if (GameUtils.CompareVector3(posIndex,data.pos))
            {
                return data.tileInfo;
            }
        }

        return null;
    }

    public List<PlayerArmy> GetPlayerArmies()
    {
        return new List<PlayerArmy>(playerArmies);
    }
    public List<EnemyArmy> GetEnemyArmies()
    {
        return new List<EnemyArmy>(enemyArmies);
    }
    public List<Vector3> GetAllDefaultMove()
    {
        Dictionary<Vector3, Vector3> moveDict = new Dictionary<Vector3, Vector3>(); //Dung dictionary de giam O(n^3) -> O(n^2)
        List<Vector3> moves = new List<Vector3>();
        foreach(var enemy in enemyArmies)
        {
            foreach(var enemyMove in enemy.movePosIndexs)
            {
                moveDict[enemyMove] = enemyMove;
            }
        }
        foreach(var move in moveDict.Values)
        {
            moves.Add(move);
        }
        return moves;
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

#if UNITY_EDITOR
    [Header("Danh cho muon sua lai 1 tile nao do ma khong can phai gen map lai")]
    //Editor only
    [SerializeField] private Vector3 posEdit;
    [SerializeField] TileInfo tileInfoEdit;
    [Button]
    private void EditTileInfo()
    {
        Debug.Log("Edit tile success");
        SetTileInfoNoDeep(posEdit, tileInfoEdit.id, tileInfoEdit.tileType);
    }
#endif
}
