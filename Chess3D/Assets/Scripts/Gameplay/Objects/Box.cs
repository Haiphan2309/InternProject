﻿using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : GameplayObject
{
    public override void MoveAnim(Vector3 posIndexToMove, float speed, ChessMan chessManAbove = null)
    //posIndexToMove: ở đây không phải vị trí cuối cùng, mà chỉ là vị trí mà chessman đẩy đến (sát bên chessman),
    //sẽ phải tự tính toàn tiếp vị trí tiếp theo nếu dưới chân nó là None (thì sẽ rơi) với tốc độ defaultSpeed.
    //speed: là speed của chessMan đẩy box, box sẽ di chuyển cùng tốc độ với chessman đẩy box,
    //khi nào đẩy đến rơi xuống vực mới di chuyển với tốc độ defaultSpeed;
    {
        //todo anim
        Vector3 direction = transform.position - posIndexToMove;


        //Nếu chessManAbove != null (là có chessMan đứng trên box) thi nho cap nhat posIndex cho chessManAbove
    }
}
