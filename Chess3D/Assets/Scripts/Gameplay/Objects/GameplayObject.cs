using DG.Tweening;
using GDC.Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayObject : MonoBehaviour
{
    public Vector3 posIndex;
    public float defaultSpeed;
    public Outline outline;

    public bool isOnSlope = false;
    public bool isOnPathSlope = false;
    public bool isFalling = false;

    public LayerMask objectLayer;

    public virtual void MoveAnim(Vector3 posIndexToMove, Vector3 direction, float speed)
    {
        Debug.Log("A");
    }

    public virtual void DestroyAnim()
    {

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
                if (path.Count >= 1) path.RemoveAt(path.Count - 1);
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

        if (GameUtils.CheckSlope(tileType))
        {
            rotation.x = -45 * direction.normalized.x;
            isOnSlope = true;
        }
        else
        {
            rotation = Vector3.zero + Vector3.up * transform.rotation.eulerAngles.y;
            isOnSlope = false;
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
}
