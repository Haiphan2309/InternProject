using GDC.Constants;
using GDC.Enums;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ChessManConfig : ScriptableObject
{
    private float _height;
    private int _moveRange;
    private ChessManType _chessManType;
    private List<Vector3> _possibleMoveList;

    public float height { get; set; }
    public int moveRange { get; set; }
    public ChessManType chessManType { get; set; }
    public List<Vector3> possibleMoveList { get; set; }

    public readonly float[,] _straghtDirection = { { 0f, 1f }, { -1f, 0f }, { 1f, 0f }, { 0f, -1f } };
    public readonly float[,] _diagonalDirection = { { -1f, 1f }, { 1f, 1f }, { -1f, -1f }, { 1f, -1f } };
    public readonly float[,] _knightDirection = {
        { -1f, 2f }, { 1f, 2f }, { 1f, -2f }, { -1f, -2f },
        { -2f, 1f }, { 2f, 1f }, { 2f, -1f }, { -2f, -1f }
    };

    // Check if the potential tile that the pieces move into can be stood on
    private bool CanStandOn(Vector3 currentMove)
    {
        bool canStandOn = true;
        TileType tileData = GameUtils.GetTileBelowObject(currentMove);

        // Object can only stand on GROUND / BOX / SLOPES
        switch (tileData)
        {
            // if it's not GROUND / BOX / SLOPES then it's NONE / OBJECT / BOULDER / WATER / CHESS
            case TileType.NONE:
            case TileType.OBJECT:
            case TileType.BOULDER:
            case TileType.WATER:
            // case TileType.PLAYER_CHESS:
            // case TileType.ENEMY_CHESS:
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
    private bool ValidateMove(Vector3 currentMove, Vector3 direction, bool dynamicObjectOnDirection)
    {
        bool isMovable = false;
        TileType tileData = GameUtils.GetTile(currentMove);

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

            // if WATER, we have to check if we're pushing a DYNAMIC OBJECT (dynamicObjectOnDirection)
            // if pushing return true else false -> return dynamicObjectOnDirection
            case TileType.WATER:
                return dynamicObjectOnDirection;

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
        TileType tileData = GameUtils.GetTile(currentMove);

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
        TileType tileData = GameUtils.GetTileBelowObject(currentMove);

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
        // Debug.Log("Current Position: " + GameUtils.GetTile(currentPosition));
        // Debug.Log("Current Move: " + GameUtils.GetTile(currentMove));
        // return false;
        TileType currentPosType = GameUtils.GetTile(currentPosition);
        TileType currentMoveType = GameUtils.GetTile(currentMove);
        return (currentPosType == TileType.PLAYER_CHESS && currentMoveType == TileType.PLAYER_CHESS)
            || (currentPosType == TileType.ENEMY_CHESS && currentMoveType == TileType.ENEMY_CHESS);
    }

    // Check if the potential tile that the pieces move into is another team's piece
    private bool IsDifferentTeam(Vector3 currentPosition, Vector3 currentMove)
    {
        // Debug.Log("Current Position: " + GetTile(currentPosition).ToString());
        // Debug.Log("Current Move: " + GetTile(currentMove).ToString());
        // return false;
        TileType currentPosType = GameUtils.GetTile(currentPosition);
        TileType currentMoveType = GameUtils.GetTile(currentMove);
        return (currentPosType == TileType.PLAYER_CHESS && currentMoveType == TileType.ENEMY_CHESS)
            || (currentPosType == TileType.ENEMY_CHESS && currentMoveType == TileType.PLAYER_CHESS);
    }

    private bool IsDynamicObject(Vector3 currentMove)
    {
        TileType currentMoveType = GameUtils.GetTile(currentMove);
        return currentMoveType == TileType.BOX || currentMoveType == TileType.BOULDER;
    }

    public virtual void GenerateMove(Vector3 currentPositionIndex, Vector3 direction)
    {
        // The Vector3 that stores the current position for the next move
        bool dynamicObjectOnDirection = false;
        bool isEnemy = false;
        // bool isOnAir = false;
        Vector3 currentMove = currentPositionIndex;
        Vector3 move;
        for (int i = 1; i <= moveRange; ++i)
        {
            // Register the next move
            move = currentMove + direction;

            // Debug.Log("Load " + move.ToString());
            // We find the first tile below the next move
            // If the piece is pushing the OBJECT:
            // 
            while (move.y >= 1f && GameUtils.InBound(move) && !CanStandOn(move))
            {
                if (dynamicObjectOnDirection) return;
                // Find the lower ground
                move += Vector3.down;
                // Debug.Log("DOWN " + move.ToString());
            }

            // Check if the potential move is in bound
            if (!GameUtils.InBound(move))
            {
                // Debug.Log("OOB " + move.ToString());
                return;
            }

            // Check if the potential move is standable
            if (!CanStandOn(move))
            {
                // Debug.Log("STAND " + move.ToString());
                break;
            }

            // Check if the potential move is jumpable
            if (!ValidateJump(move, direction))
            {
                break;
            }

            // Check if the potential move is movable
            // Since there is a WATER - DYNAMIC OBJECT interaction, we put dynamicObjectOnDirection in
            if (!ValidateMove(move, direction, dynamicObjectOnDirection))
            {
                // Debug.Log("MOVE " + move.ToString());
                break;
            }

            // Check if there is a DYNAMIC OBJECT in the potential move
            if (IsDynamicObject(move))
            {
                // Check to see if they are stacked
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
                break;
            }

            // If here means the move is executable, we add it to the list
            possibleMoveList.Add(move);

            // Check if the potential move is into another team piece
            // The pieces are ALWAYS ABOVE SLOPES
            if (IsDifferentTeam(currentPositionIndex, move))
            {
                isEnemy = true;
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

        // Universal pushing detection config
        if (dynamicObjectOnDirection)
        {
            // Custom for PAWN / KING
            if (moveRange <= 1)
            {
                bool canPush = true;
                Vector3 nextMove = currentMove + direction;
                // If the next tile is out of bound we don't care
                if (!GameUtils.InBound(nextMove)) return;

                TileType type = GameUtils.GetTile(nextMove);
                // If the next tile is not OBJECT / CHESS we keep the move
                switch (type)
                {
                    case TileType.OBJECT:
                    case TileType.BOX:
                    case TileType.BOULDER:
                    case TileType.PLAYER_CHESS:
                    case TileType.ENEMY_CHESS:
                        canPush = false;
                        break;
                    default:
                        break;
                }
                if (canPush) return;
            }
            possibleMoveList.RemoveAt(possibleMoveList.Count - 1);

            // If PAWN / KING then this is unchecked
            if (isEnemy)
            {
                possibleMoveList.RemoveAt(possibleMoveList.Count - 1);
            }
        }
    }

    public virtual void GenerateMoveList(Vector3 currentPositionIndex)
    {
        return;
    }

    public List<Vector3> Move(Vector3 currentPositionIndex)
    {
        possibleMoveList.Clear();
        GenerateMoveList(currentPositionIndex);
        return possibleMoveList;
    }

    // Simple AI Mechanics (every pieces except KING)
    // The AI knows all of the player's positions and types and also their own move list and the KINGs.
    // There will be 3 states: PatrolState, RetreatState, KillState (the KING will not have this)
    // The priority queue is as follows: KillState -> RetreatState -> PatrolState
    // The pieces' priority on states: KING -> QUEEN -> CASTLE -> KNIGHT -> BISHOP -> PAWN
    // MAYBE AIManager
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
        int[,,] scores = new int[GameConstants.MAX_X_SIZE, GameConstants.MAX_Y_SIZE, GameConstants.MAX_Z_SIZE];

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
    public virtual Vector3 PatrolState(Vector3 currentPositionIndex, Dictionary<ChessManType, int> chessManPriority = null)
    {
        return currentPositionIndex;
        // return possibleMoveList[UnityEngine.Random.Range(0, possibleMoveList.Count)];
    }
}
