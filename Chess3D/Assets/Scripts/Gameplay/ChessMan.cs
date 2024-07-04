using DG.Tweening;
using GDC;
using GDC.Enums;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

public class ChessMan : GameplayObject
{
    public ChessManConfig config;
    public Vector3 posIndex;
    [SerializeField] float speed;
    //[SerializeField] Vector3 posIndexToMove;
    Vector3 oldPosIndex;

    [SerializeField] GameObject vfxDefeated;
    [SerializeField] LayerMask groundLayerMask;

    public bool isEnemy;
    public int index;
    int moveIndex; //Dung de xac dinh index cua nuoc di ke tiep, danh rieng cho enemy

    public bool isTouchBox = false;
    public bool isTouchBoulder = false;

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
    
    public bool EnemyMove()
    {
        EnemyArmy enemy = GameplayManager.Instance.levelData.GetEnemyArmies()[index];
        if (enemy.isAI)
        {
            Vector3 posIndexToMove = config.MoveByDefault(posIndex);
            GameplayManager.Instance.MakeMove(this, posIndexToMove);
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
            GameplayManager.Instance.MakeMove(this, moves[moveIndex]);
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
        StartCoroutine(Cor_KnightMoveAnim(posIndexToMove));
    }

    IEnumerator Cor_KnightMoveAnim(Vector3 target)
    {
        Vector3 direction = (target - transform.position).normalized;
        RotateToDirection(direction);
        
        yield return new WaitForSeconds(0.5f);
        transform.DOJump(target, 3, 1, 1).SetEase(Ease.InOutSine).OnComplete(() =>
        {
            AjustPosToGround(transform.position, target, target - transform.position, true, true);
            TileInfo tileInfo = GameplayManager.Instance.levelData.GetTileInfoNoDeep(posIndex);

            GameplayManager.Instance.UpdateTile(posIndex, target, tileInfo);
            posIndex = target;
            GameplayManager.Instance.EndTurn();
        });
    }

    void OtherMoveAnim(Vector3 posIndexToMove)
    {
        StartCoroutine(Cor_OtherMoveAnim(posIndexToMove));
    }

    IEnumerator Cor_OtherMoveAnim(Vector3 target)
    {
        SetParentDefault();
        Vector3 currPos = transform.position;
        Vector3 currPosIdx = posIndex;

        Vector3 direction = (target - currPos).normalized;
        RotateToDirection(direction);
        yield return new WaitForSeconds(0.5f);

        List<Vector3> path = CalculatePath(currPosIdx, target);

        foreach (var gridCell in path)
        {
            while (currPos != gridCell)
            {
                if (GetTile(gridCell) == TileType.BOX && !isTouchBox)
                {
                    isTouchBox = true;

                    Vector3 gameplayObjectPosition = GameUtils.SnapToGrid(transform.position) + direction;
                    GameplayObject gameplayObject = GameUtils.GetGameplayObjectByPosition(gameplayObjectPosition);

                    gameplayObject.MoveAnim(SnapToGrid(target + direction), 5f * Time.deltaTime);
                }

                if (GetTile(gridCell) == TileType.BOULDER && !isTouchBoulder)
                {
                    isTouchBoulder = true;
                    GameObject foundObject = null;
                    foreach (GameObject obj in Object.FindObjectsOfType<GameObject>())
                    {
                        if (Vector3.Distance(obj.transform.position, gridCell) < 0.1f)
                        {
                            foundObject = obj;
                            break;
                        }
                    }

                    Boulder gameplayObject = foundObject.transform.GetComponent<Boulder>();
                    Debug.Log(foundObject.transform.name);

                    gameplayObject.MoveAnim(SnapToGrid(target), 5f * Time.deltaTime);
                }
                AjustPosToGround(transform.position, gridCell, direction, true);
                if (!isOnSlope) currPos = transform.position;
                else currPos = transform.position + Vector3.up * 0.4f;
                yield return null;
            }
        }

        AjustPosToGround(transform.position, target, direction, true, true);
        yield return new WaitForSeconds(0.1f);

        TileInfo tileInfo = GameplayManager.Instance.levelData.GetTileInfoNoDeep(posIndex);

        GameplayManager.Instance.UpdateTile(posIndex, target, tileInfo);
        posIndex = target;

        isTouchBox = false;
        isTouchBoulder = false;

        GameplayManager.Instance.EndTurn();
    }

    void RotateToDirection(Vector3 direction)
    {
        Quaternion targetRotation = Quaternion.FromToRotation(Vector3.forward, direction);
        transform.DORotate(Vector3.up * targetRotation.eulerAngles.y, 0.3f);
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
