using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class ChessManConfig : ScriptableObject
{
    private float _height;
    private int _moveRange;
    private GDC.Enums.ChessManType _chessManType;
    private List<Vector3> _possibleMoveList;

    public float height { get; set; }
    public int moveRange { get; set; }
    public GDC.Enums.ChessManType chessManType { get; set; }
    public List<Vector3> possibleMoveList { get; set; }

    // Check if the tile below is Standable
    public virtual bool OnBound(Vector3 currentMove)
    {
        bool onBound = true;
        float Xpos = currentMove.x;
        float Ypos = currentMove.y; // check the tile below the object
        float Zpos = currentMove.z;

        float Xlimit = GameplayManager.Instance.levelData.GetTileInfo().GetLength(0);
        float Ylimit = GameplayManager.Instance.levelData.GetTileInfo().GetLength(1);
        float Zlimit = GameplayManager.Instance.levelData.GetTileInfo().GetLength(2);
        if (Xpos < 0 || Xpos >= Xlimit)
        {
            onBound = false;
        }
        if (Ypos < 0 || Ypos >= Ylimit)
        {
            onBound = false;
        }
        if (Zpos < 0 || Zpos >= Zlimit)
        {
            onBound = false;
        }
        return onBound;
    }

    public virtual bool CanStandOn(Vector3 currentMove)
    {
        bool canStandOn = true;
        float Xpos = currentMove.x;
        float Ypos = currentMove.y - 1f; // check the tile below the object
        float Zpos = currentMove.z;
        GDC.Enums.TileType tileData 
            = GameplayManager.Instance.levelData.GetTileInfo()[
                (int)Xpos, 
                (int)Ypos, 
                (int)Zpos
                ].tileType;

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

    public virtual bool ValidateJump(Vector3 currentMove, Vector3 direction)
    {
        bool isJumpable = true;
        return isJumpable;
    }

    public virtual bool ValidateMove(Vector3 currentMove, Vector3 direction)
    {
        bool isMovable = true;
        float Xpos = currentMove.x;
        float Ypos = currentMove.y;
        float Zpos = currentMove.z;

        GDC.Enums.TileType tileData
            = GameplayManager.Instance.levelData.GetTileInfo()[
                (int)Xpos, 
                (int)Ypos,
                (int)Zpos
                ].tileType;

        switch (tileData)
        {
            // if NONE then pass
            case GDC.Enums.TileType.NONE:
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
                isMovable = CanStandOn(currentMove + direction) && ValidateMove(currentMove + direction, direction);
                break;

            // if SLOPES then FUCK YOU
            default:
                break;
        }
        return isMovable;
    }
    public virtual void GenerateMove(Vector3 currentPositionIndex, Vector3 direction)
    {
        for (int i = 1; i <= moveRange; ++i)
        {
            Vector3 move = currentPositionIndex + direction * i;
            if (!OnBound(move))
            {
                return;
            }
            if (!CanStandOn(move))
            {
                return;
            }
            if (!ValidateJump(move, direction))
            {
                return;
            }
            if (!ValidateMove(move, direction))
            {
                return;
            }
            // If here means the move is executable, we add it to the list
            possibleMoveList.Add(move);
        }
    }
    public virtual void GenerateMoveList(Vector3 currentPositionIndex)
    {
        return;
    }
    public virtual List<Vector3> Move(Vector3 currentPositionIndex)
    {
        possibleMoveList.Clear();
        GenerateMoveList(currentPositionIndex);
        return possibleMoveList;
    }
}
