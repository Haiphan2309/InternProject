using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boulder : GameplayObject
{

    public override void MoveAnim(Vector3 posIndexToMove, float speed)
    //posIndexToMove: ở đây không phải vị trí cuối cùng, mà chỉ là vị trí mà chessman đẩy đến (sát bên chessman),
    //sẽ phải tự tính toàn tiếp vị trí tiếp theo nếu dưới chân nó là None (thì sẽ rơi) với tốc độ defaultSpeed.
    //speed: là speed của chessMan đẩy box, box sẽ di chuyển cùng tốc độ với chessman đẩy box,
    //khi nào đẩy đến rơi xuống vực mới di chuyển với tốc độ defaultSpeed;
    {
        StartCoroutine(Cor_BoulderMoveAnim(posIndexToMove));
    }

    IEnumerator Cor_BoulderMoveAnim(Vector3 target)
    {
        Vector3 currPos = SnapToGrid(transform.position);

        Vector3 direction = (target - currPos).normalized;

        List<Vector3> path = CalculatePath(currPos, target + direction);

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

        AjustPosToGround(transform.position, target + direction, direction, true, true);
    }
}
