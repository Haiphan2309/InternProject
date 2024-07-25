using DG.Tweening;
using GDC;
using GDC.Enums;
using GDC.Managers;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Box : GameplayObject
{
    private bool isDestroy = false;
    private int destroyPositionY = -3;
    private bool isDropToWater = false;
    private int boxCount = 0;

    private GameObject holder;

    [SerializeField] private GameObject vfxWaterSplash;

    public void Setup(Vector3 posIndex)
    {
        this.posIndex = posIndex;
        this.holder = transform.parent.gameObject;
    }
    
    public override void MoveAnim(Vector3 posIndexToMove, Vector3 direction, float speed)
    {
        //todo anim
        targetPosition = posIndex;
        StartCoroutine(Cor_BoxMoveAnim(posIndexToMove, direction));
    }

    IEnumerator Cor_BoxMoveAnim(Vector3 target, Vector3 direction)
    {
        transform.parent = holder.transform;


        isAnim = true;
        Vector3 currIdx = GameUtils.SnapToGrid(transform.position);
        target = GameUtils.SnapToGrid(CalculateTarget(target, direction));
        targetPosition = target;
        Debug.Log("BOX Position: " + posIndex + " Target: " + targetPosition);

        // Calculate Path from First Pos to Target Pos
        List<Vector3> path = CalculatePath(currIdx, target);

        // Move
        foreach (var gridCell in path)
        {
            while (currIdx != gridCell)
            {
                if (GameUtils.GetTile(GameUtils.SnapToGrid(transform.position)) == TileType.WATER && !isDropToWater)
                {
                    Instantiate(vfxWaterSplash, target + Vector3.up, Quaternion.identity);
                    SoundManager.Instance.PlaySound(AudioPlayer.SoundID.SFX_WATER_SPLASH);
                    isDropToWater = true;
                }

                AjustPosToGround(transform.position, gridCell, direction, true);

                if (!isOnSlope) currIdx = transform.position;
                else currIdx = transform.position + Vector3.up * 0.4f;

                yield return null;
            }
        }

        yield return null;

        TileType tile = GameUtils.GetTile(GameUtils.SnapToGrid(transform.position));

        if (tile == TileType.ENEMY_CHESS)
        {
            GameplayObject destroyGO = GetChessman(target, target, Vector3.zero);
            GameplayManager.Instance.DefeatEnemyChessMan(destroyGO.index);
            destroyGO.Defeated();
        }
        else if (tile == TileType.PLAYER_CHESS)
        {
            GameplayObject destroyGO = GetChessman(target, target, Vector3.zero);
            GameplayManager.Instance.DefeatPlayerChessMan(destroyGO.index);
            destroyGO.Defeated();
        }

        GameplayObject gameplayObject = GetChessman(this.posIndex, target, Vector3.up);

        if (GameUtils.SnapToGrid(transform.position).y <= destroyPositionY)
        {
            IsDrop(gameplayObject);
        }

        isAnim = false;
        isDropToWater = false;

        CheckBox(target);

    }

    private Vector3 CalculateTarget(Vector3 target, Vector3 direction)
    {
        Vector3 nextCell = target + direction;
        TileType tile = GameUtils.GetTile(nextCell);
        TileType tileBelow = GameUtils.GetTileBelowObject(nextCell);

        // Check Slope --> Move up
        if (GameUtils.CheckSlope(tile))
        {
            nextCell.y += 1;
        }

        // Check None --> Drop down
        while (tileBelow == TileType.NONE)
        {
            nextCell.y -= 1;

            tileBelow = GameUtils.GetTileBelowObject(nextCell);

            if (nextCell.y <= -3)
            {
                break;
            }
        }

        // Check Water --> Move down
        if (tileBelow == TileType.WATER)
        {
            nextCell.y -= 1;
        }

        // Check ChessMan --> Delete ChessMan
        if (GameUtils.CheckChess(tileBelow))
        {
            nextCell.y -= 1;
        }

        return nextCell;
    }

    protected override void AjustPosToGround(Vector3 newPosition, Vector3 target, Vector3 direction, bool isChessMan = false, bool isRoundInteger = false)
    {
        Vector3 rotation = transform.rotation.eulerAngles;

        TileType tileType = GameUtils.GetTileBelowObject(GameUtils.SnapToGrid(target));

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

    private void CheckBox(Vector3 target)
    {
        Vector3 gameplayObjectPosition = GameUtils.SnapToGrid(target + Vector3.down);
        GameplayObject gameplayObject = GameUtils.GetGameplayObjectByPosition(gameplayObjectPosition);

        if (gameplayObject != null && GameUtils.GetTile(gameplayObject.transform.position) == TileType.BOX)
        {
            transform.SetParent(gameplayObject.transform);
        }
    }

    private void CheckChessman(GameplayObject gameplayObject, Vector3 oldPos, Vector3 target)
    {
        Vector3 chessmanPosIdx = posIndex + Vector3.up;
        Vector3 chessmanTarget = target + Vector3.up;

        if (gameplayObject != null && GameUtils.CheckChess(GameUtils.GetTile(chessmanPosIdx)))
        {
            TileInfo tileInfo = GameplayManager.Instance.levelData.GetTileInfoNoDeep(chessmanPosIdx);

            GameplayManager.Instance.UpdateTile(chessmanPosIdx, chessmanTarget, tileInfo);
            gameplayObject.posIndex = chessmanTarget;
        }
    }

    private void CheckBoxUpper(GameplayObject gameplayObject, Vector3 oldPosm, Vector3 target)
    {
        Vector3 boxPosIdx = posIndex + Vector3.up;
        Vector3 boxTarget = target + Vector3.up;

        if (gameplayObject != null && GameUtils.CheckBox(GameUtils.GetTile(boxPosIdx)))
        {
            TileInfo tileInfo = GameplayManager.Instance.levelData.GetTileInfoNoDeep(boxPosIdx);

            GameplayManager.Instance.UpdateTile(boxPosIdx, boxTarget, tileInfo);
            gameplayObject.posIndex = boxTarget;
        }
    }

    private GameplayObject GetChessman(Vector3 oldPos, Vector3 target, Vector3 moveVector)
    {
        Vector3 chessmanPosIdx = oldPos + moveVector;
        Vector3 chessmanTarget = target + moveVector;

        Vector3 gameplayObjectPosition = GameUtils.SnapToGrid(chessmanTarget);
        GameplayObject gameplayObject = GameUtils.GetGameplayObjectByPosition(gameplayObjectPosition);

        if (gameplayObject != null && GameUtils.CheckChess(GameUtils.GetTile(chessmanPosIdx)))
        {
            TileInfo tileInfo = GameplayManager.Instance.levelData.GetTileInfoNoDeep(chessmanPosIdx);
            return gameplayObject;
        }

        return null;
    }

    private GameplayObject GetBox(Vector3 oldPos, Vector3 target, Vector3 moveVector)
    {
        Vector3 chessmanPosIdx = oldPos + moveVector;
        Vector3 chessmanTarget = target + moveVector;

        Vector3 gameplayObjectPosition = GameUtils.SnapToGrid(chessmanTarget);
        GameplayObject gameplayObject = GameUtils.GetGameplayObjectByPosition(gameplayObjectPosition);

        if (gameplayObject != null && GameUtils.CheckBox(GameUtils.GetTile(chessmanPosIdx)))
        {
            TileInfo tileInfo = GameplayManager.Instance.levelData.GetTileInfoNoDeep(chessmanPosIdx);
            return gameplayObject;
        }

        return null;
    }

    private void IsDrop(GameplayObject gameplayObject)
    {
        isAnim = false;
        if (gameplayObject != null)
        {
            GameplayManager.Instance.UpdateTile(posIndex + Vector3.up);
            gameplayObject.transform.parent = null;

            if (gameplayObject is ChessMan)
            {
                ChessMan chessManGO = (ChessMan)gameplayObject;
                GameplayManager.Instance.uiGameplayManager.uIChessManPanel.DisableChess(chessManGO);
                if (chessManGO.isEnemy)
                {
                    GameplayManager.Instance.DefeatEnemyChessMan(chessManGO.index);
                }
                else
                {
                    GameplayManager.Instance.DefeatPlayerChessMan(chessManGO.index);
                }
            }
            gameplayObject.Defeated();
        }
        
        this.Defeated();
    }

    public override void Drop()
    {
        StartCoroutine(Cor_BoxMoveAnim(GameUtils.SnapToGrid(transform.position + Vector3.down), Vector3.down));
    }

    public override void SetPosIndex()
    {
        GameplayObject gameplayObject = GetChessman(this.posIndex, targetPosition, Vector3.up);
        GameplayObject box = GetBox(this.posIndex, targetPosition, Vector3.up);
        CheckChessman(gameplayObject, this.posIndex, targetPosition);
        CheckBoxUpper(box, this.posIndex, targetPosition);
        base.SetPosIndex();
    }

    public override void Defeated()
    {
        Vector3 posToDissapear = transform.position + Vector3.up * 0.5f;
        Instantiate(vfxDefeated, posToDissapear, Quaternion.identity);
        Destroy(gameObject);
        SoundManager.Instance.PlaySound(AudioPlayer.SoundID.SFX_DISAPPEAR);
    }
}
