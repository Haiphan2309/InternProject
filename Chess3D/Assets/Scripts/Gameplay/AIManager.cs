using GDC.Constants;
using GDC.Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    public static AIManager Instance;

    private Dictionary<ChessManType, int> chessManPriority = new Dictionary<ChessManType, int>();
    private List<ChessMan> enemyList;
    private int[,,] mapScore;
    private void Awake()
    {
        Instance = this;
        chessManPriority[ChessManType.PAWN] = 0;
        chessManPriority[ChessManType.BISHOP] = 1;
        chessManPriority[ChessManType.KNIGHT] = 2;
        chessManPriority[ChessManType.CASTLE] = 3;
        chessManPriority[ChessManType.QUEEN] = 4;
        chessManPriority[ChessManType.KING] = 5;
        mapScore = new int[GameConstants.MAX_X_SIZE, GameConstants.MAX_Y_SIZE, GameConstants.MAX_Z_SIZE];
    }

    // The decision:
    // Score of the state + Score of enemy chesspiece + Score of player chesspiece
    // Return the highest score move for GameplayManager to execute move
    public KeyValuePair<ChessMan, Vector3> Decision()
    {
        GetEnemyInfo();
        SetMapScore();

        int bestScore = 0;
        int currentScore;

        ChessMan bestChessman = null;
        ChessMan currentChessman;

        Vector3 bestMove = Vector3.zero;
        Vector3 currentMove;
        
        for (int i = 0; i < enemyList.Count; ++i)
        {
            currentChessman = enemyList[i];
            currentScore = 10 - chessManPriority[currentChessman.config.chessManType];
            currentMove = Vector3.zero;

            int killChessmanScore;

            List<Vector3> possibleMoveList = currentChessman.config.Move(currentChessman.posIndex);
            Vector3 killState = KillState(possibleMoveList, out killChessmanScore);
            Vector3 retreatState = RetreatState(possibleMoveList);
            Vector3 patrolState = PatrolState(possibleMoveList);
            
            if(patrolState != Vector3.zero)
            {
                currentScore = currentScore + 1;
                currentMove = patrolState;
            }
            if(retreatState != Vector3.zero)
            {
                currentScore = currentScore + 20;
                currentMove = retreatState;
            }
            if(killState != Vector3.zero)
            {
                currentScore = currentScore + 20 + killChessmanScore;
                currentMove = killState;
            }

            if (bestChessman == null || bestScore < currentScore || (bestScore == currentScore && Random.Range(0, 100) >= 50))
            {
                bestChessman = currentChessman;
                bestScore = currentScore;
                bestMove = currentMove;
            }
        }
        return new KeyValuePair<ChessMan, Vector3>(bestChessman, bestMove);
    }

    private void GetEnemyInfo()
    {
        if(enemyList != null) enemyList.Clear();
        enemyList = GameplayManager.Instance.enemyArmy;
    }

    private void SetTileScore(Vector3 position, int score)
    {
        mapScore[Mathf.RoundToInt(position.x),
                 Mathf.RoundToInt(position.y),
                 Mathf.RoundToInt(position.z)] = score;
    }

    private void SetMapScore()
    {
        List<ChessMan> players = GameplayManager.Instance.playerArmy;
        for(int i = 0; i < players.Count; ++i)
        {
            List<Vector3> moveSet = players[i].config.Move(players[i].posIndex);
            for(int j = 0; j < moveSet.Count; ++j)
            {
                SetTileScore(moveSet[j], 1);
            }
        }
    }

    // KillState: Use the piece to kill the player's piece
    public Vector3 KillState(List<Vector3> possibleMoveList, out int bestChessManID)
    {
        bestChessManID = 0;
        List<ChessMan> playerArmy = GameplayManager.Instance.playerArmy;
        // We find the best ChessManType in the playerArmy list
        for (int playerIdx = 0; playerIdx < playerArmy.Count; ++playerIdx)
        { 
            if (possibleMoveList.Contains(playerArmy[playerIdx].posIndex))
            {
                if (bestChessManID < chessManPriority[playerArmy[playerIdx].config.chessManType])
                {
                    bestChessManID = chessManPriority[playerArmy[playerIdx].config.chessManType];
                }
            }
        }

        Vector3 decision = Vector3.zero;
        for (int playerIdx = 0; playerIdx < playerArmy.Count; ++playerIdx)
        {
            if (chessManPriority[playerArmy[playerIdx].config.chessManType] == bestChessManID)
            {
                // Default
                if (decision == Vector3.zero)
                {
                    decision = playerArmy[playerIdx].posIndex;
                }
                // Random pick
                if (Random.Range(0, 100) >= 50)
                {
                    decision = playerArmy[playerIdx].posIndex;
                }
            }
        }
        return decision;
    }


    // RetreatState: Move the piece away from the danger tiles
    public Vector3 RetreatState(List<Vector3> possibleMoveList)
    {
        SetMapScore();

        Vector3 decision = Vector3.zero;
        foreach (Vector3 move in possibleMoveList)
        {
            int score = mapScore[(int)move.x, (int)move.y, (int)move.z];
            if (score >= 0)
            {
                if (decision == Vector3.zero)
                {
                    decision = move;
                }
                // Random pick
                if (Random.Range(0, 100) >= 50)
                {
                    decision = move;
                }
            }
        }
        return decision;
    }

    // PatrolState: Default
    public Vector3 PatrolState(List<Vector3> possibleMoveList)
    {
        return possibleMoveList[Random.Range(0, possibleMoveList.Count)];
    }
}
