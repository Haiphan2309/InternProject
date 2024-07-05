using GDC.Enums;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "PawnConfig", menuName = "ChessManConfig/PawnConfig", order = 0)]
public class PawnConfig : ChessManConfig
{
    public GameObject prefab;

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
        for (int i = possibleMoveList.Count - 1; i >= 0 ; --i)
        {
            TileType currentPosType = GameUtils.GetTile(currentPositionIndex);
            TileType currentMoveType = GameUtils.GetTile(possibleMoveList[i]);

            if ((currentPosType == TileType.PLAYER_CHESS && currentMoveType == TileType.ENEMY_CHESS)
            || (currentPosType == TileType.ENEMY_CHESS && currentMoveType == TileType.PLAYER_CHESS))
            {
                possibleMoveList.RemoveAt(i);
            }
        }
        for (int i = 0; i < _diagonalDirection.GetLength(0); ++i)
        {
            Vector3 direction = Vector3.right * _diagonalDirection[i, 0] + Vector3.forward * _diagonalDirection[i, 1];
            Vector3 currentMove = currentPositionIndex + direction;
            TileType currentPosType = GameUtils.GetTile(currentPositionIndex);
            TileType currentMoveType = GameUtils.GetTile(currentMove);

            if ((currentPosType == TileType.PLAYER_CHESS && currentMoveType == TileType.ENEMY_CHESS)
            || (currentPosType == TileType.ENEMY_CHESS && currentMoveType == TileType.PLAYER_CHESS))
            {
                possibleMoveList.Add(currentMove);
            }
        }
    }
}
