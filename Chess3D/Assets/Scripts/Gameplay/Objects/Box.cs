using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Box : GameplayObject
{
    public override void MoveAnim(Vector3 posIndexToMove, float speed, bool isRoundInteger, ChessMan chessManAbove = null)
    //posIndexToMove: ở đây không phải vị trí cuối cùng, mà chỉ là vị trí mà chessman đẩy đến (sát bên chessman),
    //sẽ phải tự tính toàn tiếp vị trí tiếp theo nếu dưới chân nó là None (thì sẽ rơi) với tốc độ defaultSpeed.
    //speed: là speed của chessMan đẩy box, box sẽ di chuyển cùng tốc độ với chessman đẩy box,
    //khi nào đẩy đến rơi xuống vực mới di chuyển với tốc độ defaultSpeed;
    {
        //todo anim
        Vector3 direction = transform.position - posIndexToMove;

        if (isRoundInteger)
        {
            transform.position = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), Mathf.Round(transform.position.z));
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, transform.position + direction, speed);
        }

        //Nếu chessManAbove != null (là có chessMan đứng trên box) thi nho cap nhat posIndex cho chessManAbove
        if (chessManAbove != null)
        {

        }
    }
    public override void MoveAnim(Vector3 posIndexToMove, float speed, bool isRoundInteger = false)
    {
        MoveAnim(posIndexToMove, speed, isRoundInteger, null);
    }
}
