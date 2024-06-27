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

    [SerializeField] GameObject vfxDefeated;
    public Outline outline;

    [Button]
    void TestOtherMove()
    {
        OtherMoveAnim(posIndexToMove);
    }
    [Button]
    void TestKnightMove()
    {
        KnightMoveAnim(posIndexToMove);
    }
    public void Move(Vector3 posIndexToMove)
    {
        if (config == null)
        {
            Debug.LogError(gameObject.name + " chua co config");
            return;
        }

        posIndex = posIndexToMove;
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
        transform.DOJump(posIndexToMove, 3, 1, 1).SetEase(Ease.InOutSine).OnComplete(()=>
        {
            AjustPosToGround(transform.position);
        });   
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

        AjustPosToGround(target, true);
    }
    void AjustPosToGround(Vector3 newPosition, bool isRoundInterger = false)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up/2f, Vector3.down, out hit, 2))
        {
            newPosition.y = hit.point.y;

            Vector3 slopeRotation = Quaternion.FromToRotation(transform.up, hit.normal).eulerAngles;
            transform.DORotate(slopeRotation, 0.3f);
            //transform.rotation = slopeRotation * transform.rotation;
        }
        else
        {
            newPosition += Vector3.down * Time.deltaTime * 5;
        }

        if (isRoundInterger)
        {
            transform.position = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), Mathf.Round(transform.position.z));
        }
        else
        {
            transform.position = newPosition;
        }
    }
    [Button]
    public void Defeated()
    {
        Vector3 posToDissapear = transform.position + new Vector3(Random.Range(0,2), 2, Random.Range(0,2));
        transform.DOMove(posToDissapear, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
        {
            Instantiate(vfxDefeated, posToDissapear, Quaternion.identity);
        });
    }
}
