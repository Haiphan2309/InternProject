using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boulder : GameplayObject
{

    public override void MoveAnim(Vector3 posIndexToMove, Vector3 direction, float speed)
    //posIndexToMove: ở đây không phải vị trí cuối cùng, mà chỉ là vị trí mà chessman đẩy đến (sát bên chessman),
    //sẽ phải tự tính toàn tiếp vị trí tiếp theo nếu dưới chân nó là None (thì sẽ rơi) với tốc độ defaultSpeed.
    //speed: là speed của chessMan đẩy box, box sẽ di chuyển cùng tốc độ với chessman đẩy box,
    //khi nào đẩy đến rơi xuống vực mới di chuyển với tốc độ defaultSpeed;
    {
        StartCoroutine(Cor_BoulderMoveAnim(posIndexToMove));
    }

    IEnumerator Cor_BoulderMoveAnim(Vector3 target)
    {
        Debug.Log("Position: " + posIndex + " Target: " + target);

        yield return new WaitForEndOfFrame();
        this.posIndex = target;
    }
}
