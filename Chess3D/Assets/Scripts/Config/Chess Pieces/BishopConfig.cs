using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BishopConfig", menuName = "ChessManConfig/BishopConfig", order = 3)]
public class BishopConfig : ChessManConfig
{
    public GameObject prefab;
    private float[,] _diagonalDirection = { { -1f, 1f },  { 1f, 1f }, { -1f, -1f }, { 1f, -1f } };

    BishopConfig()
    {
        Debug.Log("Spawn Bishop");
        this.moveRange = 16;    // hard-coded number
        this.chessManType = GDC.Enums.ChessManType.BISHOP;
        this.possibleMoveList = new List<Vector3>();
    }

    public override void GenerateMoveList(Vector3 currentPositionIndex)
    {
        for (int i = 0; i < _diagonalDirection.GetLength(0); ++i)
        {
            Vector3 direction = Vector3.right * _diagonalDirection[i, 0] + Vector3.forward * _diagonalDirection[i, 1];
            GenerateMove(currentPositionIndex, direction);
        }
    }
}
