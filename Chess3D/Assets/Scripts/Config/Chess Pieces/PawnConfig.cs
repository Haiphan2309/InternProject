using GDC.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PawnConfig : ChessManConfig
{
    PawnConfig()
    {
        Debug.Log("Spawn Pawn");
        this.chessManType = GDC.Enums.ChessManType.PAWN;
    }
    void AllValidMove()
    {
    }
    public override List<Vector3> Move(Vector3 cur)
    {
        return null;
    }
}
