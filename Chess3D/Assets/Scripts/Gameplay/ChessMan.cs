using DG.Tweening;
using GDC.Enums;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UIElements;

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
        //transform.DOJump(posIndexToMove, 3, 1, 1).SetEase(Ease.InOutSine).OnComplete(()=>
        //{
        //    AjustPosToGround(transform.position, posIndexToMove, true);
        //});   
    }

    void OtherMoveAnim(Vector3 posIndexToMove)
    {
        StartCoroutine(Cor_OtherMoveAnim(posIndexToMove));
    }

    private TileType GetTile(Vector3 position)
    {
        float Xpos = position.x;
        float Ypos = position.y-1f;
        float Zpos = position.z;
        return GameplayManager.Instance.levelData.GetTileInfo()[
               (int)Mathf.Round(Xpos),
               (int)Mathf.Round(Ypos),
               (int)Mathf.Round(Zpos)
               ].tileType;
    }

    IEnumerator Cor_OtherMoveAnim(Vector3 target)
    {
        Vector3 initPos = SnapToGrid(transform.position);
        Vector3 direction = (target - initPos).normalized;

        while (direction != Vector3.zero)
        { 
            AjustPosToGround(transform.position, target, direction);
            direction = (target - SnapToGrid(transform.position)).normalized;
            yield return null;
        }

        AjustPosToGround(transform.position, target, direction, true);
        yield return new WaitForSeconds(1);
        GameplayManager.Instance.ChangeTurn();
    }

    void AjustPosToGround(Vector3 newPosition, Vector3 target, Vector3 direction, bool isRoundInteger = false)
    {
        //RaycastHit hit;
        //if (Physics.Raycast(transform.position + Vector3.up / 2f, Vector3.down, out hit, 0.6f, groundLayerMask))
        //{
        //    newPosition = Vector3.MoveTowards(transform.position, target, speed );
        //    // newPosition = target;
        //    newPosition.y = hit.point.y;

        //    Vector3 slopeRotation = Quaternion.FromToRotation(transform.up, hit.normal).eulerAngles;

        //    // transform.DORotate(slopeRotation, 0.3f);
        //}
        //else
        //{
        //    Debug.Log("No ground detected");
        //    newPosition += Vector3.down * 10 * Time.deltaTime;
        //}

        TileType tileType = GetTile(newPosition);
        Debug.Log(tileType);
        Debug.Log(direction);
        newPosition += direction;



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
