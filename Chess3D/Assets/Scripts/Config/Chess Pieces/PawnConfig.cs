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
    public override List<Vector3> Move()
    {

    }
}
