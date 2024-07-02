using GDC.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using Unity.Mathematics;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ChessManConfig : ScriptableObject
{
    private float _Xlimit = 0;
    private float _Ylimit = 0;
    private float _Zlimit = 0;

    private float _height;
    private int _moveRange;
    private ChessManType _chessManType;
    private List<Vector3> _possibleMoveList;

    public float Xlimit { get; set; }
    public float Ylimit { get; set; }
    public float Zlimit { get; set; }

    public float height { get; set; }
    public int moveRange { get; set; }
    public ChessManType chessManType { get; set; }
    public List<Vector3> possibleMoveList { get; set; }

    // Get TileType of a tile below position
    public void LoadLimit()
    {
        _Xlimit = GameplayManager.Instance.levelData.GetTileInfo().GetLength(0);
        _Ylimit = GameplayManager.Instance.levelData.GetTileInfo().GetLength(1);
        _Zlimit = GameplayManager.Instance.levelData.GetTileInfo().GetLength(2);
    }

    private TileType GetTileBelow(Vector3 position)
    {
        float Xpos = position.x;
        float Ypos = position.y - 1f;
        float Zpos = position.z;
        return GameplayManager.Instance.levelData.GetTileInfo()[
               (int)Mathf.Round(Xpos),
               (int)Mathf.Round(Ypos),
               (int)Mathf.Round(Zpos)
               ].tileType;
    }

    private TileType GetTile(Vector3 position)
    {
        float Xpos = position.x;
    // Get TileType of a tile at position
        float Ypos = position.y;
        float Zpos = position.z;
        return GameplayManager.Instance.levelData.GetTileInfo()[
               (int)Mathf.Round(Xpos),
               (int)Mathf.Round(Ypos),
               (int)Mathf.Round(Zpos)
               ].tileType;
    }

    // Check if the potential tile is available
    private bool IsTile(Vector3 currentMove)
    {
        // if the tile below the currentMove's y-level is NONE
        // then there is no tile below the currentMove.
        bool isTile = false;
        TileType tileData = GetTileBelow(currentMove);

        // Object cannot stand on NONE
        switch (tileData)
        {
            // if it's not NONE / PLAYER_CHESS / ENEMY_CHESS then it's a tile
            case TileType.NONE:
            case TileType.PLAYER_CHESS:
            case TileType.ENEMY_CHESS:
                break;
            // else
            default:
                isTile = true;
                break;
        }
        return isTile;
    }

    // Check if the potential tile is in bound
    private bool InBound(Vector3 currentMove)
    {
        bool inBound = true;
        float Xpos = currentMove.x;
        float Ypos = currentMove.y;
        float Zpos = currentMove.z;

        if (Xpos < 0 || Xpos >= _Xlimit)
        {
            inBound = false;
        }
        if (Ypos <= 0 || Ypos >= _Ylimit)
        {
            inBound = false;
        }
        if (Zpos < 0 || Zpos >= _Zlimit)
        {
            inBound = false;
        }
        // Debug.Log("InBound = " + inBound);
        return inBound;
    }

    // Check if the potential tile that the pieces move into can be stood on
    private bool CanStandOn(Vector3 currentMove)
    {
        bool canStandOn = true;
        TileType tileData = GetTileBelow(currentMove);

        // Object can only stand on GROUND / BOX / SLOPES
        switch (tileData)
        {
            // if it's not GROUND / BOX / SLOPES then it's NONE / OBJECT / BOULDER / WATER
            case TileType.NONE:
            case TileType.OBJECT:
            case TileType.BOULDER:
            case TileType.WATER:
                canStandOn = false;
                break;
            // else
            default:
                break;
        }
        return canStandOn;
    }

    // Check if the jump path of the tile that the pieces move into is executable
    public virtual bool ValidateJump(Vector3 currentMove, Vector3 direction)
    {
        bool isJumpable = true;
        return isJumpable;
    }


    // Check if the potential tile that the pieces move into is movable
    private bool ValidateMove(Vector3 currentMove, Vector3 direction)
    {
        bool isMovable = false;
        TileType tileData = GetTile(currentMove);

        switch (tileData)
        {
            // if NONE / PLAYER_CHESS / ENEMY_CHESS then it's movable
            case TileType.NONE:
            case TileType.PLAYER_CHESS:
            case TileType.ENEMY_CHESS:

            // if DYNAMIC OBJECT then we will check it later, we just assume it's movable
            case TileType.BOX:
            case TileType.BOULDER:
                isMovable = true;
                break;

            // if SLOPES then we check based on direction
            case TileType.SLOPE_0:
            case TileType.SLOPE_180:
                if (direction.z != 0)
                {
                    isMovable = true;
                }
                break;
            case TileType.SLOPE_90:
            case TileType.SLOPE_270:
                if (direction.x != 0)
                {
                    isMovable = true;
                }
                break;

            // it is STATIC OBJECT by then it's not movable
            default:
                break;
        }
        return isMovable;
    }

    // Check if the potential tile that the pieces move into is a SLOPE UP
    private bool OnSlopeUp(Vector3 currentMove, Vector3 direction)
    {
        bool onSlopeUp = false;
        TileType tileData = GetTile(currentMove);

        // Object can only stand on GROUND / BOX / SLOPES
        switch (tileData)
        {
            // it's SLOPES
            case TileType.SLOPE_0:
                if (direction.z < 0)
                {
                    onSlopeUp = true;
                }
                break;
            case TileType.SLOPE_90:
                if (direction.x > 0)
                {
                    onSlopeUp = true;
                }
                break;
            case TileType.SLOPE_180:
                if (direction.z > 0)
                {
                    onSlopeUp = true;
                }
                break;
            case TileType.SLOPE_270:
                if (direction.x < 0)
                {
                    onSlopeUp = true;
                }
                break;
            default:
                break;
        }
        return onSlopeUp;
    }

    // Check if the potential tile that the pieces move into is a SLOPE DOWN
    private bool OnSlopeDown(Vector3 currentMove, Vector3 direction)
    {
        bool onSlopeDown = false;
        TileType tileData = GetTileBelow(currentMove);

        // Object can only stand on GROUND / BOX / SLOPES
        switch (tileData)
        {
            // it's SLOPES
            case TileType.SLOPE_0:
                if (direction.z > 0)
                {
                    onSlopeDown = true;
                }
                break;
            case TileType.SLOPE_90:
                if (direction.x < 0)
                {
                    onSlopeDown = true;
                }
                break;
            case TileType.SLOPE_180:
                if (direction.z < 0)
                {
                    onSlopeDown = true;
                }
                break;
            case TileType.SLOPE_270:
                if (direction.x > 0)
                {
                    onSlopeDown = true;
                }
                break;
            default:
                break;
        }
        return onSlopeDown;
    }

    // Check if the potential tile that the pieces move into is a team's piece
    private bool IsSameTeam(Vector3 currentPosition, Vector3 currentMove)
    {
        // Debug.Log("Current Position: " + currentPosition + GetTile(currentPosition).ToString());
        // Debug.Log("Current Move: " + currentMove + GetTile(currentMove).ToString());
        // return false;

        return (GetTile(currentPosition) == TileType.PLAYER_CHESS && GetTile(currentMove) == TileType.PLAYER_CHESS)
            || (GetTile(currentPosition) == TileType.ENEMY_CHESS && GetTile(currentMove) == TileType.ENEMY_CHESS);
    }

    // Check if the potential tile that the pieces move into is another team's piece
    private bool IsDifferentTeam(Vector3 currentPosition, Vector3 currentMove)
    {
        // Debug.Log("Current Position: " + GetTile(currentPosition).ToString());
        // Debug.Log("Current Move: " + GetTile(currentMove).ToString());
        // return false;

        return (GetTile(currentPosition) == TileType.PLAYER_CHESS && GetTile(currentMove) == TileType.ENEMY_CHESS)
            || (GetTile(currentPosition) == TileType.ENEMY_CHESS && GetTile(currentMove) == TileType.PLAYER_CHESS);
    }

    private bool IsDynamicObject(Vector3 currentMove)
    {
        return GetTile(currentMove) == TileType.BOX || GetTile(currentMove) == TileType.BOULDER;
    }

    public virtual void GenerateMove(Vector3 currentPositionIndex, Vector3 direction)
    {
        // The Vector3 that stores the current position for the next move
        bool dynamicObjectOnDirection = false;
        Vector3 currentMove = currentPositionIndex;
        Vector3 move = currentMove;
        for (int i = 1; i <= moveRange; ++i)
        {
            move = currentMove + direction;
            // Debug.Log("Load " + move.ToString());
            // We find the first tile below the next move
            while (move.y >= 1f && InBound(move) && !IsTile(move))
            {
                move += Vector3.down;
            }
            // Check if the potential move is in bound
            if (!InBound(move))
            {
                break;
            }
            // Check if the potential move is standable
            if (!CanStandOn(move))
            {
                break;
            }
            // Check if the potential move is jumpable
            if (!ValidateJump(move, direction))
            {
                return;
            }
            // Check if the potential move is movable
            if (!ValidateMove(move, direction))
            {
                break;
            }
            if (IsDynamicObject(move))
            {
                if (dynamicObjectOnDirection)
                {
                    break;
                }
                dynamicObjectOnDirection = true;
            }
            // Check if the potential move is into slopes up
            if (OnSlopeUp(move, direction))
            {
                move += Vector3.up;
            }
            // Check if the potential move is into the same team piece
            // The pieces are ALWAYS ABOVE SLOPES
            if (IsSameTeam(currentPositionIndex, move))
            {
                // Debug.Log("Same team spotted");
                break;
            }
            // If here means the move is executable, we add it to the list
            possibleMoveList.Add(move);
            // Check if the potential move is into another team piece
            // The pieces are ALWAYS ABOVE SLOPES
            if (IsDifferentTeam(currentPositionIndex, move))
            {
                // Debug.Log("Different team spotted");
                break;
            }
            // Check if the potential move is into slopes down
            if (OnSlopeDown(move, direction))
            {
                move += Vector3.down;
            }
            // Update the currentMove
            currentMove = move;
        }

        // Dynamic Object check
        // If the last move is out of bound, we don't care about the dynamic objects
        if (!InBound(move))
        {
            return;
        }
    }
    public virtual void GenerateMoveList(Vector3 currentPositionIndex)
    {
        return;
    }

    public List<Vector3> Move(Vector3 currentPositionIndex)
    {
        possibleMoveList.Clear();
        LoadLimit();
        GenerateMoveList(currentPositionIndex);
        return possibleMoveList;
    }

    // Simple AI Mechanics (every pieces except KING)
    // The AI knows all of the player's positions and types and also their own move list and the KINGs.
    // There will be 3 states: PatrolState, RetreatState, KillState (the KING will not have this)
    // The priority queue is as follows: KillState -> RetreatState -> PatrolState
    // The pieces' priority on states: KING -> QUEEN -> CASTLE -> KNIGHT -> BISHOP -> PAWN
    public Vector3 MoveByDefault(Vector3 currentPositionIndex)
    {
        // Debug.Log("AI CHECK");
        possibleMoveList = Move(currentPositionIndex);
        Dictionary<ChessManType, int> chessManPriority = new Dictionary<ChessManType, int>();
        chessManPriority[ChessManType.PAWN] = 0;
        chessManPriority[ChessManType.BISHOP] = 1;
        chessManPriority[ChessManType.KNIGHT] = 2;
        chessManPriority[ChessManType.CASTLE] = 3;
        chessManPriority[ChessManType.QUEEN] = 4;
        chessManPriority[ChessManType.KING] = 5;

        Vector3 patrolState = PatrolState(currentPositionIndex, chessManPriority);
        Vector3 retreatState = RetreatState(currentPositionIndex, chessManPriority);
        Vector3 killState = KillState(chessManPriority);

        // Debug.Log("Decision");
        if (killState != Vector3.zero)
        {
            Debug.Log("Kill State" + killState);
            return killState;
        }
        if (retreatState != Vector3.zero)
        {
            Debug.Log("Retreat State" + retreatState);
            return retreatState;
        }
        Debug.Log("Patrol State" + patrolState);
        return patrolState;
    }

    // KillState: Use the piece to kill the player's piece
    public virtual Vector3 KillState(Dictionary<ChessManType, int> chessManPriority)
    {
        int bestChessManID = 0;
        List<ChessMan> playerArmy = GameplayManager.Instance.playerArmy;
        // We find the best ChessManType in the playerArmy list
        for (int playerIdx = 0; playerIdx < playerArmy.Count; ++playerIdx)
        {
            // Debug.Log("Contain: " + playerArmy[playerIdx].posIndex + " = " + possibleMoveList.Contains(playerArmy[playerIdx].posIndex));
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
                if (UnityEngine.Random.Range(0, 100) >= 50)
                {
                    decision = playerArmy[playerIdx].posIndex;
                }
            }
        }
        return decision;
    }

    // RetreatState: Move the piece away from the danger tiles
    public virtual Vector3 RetreatState(Vector3 currentPositionIndex, Dictionary<ChessManType, int> chessManPriority)
    {
        int[,,] scores = new int[(int)_Xlimit, (int)_Ylimit, (int)_Zlimit];

        List<ChessMan> playerArmy = GameplayManager.Instance.playerArmy;
        for (int playerIdx = 0; playerIdx < playerArmy.Count; ++playerIdx)
        {
            List<Vector3> movePool = playerArmy[playerIdx].config.Move(playerArmy[playerIdx].posIndex);
            foreach (Vector3 move in movePool) scores[(int)move.x, (int)move.y, (int)move.z]--;
        }

        Vector3 decision = Vector3.zero;
        foreach (Vector3 move in possibleMoveList)
        {
            int score = scores[(int)move.x, (int)move.y, (int)move.z];
            if (score >= 0)
            {
                if (decision == Vector3.zero)
                {
                    decision = move;
                }
                // Random pick
                if (UnityEngine.Random.Range(0, 100) >= 50)
                {
                    decision = move;
                }
            }
        }
        return decision;
    }

    // PatrolState: Default
    public virtual Vector3 PatrolState(Vector3 currentPositionIndex, Dictionary<ChessManType, int> chessManPriority)
    {
        return possibleMoveList[UnityEngine.Random.Range(0, possibleMoveList.Count)];
    }
}
