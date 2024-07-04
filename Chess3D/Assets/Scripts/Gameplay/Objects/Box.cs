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

        List<Vector3> path = CalculatePath(currPos, target);

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
}
