using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessManConfig : ScriptableObject
{
    private float _height;
    private GDC.Enums.ChessManType _chessManType;
    private List<Vector3> _possibleMoveList;
    public float height { get; set; }
    public GDC.Enums.ChessManType chessManType { get; set; }
    public List<Vector3> possibleMoveList { get; set; }

    // Check if the tile below is Standable
    public virtual bool CanStandOn(Vector3 currentMove, Vector3 direction)
    {
        bool canStandOn = true;
        float Xpos = currentMove.x;
        float Ypos = currentMove.y - 1f; // check the tile below the object
        float Zpos = currentMove.z;
        GDC.Enums.TileType tileData = GameplayManager.Instance.levelData.GetTileInfo()[(int)Xpos, (int)Ypos, (int)Zpos].tileType;

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

    public virtual bool ValidateMove(Vector3 currentMove, Vector3 direction)
    {
        bool isMovable = true;
        float Xpos = currentMove.x;
        float Ypos = currentMove.y;
        float Zpos = currentMove.z;

        GDC.Enums.TileType tileData = GameplayManager.Instance.levelData.GetTileInfo()[(int)Xpos, (int)Ypos, (int)Zpos].tileType;

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
                isMovable = CanStandOn(currentMove + direction, direction) && ValidateMove(currentMove + direction, direction);
                break;

            // if SLOPES then FUCK YOU
            default:
                break;
        }
        return isMovable;
    }
    public virtual void GenerateMove(Vector3 currentPositionIndex, Vector3 direction)
    {
        return;
    }
    public virtual void GenerateMoveList(Vector3 currentPositionIndex)
    {
        return;
    }


    public virtual List<Vector3> Move(Vector3 currentPositionIndex)
    {
        return null;
    }
}
