using DG.Tweening;
using GDC.Enums;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Box : GameplayObject
{
    private bool isDestroy = false;
    private int destroyPositionY = -3;

    public void Setup(Vector3 posIndex)
    {
        this.posIndex = posIndex;
    }
    
    public override void MoveAnim(Vector3 posIndexToMove, Vector3 direction, float speed)
    //posIndexToMove: ở đây không phải vị trí cuối cùng, mà chỉ là vị trí mà chessman đẩy đến (sát bên chessman),
    //sẽ phải tự tính toàn tiếp vị trí tiếp theo nếu dưới chân nó là None (thì sẽ rơi) với tốc độ defaultSpeed.
    //speed: là speed của chessMan đẩy box, box sẽ di chuyển cùng tốc độ với chessman đẩy box,
    //khi nào đẩy đến rơi xuống vực mới di chuyển với tốc độ defaultSpeed;
    {
        //todo anim
        targetPosition = posIndex;
        StartCoroutine(Cor_BoxMoveAnim(posIndexToMove, direction));
    }

    IEnumerator Cor_BoxMoveAnim(Vector3 target, Vector3 direction)
    {
        isAnim = true;
        Vector3 currIdx = GameUtils.SnapToGrid(transform.position);
        target = GameUtils.SnapToGrid(CalculateTarget(target, direction));
        targetPosition = target;
        Debug.Log("BOX Position: " + posIndex + " Target: " + targetPosition);

        // Calculate Path from First Pos to Target Pos
        List<Vector3> path = CalculatePath(currIdx, target);

        // Move
        foreach (var gridCell in path)
        {
            while (currIdx != gridCell)
            {
                AjustPosToGround(transform.position, gridCell, direction, true);

                if (!isOnSlope) currIdx = transform.position;
                else currIdx = transform.position + Vector3.up * 0.4f;

                yield return null;
            }
        }

        yield return null;

        TileType tile = GameUtils.GetTile(GameUtils.SnapToGrid(transform.position));

        Debug.Log(tile);

        if (GameUtils.CheckChess(tile))
        {
            GameplayObject destroyGO = GetChessman(target, target, Vector3.zero);
            GameplayManager.Instance.DefeatEnemyChessMan(destroyGO.index);
            destroyGO.Defeated();
        }
        
        GameplayObject gameplayObject = GetChessman(this.posIndex, target, Vector3.up);

        if (GameUtils.SnapToGrid(transform.position).y <= destroyPositionY)
        {
            IsDrop(gameplayObject);
        }

        isAnim = false;
    }

    private Vector3 CalculateTarget(Vector3 target, Vector3 direction)
    {
        Vector3 nextCell = target + direction;
        TileType tile = GameUtils.GetTile(nextCell);
        TileType tileBelow = GameUtils.GetTileBelowObject(nextCell);

        // Check Slope --> Move up
        if (GameUtils.CheckSlope(tile))
        {
            nextCell.y += 1;
        }

        // Check None --> Drop down
        while (tileBelow == TileType.NONE)
        {
            nextCell.y -= 1;

            tileBelow = GameUtils.GetTileBelowObject(nextCell);

            if (nextCell.y <= -3)
            {
                break;
            }
        }

        // Check Water --> Move down
        if (tileBelow == TileType.WATER)
        {
            nextCell.y -= 1;
        }

        // Check ChessMan --> Delete ChessMan
        if (GameUtils.CheckChess(tileBelow))
        {
            nextCell.y -= 1;
        }

        return nextCell;
    }

    protected override void AjustPosToGround(Vector3 newPosition, Vector3 target, Vector3 direction, bool isChessMan = false, bool isRoundInteger = false)
    {
        Vector3 rotation = transform.rotation.eulerAngles;

        TileType tileType = GameUtils.GetTileBelowObject(GameUtils.SnapToGrid(target));

        switch (tileType)
        {
            case TileType.SLOPE_0:
                rotation.x = 45;
                isOnSlope = true;
                break;
            case TileType.SLOPE_90:
                rotation.z = 45;
                isOnSlope = true;
                break;
            case TileType.SLOPE_180:
                rotation.x = -45;
                isOnSlope = true;
                break;
            case TileType.SLOPE_270:
                rotation.z = -45;
                isOnSlope = true;
                break;

            default:
                rotation = Vector3.zero;
                isOnSlope = false;
                break;
        }

        if (isOnSlope) target = target - Vector3.up * 0.4f;
        newPosition = Vector3.MoveTowards(transform.position, target, 5f * Time.deltaTime);

        if (isRoundInteger)
        {
            transform.position = target;
        }

        else
        {
            transform.position = newPosition;
        }

        transform.DORotate(rotation, 0.3f);
    }

    private void CheckChessman(GameplayObject gameplayObject, Vector3 oldPos, Vector3 target)
    {
        Vector3 chessmanPosIdx = posIndex + Vector3.up;
        Vector3 chessmanTarget = target + Vector3.up;

        if (gameplayObject != null && GameUtils.CheckChess(GameUtils.GetTile(chessmanPosIdx)))
        {
            TileInfo tileInfo = GameplayManager.Instance.levelData.GetTileInfoNoDeep(chessmanPosIdx);

            GameplayManager.Instance.UpdateTile(chessmanPosIdx, chessmanTarget, tileInfo);
            gameplayObject.posIndex = chessmanTarget;
        }
    }

    private GameplayObject GetChessman(Vector3 oldPos, Vector3 target, Vector3 moveVector)
    {
        Vector3 chessmanPosIdx = oldPos + moveVector;
        Vector3 chessmanTarget = target + moveVector;

        Vector3 gameplayObjectPosition = GameUtils.SnapToGrid(chessmanTarget);
        GameplayObject gameplayObject = GameUtils.GetGameplayObjectByPosition(gameplayObjectPosition);

        if (gameplayObject != null && GameUtils.CheckChess(GameUtils.GetTile(chessmanPosIdx)))
        {
            TileInfo tileInfo = GameplayManager.Instance.levelData.GetTileInfoNoDeep(chessmanPosIdx);
            return gameplayObject;
        }

        return null;
    }

    private void IsDrop(GameplayObject gameplayObject)
    {
        isAnim = false;
        if (gameplayObject != null)
        {
            GameplayManager.Instance.UpdateTile(posIndex + Vector3.up);
            gameplayObject.Defeated();
        }
        
        this.Defeated();
    }

    public override void SetPosIndex()
    {
        GameplayObject gameplayObject = GetChessman(this.posIndex, targetPosition, Vector3.up);
        CheckChessman(gameplayObject, this.posIndex, targetPosition);
        base.SetPosIndex();
    }
}
