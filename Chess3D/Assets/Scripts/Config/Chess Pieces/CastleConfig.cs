using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CastleConfig", menuName = "ChessManConfig/CastleConfig", order = 1)]
public class CastleConfig : ChessManConfig
{
    public GameObject prefab;

    CastleConfig()
    {
        Debug.Log("Spawn Castle");
        this.moveRange = 16;    // hard-coded number
        this.chessManType = GDC.Enums.ChessManType.CASTLE;
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
