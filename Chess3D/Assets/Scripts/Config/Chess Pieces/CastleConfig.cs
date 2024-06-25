using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CastleConfig", menuName = "ChessManConfig/CastleConfig", order = 1)]
public class CastleConfig : ChessManConfig
{
    public GameObject prefab;
    private int _moveRange = 16;    // hard-coded number
    private float[,] _straghtDirection = { { 0f, 1f }, { -1f, 0f }, { 1f, 0f }, { 0f, -1f } };

    CastleConfig()
    {
        Debug.Log("Spawn Castle");
        this.chessManType = GDC.Enums.ChessManType.CASTLE;
        this.possibleMoveList = new List<Vector3>();
    }

    public override bool ValidateMove(Vector3 currentMove)
    {
        bool isMovable = true;
        return isMovable;
    }
    public override void GenerateMove(Vector3 currentPositionIndex, Vector3 direction)
    {
        for (int i = 0; i < _moveRange; ++i)
        {
            Vector3 move = currentPositionIndex + direction * i;
            // Check if this potential move is executable
            if (!ValidateMove(move))
            {
                return;
            }
            // If here means the move is executable, we add it to the list
            possibleMoveList.Add(move);
        }
    }
    public override void GenerateMoveList(Vector3 currentPositionIndex)
    {
        for (int i = 0; i < _straghtDirection.Length; ++i)
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