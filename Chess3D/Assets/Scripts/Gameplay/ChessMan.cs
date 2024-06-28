using DG.Tweening;
using GDC.Enums;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
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

    public void Setup(PlayerArmy playerArmy, int index, Vector3 posIndex)
    {
        isEnemy = false;
        this.index = index;
        this.posIndex = posIndex;
    }
    public void Setup(EnemyArmy enemyArmy, int index, Vector3 posIndex)
    {
        isEnemy = true;
        this.index = index;
        this.posIndex = posIndex;
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
    public bool EnemyMove()
    {
        List<Vector3> moves = GameplayManager.Instance.levelData.GetEnemyArmies()[index].movePosIndexs;
        if (moves.Count == 0)
        {
            Debug.LogError(gameObject.name + " khong co nuoc di mac dinh nao ca!");
            return false;
        }

        Vector3 intendedMove = moves[moveIndex];
        if (GameplayManager.Instance.CheckMove(config,posIndex,intendedMove) == false)
        {
            return false;
        }
        Move(moves[moveIndex]);
        moveIndex = (moveIndex + 1) % moves.Count;
        return true;
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
        Vector3 currentPos = SnapToGrid(transform.position);
        Vector3 direction = (target - currentPos).normalized;

        float distance = Vector3.Distance(transform.position, target);
        while (distance >= 0.05f)
        {
            AjustPosToGround(transform.position, target);
            distance = Vector3.Distance(transform.position, target);
            yield return null;
        }
        AjustPosToGround(transform.position, target, true);
        yield return new WaitForSeconds(1);
        GameplayManager.Instance.ChangeTurn();
    }

    void AjustPosToGround(Vector3 newPosition, Vector3 target, bool isRoundInteger = false)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up / 2f, Vector3.down, out hit, 0.6f, groundLayerMask))
        {
            newPosition = Vector3.MoveTowards(transform.position, target, speed );
            // newPosition = target;
            newPosition.y = hit.point.y;

            Vector3 slopeRotation = Quaternion.FromToRotation(transform.up, hit.normal).eulerAngles;

            // transform.DORotate(slopeRotation, 0.3f);
        }
        else
        {
            Debug.Log("No ground detected");
            newPosition += Vector3.down * 10 * Time.deltaTime;
        }

        if (isRoundInteger)
        {
            transform.position = target;
        }
        else
        {
            transform.position = newPosition;
        }
    }

    Vector3 SnapToGrid(Vector3 position)
    {
        return new Vector3(Mathf.Floor(position.x), Mathf.Floor(position.y), Mathf.Floor(position.z));
    }

    List<Vector3> CalculatePath(Vector3 start, Vector3 end)
    {
        List<Vector3> path = new List<Vector3>();
        Vector3 current = start;

        while (current != end)
        {
            if (current.x != end.x)
            {
                current.x += Mathf.Sign(end.x - current.x);
            }
            if (current.y != end.y)
            {
                current.y += Mathf.Sign(end.y - current.y);
            }
            if (current.z != end.z)
            {
                current.z += Mathf.Sign(end.z - current.z);
            }
            path.Add(new Vector3(current.x, start.y, current.z));
        }

        foreach (var item in path) Debug.Log(item);

        return path;
    }

    [Button]
    public void Defeated()
    {
        Vector3 posToDissapear = transform.position + new Vector3(Random.Range(0,2), 2, Random.Range(0,2));
        transform.DOMove(posToDissapear, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
        {
            Instantiate(vfxDefeated, posToDissapear, Quaternion.identity);
            if (isEnemy)
            {
                GameplayManager.Instance.DefeatEnemyChessMan(index);
            }
            else
            {
                GameplayManager.Instance.DefeatPlayerChessMan(index);
            }
            Destroy(gameObject);
        });
    }
}
