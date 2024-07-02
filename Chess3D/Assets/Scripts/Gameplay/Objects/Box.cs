using DG.Tweening;
using GDC.Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Box : GameplayObject
{
    private TileType GetChess(Vector3 position)
    {
        float Xpos = position.x;
        float Ypos = position.y - 1f;
        float Zpos = position.z;
        return GameplayManager.Instance.levelData.GetTileInfo()[
               (int)Mathf.Round(Xpos),
               (int)Mathf.Round(Ypos),
               (int)Mathf.Round(Zpos)
               ].tileType;
    }

    public override void MoveAnim(Vector3 posIndexToMove, float speed, bool isRoundInteger, ChessMan chessManAbove = null)
    //posIndexToMove: ở đây không phải vị trí cuối cùng, mà chỉ là vị trí mà chessman đẩy đến (sát bên chessman),
    //sẽ phải tự tính toàn tiếp vị trí tiếp theo nếu dưới chân nó là None (thì sẽ rơi) với tốc độ defaultSpeed.
    //speed: là speed của chessMan đẩy box, box sẽ di chuyển cùng tốc độ với chessman đẩy box,
    //khi nào đẩy đến rơi xuống vực mới di chuyển với tốc độ defaultSpeed;
    {
        //todo anim
        Vector3 direction = transform.position - posIndexToMove;

        StartCoroutine(Cor_MoveAnim(direction, speed, isRoundInteger));

        //Nếu chessManAbove != null (là có chessMan đứng trên box) thi nho cap nhat posIndex cho chessManAbove
        if (chessManAbove != null)
        {

        }
    }

    IEnumerator Cor_MoveAnim(Vector3 direction, float speed, bool isRoundInteger)
    {
        direction = direction.normalized;
        do
        {
            if (isRoundInteger)
            {
                transform.position = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), Mathf.Round(transform.position.z));
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, transform.position + direction, speed);
            }
            yield return null;
        }
        while (GetChess(transform.position) == TileType.NONE);   
    }
    public override void MoveAnim(Vector3 posIndexToMove, float speed, bool isRoundInteger = false)
    {
        MoveAnim(posIndexToMove, speed, isRoundInteger, null);
    }
}
