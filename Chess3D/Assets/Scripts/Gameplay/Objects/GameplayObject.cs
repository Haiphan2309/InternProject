using DG.Tweening;
using GDC.Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayObject : MonoBehaviour
{
    public float defaultSpeed;
    public Outline outline;

    public bool isOnSlope = false;
    public bool isOnPathSlope = false;
    public bool isFalling = false;

    public LayerMask objectLayer;

    public virtual void MoveAnim(Vector3 posIndexToMove, float speed)
    {
        Debug.Log("A");
    }

    public virtual void DestroyAnim()
    {

    }

    protected Vector3 SnapToGrid(Vector3 position)
    {
        return new Vector3(Mathf.Round(position.x), Mathf.Round(position.y), Mathf.Round(position.z));
    }

    protected bool CheckSlope(TileType tileType)
    {
        return tileType == TileType.SLOPE_0 || tileType == TileType.SLOPE_90 || tileType == TileType.SLOPE_180 || tileType == TileType.SLOPE_270;
    }

    protected TileType GetTileBelowObject(Vector3 position)
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

    protected TileType GetTile(Vector3 position)
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

    protected void SetParentDefault()
    {
        transform.SetParent(null);
    }

    protected List<Vector3> CalculatePath(Vector3 start, Vector3 end)
    {
        List<Vector3> path = new List<Vector3>();
        Vector3 current = start;

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
                if (CheckSlope(GetTileBelowObject(current + Vector3.down)))
                {
                    current.y -= 1;
                    path.Add(new Vector3(current.x, current.y, current.z));
                    continue;
                }
            }

            // Check the tile above
            Vector3 tileUp = current + Vector3.up;
            TileType tileType = GetTileBelowObject(tileUp);

            if (CheckSlope(tileType))
            {
                current.y += 1;
                path.Add(new Vector3(current.x, current.y, current.z));



                continue;
            }

            // Check the tile
            tileType = GetTileBelowObject(current);
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

        return path;
    }

    protected void AjustPosToGround(Vector3 newPosition, Vector3 target, Vector3 direction, bool isChessMan = false, bool isRoundInteger = false)
    {
        Vector3 rotation = transform.rotation.eulerAngles;

        TileType tileType = GetTileBelowObject(SnapToGrid(target));
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

            if (isChessMan && GetTileBelowObject(SnapToGrid(transform.position)) == TileType.BOX)
            {
                GameplayObject foundObject = null;
                foreach (GameplayObject obj in Object.FindObjectsOfType<GameplayObject>())
                {
                    if (GetTile(SnapToGrid(obj.transform.position)) == TileType.BOX)
                    {
                        foundObject = obj;
                        transform.SetParent(foundObject.transform);
                        Debug.Log(foundObject.transform.name);
                        Debug.Log(transform.parent);
                        Debug.Log("ON BOX");
                        break;
                    }
                }
            }
        }

        else
        {
            transform.position = newPosition;
        }

        transform.DORotate(rotation, 0.3f);
    }
}
