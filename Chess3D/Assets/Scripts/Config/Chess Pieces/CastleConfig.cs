using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CastleConfig", menuName = "ChessManConfig/CastleConfig", order = 1)]
public class CastleConfig : ChessManConfig
{
    public GameObject prefab;

    private List<Vector3> _possibleMoveList;
    public List<Vector3> possibleMoveList { get; private set; }
    private int _moveRange = 16;    // hard-coded number
    private float[,] _straghtDirection = { { 0f, 1f }, { -1f, 0f }, { 1f, 0f }, { 0f, -1f } };

    CastleConfig()
    {
        Debug.Log("Spawn Castle");
        this.chessManType = GDC.Enums.ChessManType.CASTLE;
        this.possibleMoveList = new List<Vector3>();
    }

    public override void GenerateMove(Vector3 currentPositionIndex, Vector3 direction)
    {
        for (int i = 1; i <= _moveRange; ++i)
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
            if (!ValidateMove(move, direction))
            {
                return;
            }
            // If here means the move is executable, we add it to the list
            possibleMoveList.Add(move);
        }
    }
    public override void GenerateMoveList(Vector3 currentPositionIndex)
    {
        for (int i = 0; i < _straghtDirection.GetLength(0); ++i)
        {
            Vector3 direction = Vector3.right * _straghtDirection[i, 0] + Vector3.forward * _straghtDirection[i, 1];
            GenerateMove(currentPositionIndex, direction);
        }
    }

    public override List<Vector3> Move(Vector3 currentPositionIndex)
    {
        GenerateMoveList(currentPositionIndex);
        return possibleMoveList;
    }
}
