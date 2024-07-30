using DG.Tweening;
using GDC.Enums;
using GDC.Managers;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

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
    public bool isMove = false;

    public LayerMask objectLayer;
    [SerializeField] public GameObject vfxDefeated;
    [SerializeField] public GameObject vfxDrop;

    [SerializeField] protected GameObject parentObject;

    //private void Start()
    //{
    //    parentObject = transform.parent.gameObject;
    //}

    public virtual void MoveAnim(Vector3 posIndexToMove, Vector3 direction, float speed)
    {
        
    }

    public void SetGameplayObjectData(GameplayObjectData gameplayObjectData)
    {
        posIndex = gameplayObjectData.posIndex;
        AjustPosToGround(posIndex);
    }
    public void SetOutline(float width)
    {
        outline.OutlineWidth = width;
    }
    public void SetOutline(float width, Color color)
    {
        outline.OutlineColor = color;
        outline.OutlineWidth = width;
    }

    public IEnumerator HintOutline(GameObject posIcon, float width, Color color)
    {
        outline.OutlineColor = color;
        bool isComplete = false;
        float tolerance = 0.5f;
        float t = Time.deltaTime * 150f;
        float velocity = 0f;
        posIcon.SetActive(true);

        while (!isMove && GameplayManager.Instance.isShowHint)
        {
            if (!isComplete)
            {
                outline.OutlineWidth = Mathf.SmoothDamp(outline.OutlineWidth, width, ref velocity, 0.3f);
                if (outline.OutlineWidth >= width - tolerance) isComplete = true;
            }

            else
            {
                outline.OutlineWidth = Mathf.SmoothDamp(outline.OutlineWidth, 0, ref velocity, 0.3f);
                if (outline.OutlineWidth <= 0 + tolerance) isComplete = false;
            }

            yield return new WaitForEndOfFrame();
        }

        outline.OutlineWidth = 0f;
        posIcon.SetActive(false);
    }

    public void SetParentObject(GameObject parentObject)
    {
        this.parentObject = parentObject;
    }

    protected void SetParentDefault()
    {
        transform.SetParent(parentObject.transform);
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

    protected List<Vector3> CalculatePath(Vector3 start, Vector3 end)
    {
        List<Vector3> path = new List<Vector3>();
        Vector3 current = start;

        while (current != end)
        {
            TileType tile = GameUtils.GetTileBelowObject(current);
            while (tile == TileType.NONE)
            {
                path.Add(current);
                current.y -= 1;
                tile = GameUtils.GetTileBelowObject(current);
                if (current == end) break;
                if (current.y <= -3) return path;
            }

            MoveToNextPath(ref current, end);
             
            tile = GameUtils.GetTile(current);

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
                if (current.y <= -3) return path;
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
    public void AjustPosToGround(Vector3 target)
    {
        Vector3 rotation = RotateBehavior(target);

        if (isOnSlope) target = target - Vector3.up * 0.4f;

        transform.position = target;

        //transform.rotation = Quaternion.Euler(rotation);
        transform.DORotate(rotation, 0.3f);

        isStandOnSlope = isOnSlope;
    }

    protected Vector3 RotateOnSlope(TileType tileType)
    {
        Vector3 rotation = Vector3.zero;
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

        return rotation;
    }

    private Vector3 RotateBehavior(Vector3 target)
    {
        Vector3 rotation = transform.rotation.eulerAngles;

        TileType tileType = GameUtils.GetTileBelowObject(GameUtils.SnapToGrid(target));

        if (!isStandOnSlope)
        {
            rotation = RotateOnSlope(tileType);
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

        return rotation;
    }

    protected virtual void AjustPosToGround(Vector3 newPosition, Vector3 target, Vector3 direction, bool isChessMan = false, bool isRoundInteger = false)
    {
        Vector3 rotation = RotateBehavior(target);

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
        SoundManager.Instance.PlaySound(AudioPlayer.SoundID.SFX_CLICK_CHESSMAN);
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
        GameplayManager.Instance.UpdateTile(posIndex, targetPosition, tileInfo);

        Debug.Log("Update Position: " + this.name + " Start: " + posIndex + " Target: " + targetPosition);

        posIndex = targetPosition;

    }

    public virtual void Drop()
    {
        
    }
}
