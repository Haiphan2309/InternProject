using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "PawnConfig", menuName = "ChessManConfig/PawnConfig", order = 0)]
public class PawnConfig : ChessManConfig
{
    public GameObject prefab;
    private float[,] _straghtDirection = { { 0f, 1f }, { -1f, 0f }, { 1f, 0f }, { 0f, -1f } };
    PawnConfig()
    {
        Debug.Log("Spawn Pawn");
        this.moveRange = 1;    // hard-coded number
        this.chessManType = GDC.Enums.ChessManType.PAWN;
        this.possibleMoveList = new List<Vector3>();
    }

    public override void GenerateMoveList(Vector3 currentPositionIndex)
    {
        for (int i = 0; i < _straghtDirection.GetLength(0); ++i)
        {
            Vector3 direction = Vector3.right * _straghtDirection[i, 0] + Vector3.forward * _straghtDirection[i, 1];
            GenerateMove(currentPositionIndex, direction);
        }
    }
}
