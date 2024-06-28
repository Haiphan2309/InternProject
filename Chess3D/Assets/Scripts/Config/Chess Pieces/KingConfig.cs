using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "KingConfig", menuName = "ChessManConfig/KingConfig", order = 5)]
public class KingConfig : ChessManConfig
{
    public GameObject prefab;
    private float[,] _straghtDirection = { { 0f, 1f }, { -1f, 0f }, { 1f, 0f }, { 0f, -1f } };
    private float[,] _diagonalDirection = { { -1f, 1f }, { 1f, 1f }, { -1f, -1f }, { 1f, -1f } };

    KingConfig()
    {
        Debug.Log("Spawn King");
        this.moveRange = 1;    // hard-coded number
        this.chessManType = GDC.Enums.ChessManType.KING;
        this.possibleMoveList = new List<Vector3>();
    }

    public override void GenerateMoveList(Vector3 currentPositionIndex)
    {
        for (int i = 0; i < _straghtDirection.GetLength(0); ++i)
        {
            Vector3 direction = Vector3.right * _straghtDirection[i, 0] + Vector3.forward * _straghtDirection[i, 1];
            GenerateMove(currentPositionIndex, direction);
        }
        for (int i = 0; i < _diagonalDirection.GetLength(0); ++i)
        {
            Vector3 direction = Vector3.right * _diagonalDirection[i, 0] + Vector3.forward * _diagonalDirection[i, 1];
            GenerateMove(currentPositionIndex, direction);
        }
    }
}
