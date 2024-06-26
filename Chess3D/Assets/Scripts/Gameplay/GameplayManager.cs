using GDC.Enums;
using GDC.Managers;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager Instance { get; private set; }

    [SerializeField] LevelSpawner levelSpawner;
    [SerializeField, ReadOnly] bool enemyTurn;
    public LevelData levelData;
    [SerializeField] Transform availableMovePrefab;
    List<Transform> availableMoveTrans = new List<Transform>();
    private void Awake()
    {
        Instance = this;
    }

    [Button]
    public void LoadLevel()
    {
        levelSpawner.Setup();
        levelSpawner.SpawnLevel();
        levelData = levelSpawner.levelData;
    }

    public void ChangeTurn(bool enemyTurn)
    {
        this.enemyTurn = enemyTurn;
        if (enemyTurn)
        {
            EnemyTurn();
        }
        else
        {
            PlayerTurn();
        }
        
    }
    void EnemyTurn()
    {
        //dosomething
        Debug.Log("Enemy Turn!");
        ChangeTurn(false);
    }
    void PlayerTurn()
    {
        Debug.Log("Player Turn");
        //dosomething
    }
    public void ShowAvailableMove(ChessManConfig chessManConfig, Vector3 curPosIndex)
    {
        if (chessManConfig == null)
        {
            Debug.LogError("chessman chua co config");
            return;
        }
        List<Vector3> moves = chessManConfig.Move(curPosIndex);
        if (moves != null)
        {
            foreach (Vector3 move in moves)
            {
                TileInfo tileInfo = levelData.GetTileInfo()[(int)move.x, (int)move.y, (int)move.z];
                Transform tran = Instantiate(availableMovePrefab, move, Quaternion.identity);
                switch (tileInfo.tileType)
                {
                    case TileType.GROUND:
                        tran.position += new Vector3(0.5f, 0, 0.5f);
                        break; ;
                    case TileType.SLOPE_0:
                        tran.position += new Vector3(0.5f, 0, 0.5f);
                        break;
                    case TileType.SLOPE_90:
                        tran.position += new Vector3(0.5f, 0, 0.5f);
                        break;
                    case TileType.SLOPE_180:
                        tran.position += new Vector3(0.5f, 0, 0.5f);
                        break;
                    case TileType.SLOPE_270:
                        tran.position += new Vector3(0.5f, 0, 0.5f);
                        break;
                }
                availableMoveTrans.Add(tran);
            }

            if (moves.Count == 0)
            {
                Debug.Log("Ko co nuoc di nao co the thuc hien");
            }
        }

        if (moves == null)
        {
            Debug.LogError("move is null");
        }
    }
    public bool CheckMove(ChessManConfig chessManConfig, Vector3 curPosIndex, Vector3 posIndexToMove)
    {
        List<Vector3> moves = chessManConfig.Move(curPosIndex);
        foreach (Vector3 move in moves)
        {
            if (move == posIndexToMove) return true;
        }
        return false;
    }    
    public void MakeMove(ChessMan chessMan, Vector3 posIndexToMove)
    {
        chessMan.Move(posIndexToMove);
    }
}
