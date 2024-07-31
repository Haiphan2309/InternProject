using DG.Tweening;
using GDC.Enums;
using GDC.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boulder : GameplayObject
{
    private bool isDestroy = false;
    private int destroyPositionY = -3;

    public void Setup(Vector3 posIndex, int index)
    {
        this.index = index;
        this.posIndex = posIndex;
    }

    public override void MoveAnim(Vector3 posIndexToMove, Vector3 direction, float speed)
    //posIndexToMove: ở đây không phải vị trí cuối cùng, mà chỉ là vị trí mà chessman đẩy đến (sát bên chessman),
    //sẽ phải tự tính toàn tiếp vị trí tiếp theo nếu dưới chân nó là None (thì sẽ rơi) với tốc độ defaultSpeed.
    //speed: là speed của chessMan đẩy box, box sẽ di chuyển cùng tốc độ với chessman đẩy box,
    //khi nào đẩy đến rơi xuống vực mới di chuyển với tốc độ defaultSpeed;
    {
        //todo anim
        targetPosition = posIndex;
        StartCoroutine(Cor_BoulderMoveAnim(posIndexToMove, direction));
    }

    IEnumerator Cor_BoulderMoveAnim(Vector3 target, Vector3 direction)
    {
        if (direction == Vector3.zero) yield break;
        isAnim = true;

        Vector3 initTarget = GameUtils.SnapToGrid(target + direction);

        Vector3 currIdx = GameUtils.SnapToGrid(transform.position);
        direction = GameUtils.SnapToGrid(direction);
        target = GameUtils.SnapToGrid(CalculateTarget(target, direction));
        targetPosition = target;
        Debug.Log("BOULDER Position: " + posIndex + " Target: " + targetPosition);

        // Calculate Path from First Pos to Target Pos
        List<Vector3> path = CalculatePath(currIdx, target);

        // Move
        foreach (var gridCell in path)
        {
            while (currIdx != gridCell)
            {
                AjustPosToGround(transform.position, gridCell, direction, true);

                if (!isOnSlope) currIdx = transform.position;
                else currIdx = transform.position + Vector3.up * 0.4f;

                yield return null;
            }

            TileType tile = GameUtils.GetTile(GameUtils.SnapToGrid(gridCell));

            if (tile == TileType.ENEMY_CHESS)
            {
                GameplayObject destroyGO = GetChessman(gridCell, gridCell, Vector3.zero);
                GameplayManager.Instance.DefeatEnemyChessMan(destroyGO.index);
                GameplayManager.Instance.SetTile(destroyGO.posIndex, TileType.NONE);
                destroyGO.Defeated();
            }
            else if (tile == TileType.PLAYER_CHESS)
            {
                GameplayObject destroyGO = GetChessman(gridCell, gridCell, Vector3.zero);
                GameplayManager.Instance.DefeatPlayerChessMan(destroyGO.index);
                GameplayManager.Instance.SetTile(destroyGO.posIndex, TileType.NONE);
                destroyGO.Defeated();
            }
            else if (tile == TileType.BOX)
            {
                GameplayObject destroyGO = GameUtils.GetGameplayObjectByPosition(gridCell);
                GameplayManager.Instance.UpdateTile(gridCell);
                GameplayManager.Instance.SetTile(destroyGO.posIndex, TileType.NONE);
                destroyGO.Defeated();
            }
            else if (tile == TileType.WATER)
            {
                SoundManager.Instance.PlaySound(AudioPlayer.SoundID.SFX_WATER_SPLASH);
                IsDrop();
            }
        }

        yield return new WaitForSeconds(0.2f);

        if (GameUtils.SnapToGrid(transform.position).y <= destroyPositionY)
        {
            IsDrop();
        }

        if (initTarget.y > GameUtils.SnapToGrid(transform.position).y)
        {
            SoundManager.Instance.PlaySound(AudioPlayer.SoundID.SFX_CLICK_TILE);
            Instantiate(vfxDrop, GameUtils.SnapToGrid(transform.position), Quaternion.identity);
        }

        isAnim = false;
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

        while (GameUtils.CheckSlope(tileBelow))
        {
            nextCell.y -= 1;

            switch (tileBelow)
            {
                case TileType.SLOPE_0:
                    direction = Vector3.forward;
                    break;
                case TileType.SLOPE_180:
                    direction = Vector3.back;
                    break;
                case TileType.SLOPE_90:
                    direction = Vector3.left;
                    break;
                case TileType.SLOPE_270:
                    direction = Vector3.right;
                    break;
                default:
                    break;
            }

            nextCell += direction;

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

    private void IsDrop()
    {
        isAnim = false;

        this.Defeated();
    }

    public override void SetPosIndex()
    {
        GameplayObject gameplayObject = GetChessman(this.posIndex, targetPosition, Vector3.up);
        CheckChessman(gameplayObject, this.posIndex, targetPosition);
        SoundManager.Instance.PlaySound(AudioPlayer.SoundID.SFX_DISAPPEAR);
        base.SetPosIndex();
    }

    private IEnumerator Cor_DropAnim(Vector3 target, Vector3 direction)
    {
        isAnim = true;

        Vector3 currIdx = GameUtils.SnapToGrid(transform.position);
        direction = GameUtils.SnapToGrid(direction);
        target = GameUtils.SnapToGrid(CalculateTarget(target, direction));
        targetPosition = target;
        Debug.Log("BOULDER Position: " + posIndex + " Target: " + targetPosition);

        // Calculate Path from First Pos to Target Pos
        List<Vector3> path = CalculatePath(currIdx, target);

        // Move
        foreach (var gridCell in path)
        {
            while (currIdx != gridCell)
            {
                AjustPosToGround(transform.position, gridCell, direction, true);

                if (!isOnSlope) currIdx = transform.position;
                else currIdx = transform.position + Vector3.up * 0.4f;

                yield return null;
            }

            TileType tile = GameUtils.GetTile(GameUtils.SnapToGrid(gridCell));

            if (tile == TileType.ENEMY_CHESS)
            {
                GameplayObject destroyGO = GetChessman(gridCell, gridCell, Vector3.zero);
                GameplayManager.Instance.DefeatEnemyChessMan(destroyGO.index);
                GameplayManager.Instance.SetTile(destroyGO.posIndex, TileType.NONE);
                destroyGO.Defeated();
            }
            else if (tile == TileType.PLAYER_CHESS)
            {
                GameplayObject destroyGO = GetChessman(gridCell, gridCell, Vector3.zero);
                GameplayManager.Instance.DefeatPlayerChessMan(destroyGO.index);
                GameplayManager.Instance.SetTile(destroyGO.posIndex, TileType.NONE);
                destroyGO.Defeated();
            }
            else if (tile == TileType.BOX)
            {
                GameplayObject destroyGO = GameUtils.GetGameplayObjectByPosition(gridCell);
                GameplayManager.Instance.UpdateTile(gridCell);
                GameplayManager.Instance.SetTile(destroyGO.posIndex, TileType.NONE);
                destroyGO.Defeated();
            }
            else if (tile == TileType.WATER)
            {
                SoundManager.Instance.PlaySound(AudioPlayer.SoundID.SFX_WATER_SPLASH);
                IsDrop();
            }
        }

        yield return new WaitForSeconds(0.2f);

        if (GameUtils.SnapToGrid(transform.position).y <= destroyPositionY)
        {
            IsDrop();
        }

        SoundManager.Instance.PlaySound(AudioPlayer.SoundID.SFX_CLICK_TILE);
        Instantiate(vfxDrop, GameUtils.SnapToGrid(transform.position), Quaternion.identity);

        isAnim = false;

        SetPosIndex();
    }

    public override void Drop()
    {
        StartCoroutine(Cor_DropAnim(GameUtils.SnapToGrid(transform.position + Vector3.down), Vector3.down));
    }
}
