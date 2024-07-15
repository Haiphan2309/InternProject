using DG.Tweening;
using GDC.Enums;
using GDC.Managers;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameplayObject : MonoBehaviour
{
    // Vị trí ban đầu của Object
    public Vector3 posIndex;

    // Vị trí kết thúc của Object
    public Vector3 targetPosition = Vector3.zero;

    // Check xem có đang làm anim hay không
    public bool isAnim = false;

    public int index;
    public float defaultSpeed;
    public Outline outline;

    public bool isOnSlope = false;
    public bool isStandOnSlope = false;
    public bool isFalling = false;

    public LayerMask objectLayer;
    [SerializeField] public GameObject vfxDefeated;



    public virtual void MoveAnim(Vector3 posIndexToMove, Vector3 direction, float speed)
    {
        
    }

    public virtual void DestroyAnim()
    {

    }

    public void SetGameplayObjectData(GameplayObjectData gameplayObjectData)
    {
        posIndex = gameplayObjectData.posIndex;
        //index = gameplayObjectData.index;
        transform.position = posIndex;
    }
    public void SetOutline(float width)
    {
        //outline.OutlineColor = color;
        outline.OutlineWidth = width;
    }
    public void SetOutline(float width, Color color)
    {
        outline.OutlineColor = color;
        outline.OutlineWidth = width;
    }

    protected void SetParentDefault()
    {
        transform.SetParent(null);
    }

    protected void MoveToNextPath(ref Vector3 current, Vector3 end)
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

    protected void PrintPath(List<Vector3> path)
    {
        for (int i = 0; i < path.Count; i++)
        {
            Debug.Log($"Path[{i}]: {path[i]}");
        }
    }

    protected List<Vector3> CalculatePath(Vector3 start, Vector3 end)
    {
        List<Vector3> path = new List<Vector3>();
        Vector3 current = start;

        while (current != end)
        {
            MoveToNextPath(ref current, end);
             
            TileType tile = GameUtils.GetTile(current);

            if (GameUtils.CheckSlope(tile))
            {
                current.y += 1;
                path.Add(current);
                continue;
            }

            tile = GameUtils.GetTileBelowObject(current);
            while (tile == TileType.NONE)
            {
                path.Add(current);
                current.y -= 1;
                tile = GameUtils.GetTileBelowObject(current);
                if (current == end) break;
            }

            if (GameUtils.CheckSlope(tile))
            {
                //if (path.Count >= 1 && temp > 0) path.RemoveAt(path.Count - 1);
                path.Add(current);
                current.y -= 1;
                continue;
            }

            // Water and ChessMan below
            if (tile == TileType.WATER || GameUtils.CheckChess(tile))
            {
                current.y -= 1;
                path.Add(current);
                continue;
            }

            path.Add(current);
        }
        return path;
    }

    protected virtual void AjustPosToGround(Vector3 newPosition, Vector3 target, Vector3 direction, bool isChessMan = false, bool isRoundInteger = false)
    {
        Vector3 rotation = transform.rotation.eulerAngles;

        TileType tileType = GameUtils.GetTileBelowObject(GameUtils.SnapToGrid(target));

        if (!isStandOnSlope)
        {
            switch (tileType)
            {
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

                default:
                    rotation = Vector3.zero + Vector3.up * transform.rotation.eulerAngles.y;
                    isOnSlope = false;
                    break;
            }
        }
        else
        {
            if (GameUtils.CheckSlope(tileType))
            {
                isOnSlope = true;
            }
            else
            {
                rotation = Vector3.zero + Vector3.up * transform.rotation.eulerAngles.y;
                isOnSlope = false;
            }
        }

        //switch (tileType)
        //{
        //    case TileType.SLOPE_0:
        //        rotation.x = 45 * (Mathf.Round(direction.normalized.z));
        //        isOnSlope = true;
        //        break;
        //    case TileType.SLOPE_90:
        //        rotation.x = -45 * (Mathf.Round(direction.normalized.x));
        //        isOnSlope = true;
        //        break;
        //    case TileType.SLOPE_180:
        //        rotation.x = -45 * (Mathf.Round(direction.normalized.z));
        //        isOnSlope = true;
        //        break;
        //    case TileType.SLOPE_270:
        //        rotation.x = 45 * (Mathf.Round(direction.normalized.x));
        //        isOnSlope = true;
        //        break;

        //    default:
        //        rotation = Vector3.zero + Vector3.up * transform.rotation.eulerAngles.y;
        //        isOnSlope = false;
        //        break;
        //}



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

    [Button]
    public virtual void Defeated()
    {
        Vector3 posToDissapear = transform.position + new Vector3(Random.Range(0, 2), 2, Random.Range(0, 2));
        transform.DOMove(posToDissapear, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
        {
            Instantiate(vfxDefeated, posToDissapear, Quaternion.identity);
            Destroy(gameObject);
            SoundManager.Instance.PlaySound(AudioPlayer.SoundID.SFX_DISAPPEAR);
            Debug.Log("Destroy Object Successfully");
        });
    }

    public virtual void SetPosIndex()
    {
        TileInfo tileInfo = GameplayManager.Instance.levelData.GetTileInfoNoDeep(posIndex);
        Debug.Log(tileInfo.tileType);
        GameplayManager.Instance.UpdateTile(posIndex, targetPosition, tileInfo);

        Debug.Log("Update Position: " + this.name + " Start: " + posIndex + " Target: " + targetPosition);

        posIndex = targetPosition;

    }
}
