using DG.Tweening;
using GDC;
using GDC.Enums;
using GDC.Managers;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class ChessMan : GameplayObject
{
    public ChessManConfig config;

    [SerializeField] LayerMask groundLayerMask;

    public bool isEnemy;
    public int index;
    int moveIndex; //Dung de xac dinh index cua nuoc di ke tiep, danh rieng cho enemy

    public bool isTouchBox = false;
    public bool isTouchBoulder = false;

    int deltaMoveIndex = 1; //Biến này dùng để xác định enemy di chuyển theo chiều tới hoặc chiều lùi theo pattern (1 là tới, -1 là lùi)

    public void Setup(PlayerArmy playerArmy, int index, Vector3 posIndex)
    {
        isEnemy = false;
        this.index = index;
        this.posIndex = posIndex;
    }
    public void Setup(EnemyArmy enemyArmy, int index, Vector3 posIndex)
    {
        isEnemy = true;
        this.index = index;
        this.posIndex = posIndex;
    }

    public bool EnemyMove()
    {
        EnemyArmy enemy = GameplayManager.Instance.levelData.GetEnemyArmies()[index];
        if (enemy.isAI)
        {
            Vector3 posIndexToMove = config.MoveByDefault(posIndex);
            GameplayManager.Instance.MakeMove(this, posIndexToMove);
        }
        else
        {
            List<Vector3> moves = enemy.movePosIndexs;
            if (moves.Count == 0)
            {
                Debug.LogError(gameObject.name + " khong co nuoc di mac dinh nao ca!");
                return false;
            }

            Vector3 intendedMove = moves[moveIndex];
            if (GameplayManager.Instance.CheckMove(config, posIndex, intendedMove) == false)
            {
                deltaMoveIndex = -deltaMoveIndex; //Dao nguoc chieu di chuyen lai
                int backMoveIndex = moveIndex + deltaMoveIndex*2; //x2 để ko tính cái ô mà mình đang đứng mà là tính cái ô trước đó theo default move
                if (backMoveIndex < 0) backMoveIndex = moves.Count - 1;
                backMoveIndex = backMoveIndex % moves.Count;

                intendedMove = moves[backMoveIndex];
                //Debug.Log(intendedMove);
                
                if (GameplayManager.Instance.CheckMove(config, posIndex, intendedMove) == false)
                {
                    deltaMoveIndex = -deltaMoveIndex;
                    return false;
                }
                moveIndex = backMoveIndex;
            }
            GameplayManager.Instance.MakeMove(this, moves[moveIndex]);

            moveIndex = moveIndex + deltaMoveIndex;
            if (moveIndex < 0) moveIndex = moves.Count - 1;
            moveIndex = moveIndex % moves.Count;
        }
        return true;
    }
    public void Move(Vector3 posIndexToMove)
    {
        if (config == null)
        {
            Debug.LogError(gameObject.name + " chua co config");
            return;
        }

        if (config.chessManType != ChessManType.KNIGHT)
        {
            OtherMoveAnim(posIndexToMove);
        }
        else
        {
            KnightMoveAnim(posIndexToMove);
        }
    }
    void KnightMoveAnim(Vector3 posIndexToMove)
    {
        StartCoroutine(Cor_KnightMoveAnim(posIndexToMove));
    }

    IEnumerator Cor_KnightMoveAnim(Vector3 target)
    {
        Vector3 direction = (target - transform.position).normalized;
        RotateToDirection(direction);

        yield return new WaitForSeconds(0.5f);
        transform.DOJump(target, 3, 1, 1).SetEase(Ease.InOutSine).OnComplete(() =>
        {
            // AjustPosToGround(transform.position, target, target - transform.position, true, true);
            TileInfo tileInfo = GameplayManager.Instance.levelData.GetTileInfoNoDeep(posIndex);

            GameplayManager.Instance.UpdateTile(posIndex, target, tileInfo);
            posIndex = target;
            GameplayManager.Instance.EndTurn();
        });
    }

    void OtherMoveAnim(Vector3 posIndexToMove)
    {
        StartCoroutine(Cor_OtherMoveAnim(posIndexToMove));
    }

    IEnumerator Cor_OtherMoveAnim(Vector3 target)
    {
        // Unset Parent for chess piece
        SetParentDefault();

        // First Pos + Target Pos
        Debug.Log("CHESSMAN Position: " + posIndex + " Target: " + target);

        // // Store current position and current index
        Vector3 currPos = transform.position;
        Vector3 currIdx = posIndex;

        Vector3 direction = (target - currPos).normalized;
        direction.y = 0;

        Debug.Log("Direction: " + direction);

        // Rotate to target
        RotateToDirection(direction);
        yield return new WaitForSeconds(0.5f);

        // Calculate Path from First Pos to Target Pos
        List<Vector3> path = CalculatePath(currIdx, target);

        // Move
        foreach (var gridCell in path)
        {
            Debug.Log("Grid: " + gridCell);
            Vector3 gameplayObjectPosition = GameUtils.SnapToGrid(gridCell);
            GameplayObject gameplayObject = GameUtils.GetGameplayObjectByPosition(gameplayObjectPosition);
            Vector3 boxDirection = direction;
            boxDirection.y = 0;

            if (gameplayObject != null)
            {
                gameplayObject.MoveAnim(gridCell, boxDirection, 5f * Time.deltaTime);
                yield return null;
            }
            

            while (currPos != gridCell)
            {
                AjustPosToGround(transform.position, gridCell, direction, true);

                if (!isOnSlope) currPos = transform.position;
                else currPos = transform.position + Vector3.up * 0.4f;

                yield return null;
            }
        }

        yield return new WaitForSeconds(0.5f);

        TileInfo tileInfo = GameplayManager.Instance.levelData.GetTileInfoNoDeep(posIndex);

        GameplayManager.Instance.UpdateTile(posIndex, target, tileInfo);
        posIndex = target;

        CheckBox(target);

        GameplayManager.Instance.EndTurn();
    }

    void RotateToDirection(Vector3 direction)
    {
        Quaternion targetRotation = Quaternion.FromToRotation(Vector3.forward, direction);
        transform.DORotate(Vector3.up * targetRotation.eulerAngles.y, 0.3f);
    }

    private void CheckBox(Vector3 target)
    {
        Vector3 gameplayObjectPosition = GameUtils.SnapToGrid(target + Vector3.down);
        GameplayObject gameplayObject = GameUtils.GetGameplayObjectByPosition(gameplayObjectPosition);

        if (gameplayObject != null && GameUtils.GetTile(gameplayObject.transform.position) == TileType.BOX)
        {
            transform.SetParent(gameplayObject.transform);
        }

    }
}
