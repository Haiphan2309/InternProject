using DG.Tweening;
using GDC.Enums;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessMan : MonoBehaviour
{
    public ChessManConfig config;
    public Vector3 posIndex;
    [SerializeField] float speed;
    [SerializeField] Vector3 posIndexToMove;

    [Button]
    void TestMove()
    {
        OtherMoveAnim(posIndexToMove);
    }
    public void Move(Vector3 posIndexToMove)
    {
        //todo some animaiton
        if (config.chessManType != ChessManType.KNIGHT)
        {
            OtherMoveAnim(posIndexToMove);
        }
        else
        {
            KnightMoveAnim(posIndexToMove);
        }
        GameplayManager.Instance.ChangeTurn(true);
    }
    void KnightMoveAnim(Vector3 posIndexToMove)
    {

    }
    void OtherMoveAnim(Vector3 posIndexToMove)
    {
        StartCoroutine(Cor_OtherMoveAnim(posIndexToMove));
    }
    IEnumerator Cor_OtherMoveAnim(Vector3 target)
    {
        float distance = Vector3.Distance(transform.position, target);
        while (distance > 0.1f)
        {
            Vector3 newPosition = Vector3.MoveTowards(transform.position, target, speed);
            AjustPosToGround(newPosition);
            
            distance = Vector3.Distance(transform.position, target);
            yield return null;
        }

        AjustPosToGround(target);
    }
    void AjustPosToGround(Vector3 newPosition)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1))
        {
            newPosition.y = hit.point.y + 0.5f;

            Vector3 slopeRotation = Quaternion.FromToRotation(transform.up, hit.normal).eulerAngles;
            transform.DORotate(slopeRotation, 0.3f);
            //transform.rotation = slopeRotation * transform.rotation;
        }
        else
        {
            newPosition += Vector3.down * Time.deltaTime * 5;
        }
        transform.position = newPosition;
    }
}
