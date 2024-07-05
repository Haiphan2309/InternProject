using DG.Tweening;
using GDC.Enums;
using Mono.Cecil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Box : GameplayObject
{
    public override void MoveAnim(Vector3 posIndexToMove, Vector3 direction, float speed)
    //posIndexToMove: ở đây không phải vị trí cuối cùng, mà chỉ là vị trí mà chessman đẩy đến (sát bên chessman),
    //sẽ phải tự tính toàn tiếp vị trí tiếp theo nếu dưới chân nó là None (thì sẽ rơi) với tốc độ defaultSpeed.
    //speed: là speed của chessMan đẩy box, box sẽ di chuyển cùng tốc độ với chessman đẩy box,
    //khi nào đẩy đến rơi xuống vực mới di chuyển với tốc độ defaultSpeed;
    {
        //todo anim
        StartCoroutine(Cor_BoxMoveAnim(posIndexToMove, direction));
    }

    IEnumerator Cor_BoxMoveAnim(Vector3 target, Vector3 direction)
    {
        Vector3 currIdx = GameUtils.SnapToGrid(transform.position);
        this.posIndex = currIdx;
        target = GameUtils.SnapToGrid(CalculateTarget(target, direction));
        Debug.Log("BOX Position: " + currIdx + " Old Target: " + target + " New Target " + GameUtils.SnapToGrid(CalculateTarget(target, direction)));

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

        TileInfo tileInfo = GameplayManager.Instance.levelData.GetTileInfoNoDeep(posIndex);

        GameplayManager.Instance.UpdateTile(posIndex, target, tileInfo);
        this.posIndex = target;

    }

    private Vector3 CalculateTarget(Vector3 target, Vector3 direction)
    {
        Vector3 nextCell = target + direction;
        TileType tile = GameUtils.GetTile(nextCell);
        TileType tileBelow = GameUtils.GetTileBelowObject(nextCell);

        if (GameUtils.CheckSlope(tile))
        {
            nextCell.y += 1;
        }

        if (tileBelow == TileType.GROUND) return nextCell;

        while (tile == TileType.NONE)
        {
            nextCell.y -= 1;

            tile = GameUtils.GetTile(nextCell);

            if (nextCell.y <= -3)
            {
                break;
            }
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

}
