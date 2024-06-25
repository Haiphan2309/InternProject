using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BishopConfig", menuName = "ChessManConfig/BishopConfig", order = 3)]
public class BishopConfig : ChessManConfig
{
    public GameObject prefab;
    private int _moveRange = 16;    // hard-coded number
    private float[,] _diagonalDirection = { { -1f, 1f },  { 1f, 1f }, { -1f, -1f }, { 1f, -1f } };

    BishopConfig()
    {
        Debug.Log("Spawn Bishop");
        this.chessManType = GDC.Enums.ChessManType.BISHOP;
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
        for (int i = 0; i < _diagonalDirection.Length; ++i)
        {
            Vector3 direction = Vector3.right * _diagonalDirection[i, 0] + Vector3.forward * _diagonalDirection[i, 1];
            GenerateMove(currentPositionIndex, direction);
        }
    }

    public override List<Vector3> Move(Vector3 currentPositionIndex)
    {
        GenerateMoveList(currentPositionIndex);
        return possibleMoveList;
    }
}
