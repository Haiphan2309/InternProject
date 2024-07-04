using DG.Tweening;
using GDC.Enums;
using Mono.Cecil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Box : GameplayObject
{
    public override void MoveAnim(Vector3 posIndexToMove, float speed)
    //posIndexToMove: ở đây không phải vị trí cuối cùng, mà chỉ là vị trí mà chessman đẩy đến (sát bên chessman),
    //sẽ phải tự tính toàn tiếp vị trí tiếp theo nếu dưới chân nó là None (thì sẽ rơi) với tốc độ defaultSpeed.
    //speed: là speed của chessMan đẩy box, box sẽ di chuyển cùng tốc độ với chessman đẩy box,
    //khi nào đẩy đến rơi xuống vực mới di chuyển với tốc độ defaultSpeed;
    {
        //todo anim
        StartCoroutine(Cor_BoxMoveAnim(posIndexToMove));
    }

    IEnumerator Cor_BoxMoveAnim(Vector3 target)
    {
        Vector3 startPos = GameUtils.SnapToGrid(transform.position);
        Vector3 currPos = startPos;

        Vector3 direction = (target - currPos).normalized;

        target = CalculateTarget(target);

        List<Vector3> path = CalculatePath(currPos, target, isBox: true);

        foreach (var gridCell in path)
        {
            while (currPos != gridCell)
            {
                AjustPosToGround(transform.position, gridCell, direction, true);
                if (!isOnSlope) currPos = transform.position;
                else currPos = transform.position + Vector3.up * 0.4f;
                yield return null;
            }
        }

        AjustPosToGround(transform.position, target, direction, true, true);

        TileInfo tileInfo = GameplayManager.Instance.levelData.GetTileInfoNoDeep(startPos);
        Debug.Log(tileInfo.tileType);
        GameplayManager.Instance.UpdateTile(startPos, GameUtils.SnapToGrid(target), tileInfo);
        
    }

    protected override void AjustPosToGround(Vector3 newPosition, Vector3 target, Vector3 direction, bool isChessMan = false, bool isRoundInteger = false)
    {
        Vector3 rotation = transform.rotation.eulerAngles;

        TileType tileType = GetTileBelowObject(SnapToGrid(target));
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
                rotation = Vector3.zero + Vector3.up * transform.rotation.eulerAngles.y;
                isOnSlope = false;
                break;
        }

        if (isOnSlope) target = target - Vector3.up * 0.4f;
        newPosition = Vector3.MoveTowards(transform.position, target, 5f * Time.deltaTime);

        //transform.DOMove(target, 0.3f).SetEase(Ease.Linear).OnComplete(() =>
        //{

        //});

        if (isRoundInteger)
        {
            transform.position = target;

            if (isChessMan && GetTileBelowObject(SnapToGrid(transform.position)) == TileType.BOX)
            {
                Vector3 gameplayObjectPosition = GameUtils.SnapToGrid(transform.position) + Vector3.down;

                GameplayObject gameplayObject = GameUtils.GetGameplayObjectByPosition(gameplayObjectPosition);

                Debug.Log("Stay on GamplayObject: " + gameplayObject.transform.name);

                transform.SetParent(gameplayObject.transform);
            }
        }

        else
        {
            transform.position = newPosition;
        }

        transform.DORotate(rotation, 0.3f);
    }

    private Vector3 CalculateTarget(Vector3 target)
    {
        TileType tileType = GameUtils.GetTile(target);

        if (GameUtils.CheckSlope(tileType))
        {
            target.y += 1;
        }

        

        return target;
    }
}
