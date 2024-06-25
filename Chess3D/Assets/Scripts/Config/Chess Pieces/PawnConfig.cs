using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PawnConfig : ChessManConfig
{
    private float[] XmovementList = { 1f, 0f, -1f, 0f };
    private float[] ZmovementList = { 0f, 1f, 0f, -1f };
    PawnConfig()
    {
        Debug.Log("Spawn Pawn");
        this.chessManType = GDC.Enums.ChessManType.PAWN;
    }
    public override List<Vector3> Move(Vector3 currentPosIndex)
    {
        List<Vector3> moveableList = new List<Vector3>();
        for(int i = 0; i < XmovementList.Length; ++i)
        {
            moveableList.Add(currentPosIndex + Vector3.forward * ZmovementList[i] + Vector3.right * XmovementList[i]);
        }
        return moveableList;
    }
}
