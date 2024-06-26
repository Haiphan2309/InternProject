using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QueenConfig", menuName = "ChessManConfig/KingConfig", order = 4)]
public class QueenConfig : ChessManConfig
{
    public GameObject prefab;

    private int _moveRange = 16;    // hard-coded number
    private float[,] _straghtDirection = { { 0f, 1f }, { -1f, 0f }, { 1f, 0f }, { 0f, -1f } };
    private float[,] _diagonalDirection = { { -1f, 1f }, { 1f, 1f }, { -1f, -1f }, { 1f, -1f } };

    QueenConfig()
    {
        Debug.Log("Spawn Queen");
        this.chessManType = GDC.Enums.ChessManType.QUEEN;
        this.possibleMoveList = new List<Vector3>();
    }

    public override bool CanStandOn(Vector3 currentMove, Vector3 direction)
    {
        bool canStandOn = base.CanStandOn(currentMove, direction);
        return canStandOn;
    }
    public override bool ValidateMove(Vector3 currentMove, Vector3 direction)
    {
        bool isMovable = base.ValidateMove(currentMove, direction);
        return isMovable;
    }
    public override void GenerateMove(Vector3 currentPositionIndex, Vector3 direction)
    {
        for (int i = 0; i < _moveRange; ++i)
        {
            Vector3 move = currentPositionIndex + direction * i;
            if (!CanStandOn(move, direction))
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
        for (int i = 0; i < _straghtDirection.Length; ++i)
        {
            Vector3 direction = Vector3.right * _straghtDirection[i, 0] + Vector3.forward * _straghtDirection[i, 1];
            GenerateMove(currentPositionIndex, direction);
        }
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