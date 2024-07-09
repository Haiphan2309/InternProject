using GDC.Constants;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "KnightConfig", menuName = "ChessManConfig/KnightConfig", order = 2)]
public class KnightConfig : ChessManConfig
{
    public GameObject prefab;
    private float _jumpLimit = 2f;      // hard-coded number

    KnightConfig()
    {
        Debug.Log("Spawn Knight");
        this.moveRange = 1;             // hard-coded number
        this.chessManType = GDC.Enums.ChessManType.KNIGHT;
        this.possibleMoveList = new List<Vector3>();
    }

    // We have to check if this jump is executable, before checking if the move is valid
    public override bool ValidateJump(Vector3 currentMove, Vector3 direction)
    {
        // if the jump height is higher than the world limit
        // then there is no object to block the jump
        // therefore we return true
        if ((currentMove - direction).y + _jumpLimit >= GameConstants.MAX_Y_SIZE)
        {
            return true;
        }

        bool isJumpable = true;

        float Xsign = direction.x / Mathf.Abs(direction.x);
        float Zsign = direction.z / Mathf.Abs(direction.z);

        Vector3 firstBlock = Vector3.right * Xsign + Vector3.forward * Zsign;
        Vector3 secondBlock = direction - firstBlock;

        Vector3 v1 = currentMove - direction + firstBlock + Vector3.up * _jumpLimit;
        Vector3 v2 = currentMove - direction + secondBlock + Vector3.up * _jumpLimit;

        // Because currentMove is a Vector3 that shows the position AFTER moving the object
        // We have to subtract the direction vector to show the initial position of the object
        // Then check the block at y = object.y + _jumpLimit
        GDC.Enums.TileType firstBlockData = GameUtils.GetTile(v1);
        GDC.Enums.TileType secondBlockData = GameUtils.GetTile(v2);
        GDC.Enums.TileType currentBlockData = GameUtils.GetTile(currentMove);

        // if there is something that blocks the jump -> block datas will not be NONE
        if (firstBlockData != GDC.Enums.TileType.NONE || secondBlockData != GDC.Enums.TileType.NONE)
        {
            isJumpable = false;
        }

        // Debug.Log($"Check {currentMove} type {currentBlockData} with firstBlock {v1} type {firstBlockData} and secondBlock {v2} type {secondBlockData}");

        return isJumpable;
    }

    public override void GenerateMoveList(Vector3 currentPositionIndex)
    {
        for (int i = 0; i < _knightDirection.GetLength(0); ++i)
        {
            Vector3 direction = Vector3.right * _knightDirection[i, 0]
                                + Vector3.up * _jumpLimit
                                + Vector3.forward * _knightDirection[i, 1];
            GenerateMove(currentPositionIndex, direction);
        }
    }
}
