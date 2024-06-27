using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ChessManConfig : ScriptableObject
{
    public float Xlimit = 0;
    public float Ylimit = 0;
    public float Zlimit = 0;

    private float _height;
    private int _moveRange;
    private GDC.Enums.ChessManType _chessManType;
    private List<Vector3> _possibleMoveList;

    public float height { get; set; }
    public int moveRange { get; set; }
    public GDC.Enums.ChessManType chessManType { get; set; }
    public List<Vector3> possibleMoveList { get; set; }

    // Get TileType of a tile below position
    private GDC.Enums.TileType GetTileBelow(Vector3 position)
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

    // Get TileType of a tile at position
    private GDC.Enums.TileType GetTile(Vector3 position)
    {
        float Xpos = position.x;
        float Ypos = position.y;
        float Zpos = position.z;
        return GameplayManager.Instance.levelData.GetTileInfo()[
               (int)Mathf.Round(Xpos),
               (int)Mathf.Round(Ypos),
               (int)Mathf.Round(Zpos)
               ].tileType;
    }

    // Check if the potential tile is available
    public virtual bool IsTile(Vector3 currentMove)
    {
        // if the tile below the currentMove's y-level is NONE
        // then there is no tile below the currentMove.
        bool isTile = false;
        GDC.Enums.TileType tileData = GetTileBelow(currentMove);

        // Object cannot stand on NONE
        switch (tileData)
        {
            // if it's not NONE then it's a tile
            case GDC.Enums.TileType.NONE:
                break;
            // else
            default:
                isTile = true;
                break;
        }
        return isTile;
    }

    // Check if the potential tile is in bound
    public virtual bool InBound(Vector3 currentMove)
    {
        bool inBound = true;
        float Xpos = currentMove.x;
        float Ypos = currentMove.y; // check the tile below the object
        float Zpos = currentMove.z;

        if (Xpos < 0 || Xpos >= Xlimit)
        {
            inBound = false;
        }
        if (Ypos <= 0 || Ypos >= Ylimit)
        {
            inBound = false;
        }
        if (Zpos < 0 || Zpos >= Zlimit)
        {
            inBound = false;
        }
        Debug.Log("InBound = " + inBound);
        return inBound;
    }

    // Check if the potential tile that the pieces move into can be stood on
    public virtual bool CanStandOn(Vector3 currentMove)
    {
        bool canStandOn = true;
        GDC.Enums.TileType tileData = GetTileBelow(currentMove);

        // Object can only stand on GROUND / BOX / SLOPES
        switch (tileData)
        {
            // if it's not GROUND / BOX / SLOPES then it's NONE / OBJECT / BOULDER / WATER
            case GDC.Enums.TileType.NONE:
            case GDC.Enums.TileType.OBJECT:
            case GDC.Enums.TileType.BOULDER:
            case GDC.Enums.TileType.WATER:
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
    public virtual bool ValidateMove(Vector3 currentMove, Vector3 direction)
    {
        bool isMovable = false;
        GDC.Enums.TileType tileData = GetTile(currentMove);

        switch (tileData)
        {
            // if NONE then it's movable
            case GDC.Enums.TileType.NONE:
                isMovable = true;
                break;

            // if STATIC OBJECT then it's not movable
            case GDC.Enums.TileType.GROUND:
            case GDC.Enums.TileType.OBJECT:
                isMovable = false;
                break;

            // if DYNAMIC OBJECT then it can be movable if:
            // The next TileData based on direction is not a STATIC OBJECT
            case GDC.Enums.TileType.BOX:
            case GDC.Enums.TileType.BOULDER:
                // We check the next tile of that direction can be stood on
                // Then check if the next tile is movable
                isMovable = CanStandOn(currentMove + direction) && ValidateMove(currentMove + direction, direction);
                break;

            // if SLOPES then we check based on direction
            case GDC.Enums.TileType.SLOPE_0:
            case GDC.Enums.TileType.SLOPE_180:
                if (direction.z != 0)
                {
                    isMovable = true;
                }
                break;
            case GDC.Enums.TileType.SLOPE_90:
            case GDC.Enums.TileType.SLOPE_270:
                if (direction.x != 0)
                {
                    isMovable = true;
                }
                break;
            default:
                break;
        }
        return isMovable;
    }

    // Check if the potential tile that the pieces move into is a SLOPE UP
    public virtual bool OnSlopeUp(Vector3 currentMove, Vector3 direction)
    {
        bool onSlopeUp = false;
        GDC.Enums.TileType tileData = GetTile(currentMove);

        // Object can only stand on GROUND / BOX / SLOPES
        switch (tileData)
        {
            // it's SLOPES
            case GDC.Enums.TileType.SLOPE_0:
                if (direction.z < 0)
                {
                    onSlopeUp = true;
                }
                break;
            case GDC.Enums.TileType.SLOPE_90:
                if (direction.x > 0)
                {
                    onSlopeUp = true;
                }
                break;
            case GDC.Enums.TileType.SLOPE_180:
                if (direction.z > 0)
                {
                    onSlopeUp = true;
                }
                break;
            case GDC.Enums.TileType.SLOPE_270:
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
    public virtual bool OnSlopeDown(Vector3 currentMove, Vector3 direction)
    {
        bool onSlopeDown = false;
        GDC.Enums.TileType tileData = GetTileBelow(currentMove);

        // Object can only stand on GROUND / BOX / SLOPES
        switch (tileData)
        {
            // it's SLOPES
            case GDC.Enums.TileType.SLOPE_0:
                if (direction.z > 0)
                {
                    onSlopeDown = true;
                }
                break;
            case GDC.Enums.TileType.SLOPE_90:
                if (direction.x < 0)
                {
                    onSlopeDown = true;
                }
                break;
            case GDC.Enums.TileType.SLOPE_180:
                if (direction.z < 0)
                {
                    onSlopeDown = true;
                }
                break;
            case GDC.Enums.TileType.SLOPE_270:
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

    public virtual void GenerateMove(Vector3 currentPositionIndex, Vector3 direction)
    {
        // The Vector3 that stores the current position for the next move
        Vector3 currentMove = currentPositionIndex;
        for (int i = 1; i <= moveRange; ++i)
        {
            Vector3 move = currentMove + direction;
            // We find the first tile below the next move
            while (move.y >= 1f && InBound(move) && !IsTile(move))
            {
                move += Vector3.down;
            }
            // Check if the potential move is in bound
            if (!InBound(move))
            {
                return;
            }
            // Check if the potential move is standable
            if (!CanStandOn(move))
            {
                return;
            }
            // Check if the potential move is jumpable
            if (!ValidateJump(move, direction))
            {
                return;
            }
            // Check if the potential move is movable
            if (!ValidateMove(move, direction))

            {
                return;
            }
            // Check if the potential move is into slopes up
            if (OnSlopeUp(move, direction))
            {
                move += Vector3.up;
            }
            // If here means the move is executable, we add it to the list
            possibleMoveList.Add(move);
            // Check if the potential move is into slopes down
            if (OnSlopeDown(move, direction))
            {
                move += Vector3.down;
            }
            // Update the currentMove
            currentMove = move;
        }
    }
    public virtual void GenerateMoveList(Vector3 currentPositionIndex)
    {
        return;
    }
    public virtual List<Vector3> Move(Vector3 currentPositionIndex)
    {
        possibleMoveList.Clear();
        Xlimit = GameplayManager.Instance.levelData.GetTileInfo().GetLength(0);
        Ylimit = GameplayManager.Instance.levelData.GetTileInfo().GetLength(1);
        Zlimit = GameplayManager.Instance.levelData.GetTileInfo().GetLength(2);
        GenerateMoveList(currentPositionIndex);
        return possibleMoveList;
    }
}
