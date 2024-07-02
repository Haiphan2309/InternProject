using DG.Tweening;
using GDC.Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Box : GameplayObject
{
    private bool isOnSlope = false;
    public bool isAnim = true;
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

    public override void MoveAnim(Vector3 posIndexToMove, float speed, ChessMan chessManAbove = null)
    //posIndexToMove: ở đây không phải vị trí cuối cùng, mà chỉ là vị trí mà chessman đẩy đến (sát bên chessman),
    //sẽ phải tự tính toàn tiếp vị trí tiếp theo nếu dưới chân nó là None (thì sẽ rơi) với tốc độ defaultSpeed.
    //speed: là speed của chessMan đẩy box, box sẽ di chuyển cùng tốc độ với chessman đẩy box,
    //khi nào đẩy đến rơi xuống vực mới di chuyển với tốc độ defaultSpeed;
    {
        //todo anim
        isAnim = false;
        Vector3 direction = transform.position - posIndexToMove;
        Debug.Log("Direction: " + direction + " posIndexToMove: " + posIndexToMove);

        StartCoroutine(Cor_BoxMoveAnim(direction));

        //Nếu chessManAbove != null (là có chessMan đứng trên box) thi nho cap nhat posIndex cho chessManAbove
        if (chessManAbove != null)
        {

        }
    }

    IEnumerator Cor_BoxMoveAnim(Vector3 direction)
    {
        Vector3 initPos = transform.position;
        Vector3 currPos = initPos;
        direction = direction.normalized;

        Debug.Log(direction);

        while(currPos != initPos + direction)
        {
            AjustPosToGround(transform.position, initPos + direction, direction);
            if (!isOnSlope) currPos = transform.position;
            else currPos = transform.position + Vector3.up * 0.4f;
            yield return null;
        }

        AjustPosToGround(transform.position, initPos + direction, direction, true);
        yield return new WaitForEndOfFrame();
        isAnim = true;
    }

    void AjustPosToGround(Vector3 newPosition, Vector3 target, Vector3 direction, bool isRoundInteger = false)
    {
        Vector3 rotation = transform.rotation.eulerAngles;

        TileType tileType = GetChess(SnapToGrid(target));
        switch (tileType)
        {
            case TileType.SLOPE_0:
            case TileType.SLOPE_90:
            case TileType.SLOPE_180:
            case TileType.SLOPE_270:
                rotation.x = -45 * direction.normalized.x;
                isOnSlope = true;
                break;

            default:
                rotation = Vector3.zero + Vector3.up * transform.rotation.eulerAngles.y;
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

    Vector3 SnapToGrid(Vector3 position)
    {
        return new Vector3(Mathf.Round(position.x), Mathf.Round(position.y), Mathf.Round(position.z));
    }

    public override void MoveAnim(Vector3 posIndexToMove, float speed)
    {
        MoveAnim(posIndexToMove, speed, null);
    }
}
