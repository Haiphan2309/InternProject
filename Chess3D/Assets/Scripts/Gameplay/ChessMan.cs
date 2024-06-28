using DG.Tweening;
using GDC.Enums;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.Rendering.UI;
using UnityEngine.UIElements;
using static UnityEngine.RuleTile.TilingRuleOutput;

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
        transform.DOJump(posIndexToMove, 3, 1, 1).SetEase(Ease.InOutSine).OnComplete(() =>
        {
            AjustPosToGround(transform.position, posIndexToMove, true);
        });
    }

    void OtherMoveAnim(Vector3 posIndexToMove)
    {
        StartCoroutine(Cor_OtherMoveAnim(posIndexToMove));
    }

    private TileType GetTile(Vector3 position)
    {
        float Xpos = position.x;
        float Ypos = position.y;
        float Zpos = position.z;
        return GameplayManager.Instance.levelData.GetTileInfo()[
               (int)Mathf.Round(Xpos),
               (int)Mathf.Round(Ypos),
               (int)Mathf.Round(Zpos)
               ].tileType;
    }

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

    IEnumerator Cor_OtherMoveAnim(Vector3 target)
    {
        Vector3 initPos = SnapToGrid(transform.position);
        Vector3 direction = (target - initPos);

        List<Vector3> path = CalculatePath(transform.position, target);

        foreach (var gridCell in path)
        {
            while (transform.position != gridCell)
            {
                AjustPosToGround(transform.position, gridCell);
                
                yield return null;
            }
        }

        AjustPosToGround(transform.position, target, true);
        yield return new WaitForSeconds(1);
        GameplayManager.Instance.ChangeTurn();
    }

    void AjustPosToGround(Vector3 newPosition, Vector3 target, bool isRoundInteger = false)
    {   
        newPosition = Vector3.MoveTowards(transform.position, target, 5f * Time.deltaTime);
        Vector3 rotation = transform.rotation.eulerAngles;

        TileType tileType = GetChess(SnapToGrid(newPosition));
        switch (tileType) {
            case TileType.SLOPE_0:
                rotation.x = 45;
                break;

            case TileType.SLOPE_90:
                rotation.z = 45;
                break;

            case TileType.SLOPE_180:
                rotation.x = -45;
                break;

            case TileType.SLOPE_270:
                rotation.z = -45;
                break;

            default:
                rotation = Vector3.zero;
                break;
        }
        


        if (isRoundInteger)
        {
            transform.position = target;
        }
        else
        {
            transform.position = newPosition;
        }

        transform.DORotate(rotation, 0.01f);
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
            Vector3 tileUp = current + Vector3.up;
            TileType tileType = GetChess(tileUp);

            if (CheckSlope(tileType))
            {
                Debug.Log("Im in SLope");
                current.y += 1;
                path.Add(new Vector3(current.x, current.y, current.z));

                if (current.x != end.x)
                {
                    current.x += Mathf.Sign(end.x - current.x);
                }
                if (current.z != end.z)
                {
                    current.z += Mathf.Sign(end.z - current.z);
                }

                continue;
            }

            tileType = GetChess(current);
            if (tileType == TileType.NONE)
            {
                current.y -= 1;
                path.Add(new Vector3(current.x, current.y, current.z));
                continue;
            }

            else if (CheckSlope(tileType))
            {
                current.y -= 1;
                if (current.x != end.x)
                {
                    current.x += Mathf.Sign(end.x - current.x);
                }
                if (current.z != end.z)
                {
                    current.z += Mathf.Sign(end.z - current.z);
                }
                path.Add(new Vector3(current.x, current.y, current.z));
                continue;
            }

            if (current.x != end.x)
            {
                current.x += Mathf.Sign(end.x - current.x);
            }
            if (current.z != end.z)
            {
                current.z += Mathf.Sign(end.z - current.z);
            }

            path.Add(new Vector3(current.x, current.y, current.z));
        }

        return path;
    }

    bool CheckSlope(TileType tileType)
    {
        return tileType == TileType.SLOPE_0 || tileType == TileType.SLOPE_90 || tileType == TileType.SLOPE_180 || tileType == TileType.SLOPE_270;
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
