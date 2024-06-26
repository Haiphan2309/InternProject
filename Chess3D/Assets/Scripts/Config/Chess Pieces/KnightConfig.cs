using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "KnightConfig", menuName = "ChessManConfig/KnightConfig", order = 2)]
public class KnightConfig : ChessManConfig
{
    public GameObject prefab;

    private int _moveRange = 1;    // hard-coded number
    private float[,] _knightDirection = { 
        { -1f, 2f }, { 1f, 2f }, { 1f, -2f }, { -1f, -2f },
        { -2f, 1f }, { 2f, 1f }, { 2f, -1f }, { -2f, -1f }
    };

    KnightConfig()
    {
        Debug.Log("Spawn Knight");
        this.chessManType = GDC.Enums.ChessManType.KNIGHT;
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
        for (int i = 0; i < _knightDirection.Length; ++i)
        {
            Vector3 direction = Vector3.right * _knightDirection[i, 0] + Vector3.forward * _knightDirection[i, 1];
            GenerateMove(currentPositionIndex, direction);
        }
    }

    public override List<Vector3> Move(Vector3 currentPositionIndex)
    {
        GenerateMoveList(currentPositionIndex);
        return possibleMoveList;
    }
}
