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

    public virtual bool ValidateMove(Vector3 currentMove)
    {
        return false;
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
