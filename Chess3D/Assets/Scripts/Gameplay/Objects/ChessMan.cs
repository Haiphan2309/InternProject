using DG.Tweening;
using GDC;
using GDC.Enums;
using GDC.Managers;
using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class ChessMan : GameplayObject
{
    public ChessManConfig config;

    [SerializeField] LayerMask groundLayerMask;
    public ChessManType testPromoteType;

    [SerializeField] private GameObject vfxPromote;

    public bool isEnemy;
    //public int index;
    public int moveIndex; //Dung de xac dinh index cua nuoc di ke tiep, danh rieng cho enemy

    public int deltaMoveIndex = 1; //Biến này dùng để xác định enemy di chuyển theo chiều tới hoặc chiều lùi theo pattern (1 là tới, -1 là lùi)
    public bool isAI;
    public bool isDrop;

    List<Vector3> showPath = new List<Vector3>();

    public void Setup(PlayerArmy playerArmy, int index, Vector3 posIndex)
    {
        isEnemy = false;
        this.index = index;
        this.posIndex = posIndex;

        testPromoteType = config.chessManType;
        parentObject = transform.parent.gameObject;
    }
    public void Setup(EnemyArmy enemyArmy, int index, Vector3 posIndex)
    {
        isEnemy = true;
        this.index = index;
        this.posIndex = posIndex;

        parentObject = transform.parent.gameObject;
    }
    public void SetChessManData(PlayerChessManData chessManData, Transform parentTransform = null)
    {
        if (config.chessManType != chessManData.chessManType)
        {
            config = GetConfigFromType(chessManData.chessManType);
            testPromoteType = chessManData.chessManType;
            ChangeMesh(chessManData.chessManType);
        }
        
        index = chessManData.index;
        posIndex = chessManData.posIndex;
        isEnemy = false;

        SetParentObject(parentTransform.gameObject);
        SetParentDefault();

        // transform.parent = null;

        //transform.position = posIndex;  //Chỗ này cần là 1 hàm để check player đứng ở vị trí slope hay phẳng
        AjustPosToGround(posIndex);
        CheckBox(posIndex);
    }
    public void SetChessManData(EnemyChessManData chessManData, Transform parentTransform = null)
    {
        config = GetConfigFromType(chessManData.chessManType);
        index = chessManData.index;
        posIndex = chessManData.posIndex;
        moveIndex = chessManData.moveIndex;
        deltaMoveIndex = chessManData.deltaMoveIndex;
        isEnemy = true;
        isAI = chessManData.isAI;

        SetParentObject(parentTransform.gameObject);
        SetParentDefault();

        // transform.parent = null;
        //transform.position = posIndex;  //Chỗ này cần là 1 hàm để check player đứng ở vị trí slope hay phẳng
        AjustPosToGround(posIndex);
        CheckBox(posIndex);
    }

    public bool EnemyMove()
    {
        EnemyArmy enemy = GameplayManager.Instance.levelData.GetEnemyArmies()[index];
        if (enemy.isAI)
        {
            Vector3 posIndexToMove;
            if (config.chessManType != ChessManType.KING)
            {
                posIndexToMove = config.MoveByDefault(posIndex);
                // Check if it's unmovable to prevent softlocks
                if (posIndexToMove == posIndex)
                {
                    return false;
                }
            }
            else //Neu la king thi chay tron
            {
                posIndexToMove = config.RetreatMove(posIndex);
            }

            GameplayObject obj = GameUtils.GetGameplayObjectByPosition(posIndexToMove);
            ChessMan playerChessManDefeated = null;
            if (obj != null)
            {
                playerChessManDefeated = obj.GetComponent<ChessMan>();
            }
            GameplayManager.Instance.MakeMove(this, posIndexToMove, playerChessManDefeated);
        }
        else
        {
            List<Vector3> moves = enemy.movePosIndexs;
            if (moves.Count == 0)
            {
                Debug.Log(gameObject.name + " khong co nuoc di mac dinh nao ca! -> skip");
                return false;
            }

            Vector3 intendedMove = moves[moveIndex];
            if (GameplayManager.Instance.CheckMove(config, posIndex, intendedMove) == false)
            {
                deltaMoveIndex = -deltaMoveIndex; //Dao nguoc chieu di chuyen lai
                int backMoveIndex = moveIndex + deltaMoveIndex*2; //x2 để ko tính cái ô mà mình đang đứng mà là tính cái ô trước đó theo default move
                if (backMoveIndex == -1) backMoveIndex = moves.Count - 1;
                else backMoveIndex = moves.Count - 2; //luc nay backMoveIndex = -2
                backMoveIndex = backMoveIndex % moves.Count;

                //if (config.chessManType == ChessManType.CASTLE)
                //{
                //    Debug.Log("BACKMOVEINDEX: " + backMoveIndex);
                //}

                intendedMove = moves[backMoveIndex];
                //Debug.Log(intendedMove);
                
                if (GameplayManager.Instance.CheckMove(config, posIndex, intendedMove) == false)
                {
                    deltaMoveIndex = -deltaMoveIndex;
                    return false;
                }
                moveIndex = backMoveIndex;
            }
            GameplayManager.Instance.MakeMove(this, moves[moveIndex]);

            moveIndex = moveIndex + deltaMoveIndex;
            if (moveIndex < 0) moveIndex = moves.Count - 1;
            moveIndex = moveIndex % moves.Count;
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
    }
    void KnightMoveAnim(Vector3 posIndexToMove)
    {
        StartCoroutine(Cor_KnightMoveAnim(posIndexToMove));
    }

    IEnumerator Cor_KnightMoveAnim(Vector3 target)
    {
        isMove = true;
        SetParentDefault();
        Vector3 direction = (target - transform.position).normalized;
        targetPosition = target;
        //transform.rotation = Quaternion.Euler(Vector3.zero);
        transform.DORotate(Vector3.zero, 0.3f);
        RotateToDirection(direction);

        yield return new WaitForSeconds(0.5f);

        transform.DOJump(target, 3, 1, 1).SetEase(Ease.InOutSine).OnComplete(() =>
        {
            AjustPosToGround(target);
            isStandOnSlope = isOnSlope;
            isMove = false;
            SetPosIndex();
            CheckBox(target);
            GameplayManager.Instance.EndTurn();
        });
    }

    void OtherMoveAnim(Vector3 posIndexToMove)
    {
        StartCoroutine(Cor_OtherMoveAnim(posIndexToMove));
    }

    IEnumerator Cor_OtherMoveAnim(Vector3 target)
    {
        isMove = true;
        // Unset Parent for chess piece
        SetParentDefault();
        target = GameUtils.SnapToGrid(target);

        // First Pos + Target Pos
        Debug.Log("CHESSMAN Position: " + posIndex + " Target: " + target);

        // // Store current position and current index
        Vector3 currPos = transform.position;
        Vector3 currIdx = posIndex;

        Vector3 direction = (target - currIdx).normalized;

        Vector3 storeBoxPos = Vector3.zero;

        // Rotate to target
        RotateToDirection(direction);
        yield return new WaitForSeconds(0.5f);

        // Calculate Path from First Pos to Target Pos
        List<Vector3> path = CalculatePath(currIdx, target);
        showPath = path;
        targetPosition = target;

        Vector3 gameplayObjectPosition = Vector3.zero;
        GameplayObject gameplayObject = null;
        GameplayObject objectInteract = null;

        // Move
        foreach (var gridCell in path)
        {
            gameplayObjectPosition = GameUtils.SnapToGrid(gridCell);

            gameplayObject = GameUtils.GetGameplayObjectByPosition(gameplayObjectPosition);

            if (gameplayObject != null && gameplayObject.CompareTag("Object")) objectInteract = gameplayObject;

            Vector3 boxDirection = direction;
            boxDirection.y = 0;

            if (gameplayObject != null && !gameplayObject.isAnim)
            {
                gameplayObject.MoveAnim(target, boxDirection, 5f * Time.deltaTime);

                yield return null;

                //yield return new WaitForSeconds(0.5f);
            }
            

            while (currPos != gridCell)
            {
                AjustPosToGround(transform.position, gridCell, direction, true);

                if (!isOnSlope) currPos = transform.position;
                else currPos = transform.position + Vector3.up * 0.4f;

                yield return null;
            }
        }

        yield return null;

        if (objectInteract != null && objectInteract.CompareTag("Object"))
        {
            Debug.Log("Object: " + objectInteract.name + " GameplayObject isAnim: " + objectInteract.isAnim);
            yield return new WaitUntil(() => objectInteract.isAnim == false);
            Debug.Log("GameplayObject isAnim: " + objectInteract.isAnim);
            objectInteract.SetPosIndex();
        }
        isMove = false;

        isStandOnSlope = isOnSlope;

        SetPosIndex();
        
        CheckBox(target);

        Debug.Log(GameUtils.GetTile(posIndex));

        GameplayManager.Instance.CheckActiveButtonObjects();

        StartCoroutine(CheckPromote());
        
    }

    private IEnumerator CheckPromote()
    {
        GameObject promoteGround = GameUtils.GetObjectByPosition(GameUtils.SnapToGrid(transform.position) + Vector3.down);
        if (config.chessManType == ChessManType.PAWN && promoteGround.name == "150(Clone)")
        {
            if (!GameplayManager.Instance.isShowHint)
            {
                UIGameplayManager.Instance.ShowPromote();
                while (testPromoteType == ChessManType.PAWN)
                {
                    testPromoteType = UIGameplayManager.Instance.GetPromoteType();
                    yield return null;
                }
            }
            
            else
            {
                testPromoteType = GameplayManager.Instance.GetPromoteHint();
            }

            Promote(testPromoteType);
            GameplayManager.Instance.uiGameplayManager.UpdateHolder(this);
        }
        if (!isDrop) GameplayManager.Instance.EndTurn();
        isDrop = false;
    }

    void RotateToDirection(Vector3 direction)
    {
        Transform childObject = transform.GetChild(0);

        Quaternion targetRotation = Quaternion.FromToRotation(Vector3.forward, direction);
        Vector3 eulerAngles = childObject.rotation.eulerAngles;

        childObject.DOLocalRotate(Vector3.up * targetRotation.eulerAngles.y, 0.3f);
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


    public void Promote(ChessManType chessManType)
    {
        // Change Config
        ChessManConfig newConfig = GetConfigFromType(chessManType);
        if (newConfig == null)
        {
            Debug.LogError("Fail to load new CONFIG in PROMOTE");
            return;
        }
        config = newConfig;

        // Change Mesh

        ChangeMesh(chessManType);

        //
        GameplayManager.Instance.HideAvailableMove();
        SoundManager.Instance.PlaySound(AudioPlayer.SoundID.SFX_PHONG);
        Instantiate(vfxPromote, transform.position, Quaternion.identity);
    }
    private ChessManConfig GetConfigFromType(ChessManType type)
    {
        string path = "ScriptableObjects/ChessMan/";

        switch (type)
        {
            case ChessManType.PAWN:
                path += "PawnConfig";
                break;
            case ChessManType.CASTLE:
                path += "CastleConfig";
                break;
            case ChessManType.BISHOP:
                path += "BishopConfig";
                break;
            case ChessManType.KNIGHT:
                path += "KnightConfig";
                break;
            case ChessManType.QUEEN:
                path += "QueenConfig";
                break;
            case ChessManType.KING:
                path += "KingConfig";
                break;
            default:
                break;
        }
        // Assets/Resources/ScriptableObjects/ChessMan/KnightConfig.asset

        return Resources.Load<ChessManConfig>(path);
    }

    private Mesh GetMeshFromType(ChessManType type)
    {
        string path = "Materials/ChessMesh/"; //.fbs
        switch (type)
        {
            case ChessManType.PAWN:
                path += "pawn";
                break;
            case ChessManType.CASTLE:
                path += "rook";
                break;
            case ChessManType.BISHOP:
                path += "bishop";
                break;
            case ChessManType.KNIGHT:
                path += "knight";
                break;
            case ChessManType.QUEEN:
                path += "queen";
                break;
            case ChessManType.KING:
                path += "king";
                break;
            default:
                break;
        }
        return Resources.Load<Mesh>(path);
    }
    private void ChangeMesh(ChessManType type)
    {
        Mesh newMesh = GetMeshFromType(type);
        if (newMesh == null)
        {
            Debug.LogError("Fail to load new MESH in PROMOTE");
            return;
        }
        gameObject.GetComponentInChildren<MeshFilter>().mesh = newMesh;
        outline.LoadSmoothNormals();
    }

    public override void Drop()
    {
        Vector3 currentPos = GameUtils.SnapToGrid(posIndex);

        TileType tileType = GameUtils.GetTileBelowObject(currentPos);

        while (tileType == TileType.NONE)
        {
            currentPos.y--;

            if (currentPos.y <= -3)
            {
                break;
            }

            tileType = GameUtils.GetTileBelowObject(currentPos);
        }

        StartCoroutine(Cor_DropAnim(currentPos));
    }

    private IEnumerator Cor_DropAnim(Vector3 target)
    {
        isMove = true;
        isDrop = true;
        // Unset Parent for chess piece
        SetParentDefault();
        target = GameUtils.SnapToGrid(target);

        // Store current position and current index
        Vector3 currPos = transform.position;
        Vector3 currIdx = posIndex;

        Vector3 direction = (target - currIdx).normalized;


        // Calculate Path from First Pos to Target Pos
        List<Vector3> path = CalculatePath(currIdx, target);

        targetPosition = target;

        // Move
        foreach (var gridCell in path)
        {
            while (currPos != gridCell)
            {
                AjustPosToGround(transform.position, gridCell, direction, true);

                if (!isOnSlope) currPos = transform.position;
                else currPos = transform.position + Vector3.up * 0.4f;

                yield return null;
            }
        }

        yield return null;
        isMove = false;

        isStandOnSlope = isOnSlope;

        SetPosIndex();

        CheckBox(target);

        GameplayManager.Instance.CheckActiveButtonObjects();

        StartCoroutine(CheckPromote());
    }

#if UNITY_EDITOR
    [Button]
    void TestPromote()
    {
        Promote(testPromoteType);
    }
    [Button]
    void ShowMoveAvaiList()
    {
        foreach(var move in config.Move(posIndex))
        {
            Debug.Log("Avai " + move);
        }
    }

    [Button]
    void ShowPath()
    {
        foreach (var path in showPath)
        {
            Debug.Log("Path: " + path);
        }
    }
#endif
}
