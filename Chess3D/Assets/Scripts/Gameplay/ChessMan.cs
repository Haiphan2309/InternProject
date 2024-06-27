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
    [SerializeField] LayerMask groundLayerMask;

    public bool isEnemy;
    public int index;
    int moveIndex; //Dung de xac dinh index cua nuoc di ke tiep, danh rieng cho enemy

    bool isFalling;

    public void Setup(PlayerArmy playerArmy, int index)
    {
        isEnemy = false;
        this.index = index;
    }
    public void Setup(EnemyArmy enemyArmy, int index)
    {
        isEnemy = true;
        this.index = index;
    }
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
    public void EnemyMove()
    {
        List<Vector3> moves = GameplayManager.Instance.levelData.GetEnemyArmies()[index].movePosIndexs;
        if (moves.Count == 0)
        {
            Debug.LogError(gameObject.name + " khong co nuoc di mac dinh nao ca!");
            return;
        }
        Move(moves[moveIndex]);
        index = (index + 1) % moves.Count;
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
        //GameplayManager.Instance.ChangeTurn(true);
    }
    void KnightMoveAnim(Vector3 posIndexToMove)
    {
        transform.DOJump(posIndexToMove, 3, 1, 1).SetEase(Ease.InOutSine).OnComplete(()=>
        {
            AjustPosToGround(transform.position, posIndexToMove, true);
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
            AjustPosToGround(transform.position, target);
            
            distance = Vector3.Distance(transform.position, target);
            yield return null;
        }

        AjustPosToGround(transform.position, target, true);
        yield return new WaitForSeconds(1);
        GameplayManager.Instance.ChangeTurn(true);
    }
    void AjustPosToGround(Vector3 newPosition, Vector3 target, bool isRoundInterger = false)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up/2f, Vector3.down, out hit, 0.6f, groundLayerMask))
        {
            newPosition = Vector3.MoveTowards(transform.position, target, speed);
            newPosition.y = hit.point.y;

            Vector3 slopeRotation = Quaternion.FromToRotation(transform.up, hit.normal).eulerAngles;
            transform.DORotate(slopeRotation, 0.3f);
            //transform.rotation = slopeRotation * transform.rotation;
        }
        else
        {
            newPosition += Vector3.down * 10* Time.deltaTime;
        }

        if (isRoundInterger)
        {
            //transform.position = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), Mathf.Round(transform.position.z));
            transform.position = target;
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
