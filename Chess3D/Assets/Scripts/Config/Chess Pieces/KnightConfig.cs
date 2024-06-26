using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

[CreateAssetMenu(fileName = "KnightConfig", menuName = "ChessManConfig/KnightConfig", order = 2)]
public class KnightConfig : ChessManConfig
{
    public GameObject prefab;

    private List<Vector3> _possibleMoveList;
    public List<Vector3> possibleMoveList { get; private set; }
    private float _jumpLimit = 2f;      // hard-coded number
    private int _moveRange = 1;         // hard-coded number
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

    // THE FACT
    // THE FACT THAT THIS FUCKER CAN JUMP
    // EVERYTIME I WAKE UP
    // AND THIS PIECE HAUNTS MEEEEEEEEEEEEEEEE
    // We have to check if this jump is executable, before checking if the move is valid
    private bool ValidateJump(Vector3 currentMove, Vector3 direction)
    {
        bool isJumpable = true;

        float Xsign = direction.x / Mathf.Abs(direction.x);
        float Zsign = direction.z / Mathf.Abs(direction.z);

        Vector3 firstBlock = Vector3.right * Xsign + Vector3.forward * Zsign;
        Vector3 secondBlock = direction - firstBlock;

        // Because currentMove is a Vector3 that shows the position AFTER moving the object
        // We have to subtract the direction vector to show the initial position of the object
        // Then check the block at y = object.y + _jumpLimit
        GDC.Enums.TileType firstBlockData
            = GameplayManager.Instance.levelData.GetTileInfo()[
                (int)((currentMove - direction + firstBlock).x), 
                (int)((currentMove - direction).y + _jumpLimit), 
                (int)((currentMove - direction + firstBlock).z)
                ].tileType;

        GDC.Enums.TileType secondBlockData
            = GameplayManager.Instance.levelData.GetTileInfo()[
                (int)((currentMove - direction + secondBlock).x),
                (int)((currentMove - direction).y + _jumpLimit),
                (int)((currentMove - direction + secondBlock).z)
                ].tileType;

        // if there is something that blocks the jump -> block datas will not be NONE
        if (firstBlockData != GDC.Enums.TileType.NONE || secondBlockData != GDC.Enums.TileType.NONE)
        {
            isJumpable= false;
        }
        return isJumpable;
    }

    public override void GenerateMove(Vector3 currentPositionIndex, Vector3 direction)
    {
        for (int i = 1; i <= _moveRange; ++i)
        {
            Vector3 move = currentPositionIndex + direction * i;
            if (!OnBound(move))
            {
                return;
            }
            if (!CanStandOn(move))
            {   
                return;
            }
            if (!ValidateJump(move, direction))
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
        for (int i = 0; i < _knightDirection.GetLength(0); ++i)
        {
            for(int j = -(int)_jumpLimit; j < (int)_jumpLimit; ++j)
            {
                Vector3 direction
                    = Vector3.right * _knightDirection[i, 0]
                    + Vector3.up * (float)j
                    + Vector3.forward * _knightDirection[i, 1];
                GenerateMove(currentPositionIndex, direction);
            }
        }
    }

    public override List<Vector3> Move(Vector3 currentPositionIndex)
    {
        GenerateMoveList(currentPositionIndex);
        return possibleMoveList;
    }
}
