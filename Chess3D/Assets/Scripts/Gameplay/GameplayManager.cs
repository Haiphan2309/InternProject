using GDC.Enums;
using GDC.Managers;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager Instance { get; private set; }

    [SerializeField, ReadOnly] bool enemyTurn;
    public LevelData levelData;
    private void Awake()
    {
        Instance = this;
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
        ChangeTurn(false);
    }
    void PlayerTurn()
    {
        //dosomething
    }
    public void ShowAvailableMove(ChessManConfig chessManConfig, Vector3 curPosIndex)
    {
        List<Vector3> moves = chessManConfig.Move(curPosIndex);
        //todo
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
