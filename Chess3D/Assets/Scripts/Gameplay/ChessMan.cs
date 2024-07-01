using DG.Tweening;
using GDC.Enums;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.UI;
using UnityEngine.UIElements;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class ChessMan : MonoBehaviour
{
    public ChessManConfig config;
    public Vector3 posIndex;
    [SerializeField] float speed;
    //[SerializeField] Vector3 posIndexToMove;
    Vector3 oldPosIndex;

    [SerializeField] GameObject vfxDefeated;
    public Outline outline;
    [SerializeField] LayerMask groundLayerMask;

    public bool isEnemy;
    public int index;
    int moveIndex; //Dung de xac dinh index cua nuoc di ke tiep, danh rieng cho enemy

    bool isFalling = true;
    bool isOnSlope = false;
    bool isOnPathSlope = false;

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
    //[Button]
    //void TestOtherMove()
    //{
    //    OtherMoveAnim(posIndexToMove);
    //}
    //[Button]
    //void TestKnightMove()
    //{
    //    KnightMoveAnim(posIndexToMove);
    //}
    public bool EnemyMove()
    {
        EnemyArmy enemy = GameplayManager.Instance.levelData.GetEnemyArmies()[index];
        if (enemy.isAI)
        {
            Move(config.MoveByDefault(posIndex));
        }
        else
        {
            List<Vector3> moves = enemy.movePosIndexs;
            if (moves.Count == 0)
            {
                Debug.LogError(gameObject.name + " khong co nuoc di mac dinh nao ca!");
                return false;
            }

            Vector3 intendedMove = moves[moveIndex];
            if (GameplayManager.Instance.CheckMove(config, posIndex, intendedMove) == false)
            {
                return false;
            }
            Move(moves[moveIndex]);
            moveIndex = (moveIndex + 1) % moves.Count;
        }
        return true;
    }
    public void Move(Vector3 posIndexToMove)
    {
        if (config == null)
        {
            Debug.LogError(gameObject.name + " chua co config");
            return;
        }

        if (config.chessManType != ChessManType.KNIGHT)
        {
            OtherMoveAnim(posIndexToMove);
        }
        else
        {
            KnightMoveAnim(posIndexToMove);
        }

        // posIndex = posIndexToMove;
        // GameplayManager.Instance.ChangeTurn(true);
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
        Vector3 currPos = transform.position;
        Vector3 currPosIdx = posIndex;

        List<Vector3> path = CalculatePath(currPosIdx, target);

        foreach (var gridCell in path)
        {
            while (currPos != gridCell)
            {
                AjustPosToGround(transform.position, gridCell);
                if (!isOnSlope) currPos = transform.position;
                else currPos = transform.position + Vector3.up * 0.4f;
                yield return null;
            }
        }

        AjustPosToGround(transform.position, target, true);
        yield return new WaitForSeconds(1);
        posIndex = target;
        GameplayManager.Instance.ChangeTurn();
    }

    void AjustPosToGround(Vector3 newPosition, Vector3 target, bool isRoundInteger = false)
    {   
        Vector3 rotation = transform.rotation.eulerAngles;

        TileType tileType = GetChess(SnapToGrid(target));
        switch (tileType) {
            case TileType.SLOPE_0:
                rotation.x = 45;
                isOnSlope = true;
                break;

            case TileType.SLOPE_90:
                rotation.z = 45;
                isOnSlope = true;
                break;

            case TileType.SLOPE_180:
                rotation.x = -45;
                isOnSlope = true;
                break;

            case TileType.SLOPE_270:
                rotation.z = -45;
                isOnSlope = true;
                break;

            case TileType.BOX:
                break;

            case TileType.BOULDER:
                break;

            default:
                rotation = Vector3.zero;
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
        return new Vector3(Mathf.Floor(position.x), Mathf.Floor(position.y), Mathf.Floor(position.z));
    }

    Vector3 SnapToSlope(Vector3 position)
    {
        return new Vector3(Mathf.Ceil(position.x), Mathf.Ceil(position.y), Mathf.Ceil(position.z));
    }

    List<Vector3> CalculatePath(Vector3 start, Vector3 end)
    {
        List<Vector3> path = new List<Vector3>();
        Vector3 current = start;

        Debug.Log("Start: " + start + "; End" + end);
        isOnPathSlope = isOnSlope;

        while (current != end) 
        {
            // Move to next tile
            if (!isFalling || isOnPathSlope)
            {
                if (current.x != end.x)
                {
                    current.x += Mathf.Sign(end.x - current.x);
                }
                if (current.z != end.z)
                {
                    current.z += Mathf.Sign(end.z - current.z);
                }
            }
            isFalling = false;

            if (isOnPathSlope)
            {
                isOnPathSlope = false;
                if (CheckSlope(GetChess(current + Vector3.down)))
                {
                    current.y -= 1;
                    path.Add(new Vector3(current.x, current.y, current.z));
                    continue;
                }
            }

            // Check the tile above
            Vector3 tileUp = current + Vector3.up;
            TileType tileType = GetChess(tileUp);

            if (CheckSlope(tileType))
            {
                current.y += 1;
                path.Add(new Vector3(current.x, current.y, current.z));
                continue;
            }

            // Check the tile
            tileType = GetChess(current);
            if (tileType == TileType.NONE || tileType == TileType.PLAYER_CHESS || tileType == TileType.ENEMY_CHESS)
            {
                if (isOnPathSlope)
                {
                    isFalling = false;
                }
                else isFalling = true;

                path.Add(new Vector3(current.x, current.y, current.z));
                current.y -= 1;

                continue;
            }

            else if (CheckSlope(tileType))
            {
                if (isOnPathSlope)
                {
                    current.y -= 1;
                    path.Add(new Vector3(current.x, current.y, current.z));
                }
                else
                {
                    path.Add(new Vector3(current.x, current.y, current.z));
                    current.y -= 1;
                }

                continue;
            }

            path.Add(new Vector3(current.x, current.y, current.z));
        }
        LogVector3List(path);

        return path;
    }

    public void LogVector3List(List<Vector3> vectorList, string message = "Vector3 List:")
    {
        Debug.Log(message);
        foreach (Vector3 vec in vectorList)
        {
            Debug.Log(vec.ToString());
        }
    }

    bool CheckTwoLastElement(List<Vector3> list)
    {
        if (list.Count < 2)
        {
            return false; // Not enough elements to compare
        }

        return list[list.Count - 1].Equals(list[list.Count - 2]);
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
