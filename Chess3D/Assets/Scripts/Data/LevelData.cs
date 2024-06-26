using UnityEngine;
using System.Collections.Generic;
using GDC.Enums;

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

public abstract class Army
{
    public ChessManType chessManType;
    public Vector3 posIndex;
}

public class PlayerArmy : Army
{
    public PlayerArmy(Vector3 posIndex, ChessManType chessManType)
    {
        this.posIndex = posIndex;
        this.chessManType = chessManType;
    }
}

public class EnemyArmy : Army
{
    public EnemyArmy(Vector3 posIndex, ChessManType chessManType)
    {
        this.posIndex = posIndex;
        this.chessManType = chessManType;
    }
}

[CreateAssetMenu(menuName = "Data/Level Data")]
public class LevelData : ScriptableObject
{
    private TileInfo[,,] tileInfo;
    private List<PlayerArmy> playerArmies;
    private List<EnemyArmy> enemyArmies;

    public LevelData()
    {
        tileInfo = new TileInfo[30, 20, 30];
        playerArmies = new List<PlayerArmy>();
        enemyArmies = new List<EnemyArmy>();
    }

    public void SetData(TileInfo[,,] tileInfo, List<PlayerArmy> playerArmies, List<EnemyArmy> enemyArmies)
    {
        this.tileInfo = DeepCopyArray(tileInfo);
        this.playerArmies.AddRange(playerArmies);
        this.enemyArmies.AddRange(enemyArmies);
    }

    public TileInfo[,,] GetTileInfo()
    {
        return DeepCopyArray(tileInfo);
    }

    public List<PlayerArmy> GetPlayerArmies()
    {
        return new List<PlayerArmy>(playerArmies); // Return a shallow copy to prevent external modification
    }

    public List<EnemyArmy> GetEnemyArmies()
    {
        return new List<EnemyArmy>(enemyArmies); // Return a shallow copy to prevent external modification
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
