using GDC.Enums;
using GDC.Managers;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager Instance { get; private set; }

    [SerializeField] LevelSpawner levelSpawner;
    public CameraController camController;
    [SerializeField] UIGameplayManager uiGameplayManager;
    [SerializeField] TutorialConfig tutorialConfig;

    [SerializeField] Transform availableMovePrefab;
    List<Transform> availableMoveTrans = new List<Transform>();

    [HideInInspector] public LevelData levelData;
    [HideInInspector] public ChapterData chapterData;

    [SerializeField, ReadOnly] public List<ChessMan> playerArmy, enemyArmy;
    [SerializeField, ReadOnly] List<ChessMan> listEnemyPriorityLowest, outlineChessMan;
    [SerializeField, ReadOnly] List<GameplayObject> outlineGameplayObj;
    [ReadOnly] public int remainTurn;
    [ReadOnly] public bool enemyTurn;
    [HideInInspector] public bool isAnimMoving, isEndTurn;

    [Header("Test only")]
    public int levelIndex, chapterIndex;
    private void Awake()
    {
        Instance = this;
    }
    //private void Start()
    //{
    //    LoadLevel(levelName);
    //}
    [Button]
    void ResetLevelRef()
    {
        playerArmy.Clear();
        enemyArmy.Clear();
        listEnemyPriorityLowest.Clear();
        outlineChessMan.Clear();
        levelData = null;
        chapterData = null;
        enemyTurn = false;
    }
    [Button]
    public void LoadLevel()
    {
        LoadLevel(chapterIndex ,levelIndex);
    }
    public void LoadLevel(int chapterIndex, int levelIndex)
    {
        levelSpawner.SpawnLevel(chapterIndex, levelIndex);
        DeepCopyLevelData(levelSpawner.levelData,out levelData);
        chapterData = levelSpawner.GetChapterData(chapterIndex);

        playerArmy = levelSpawner.playerArmy;
        enemyArmy = levelSpawner.enemyArmy;
        SetRemainTurn(levelSpawner.levelData.maxTurn);
        camController.Setup(levelSpawner.levelData.center, levelSpawner.levelData.distance);

        if (listEnemyPriorityLowest == null) listEnemyPriorityLowest = new List<ChessMan>();
        if (enemyArmy.Count > 0)
        {
            listEnemyPriorityLowest.Clear();
            foreach (var enemy in enemyArmy)
            {
                if (levelData.GetEnemyArmies()[enemy.index].priority == levelData.GetEnemyArmies()[0].priority)
                {
                    listEnemyPriorityLowest.Add(enemy);
                }
            }
        }
        RenderSettings.skybox = chapterData.skyBox;

        isAnimMoving = false;
        isEndTurn = true;
        enemyTurn = false;

        uiGameplayManager.Setup();
        CheckShowTutorial();
    }

    void CheckShowTutorial()
    {
        foreach(var tutorialData in tutorialConfig.tutorialDatas)
        {
            if (chapterData.id == tutorialData.chapterIndex && levelData.id == tutorialData.levelIndex)
            {
                uiGameplayManager.ShowTutorial(tutorialData.tutorialSprite);
            }
        }
    }


    void DeepCopyLevelData(LevelData levelDataSO, out LevelData levelData)
    {
        levelData = new LevelData();
        levelData.SetData(levelDataSO.GetTileInfo(), levelDataSO.GetPlayerArmies(), levelDataSO.GetEnemyArmies());
        levelData.starTurn2 = levelDataSO.starTurn2;
        levelData.starTurn3 = levelDataSO.starTurn3;
        levelData.maxTurn = levelDataSO.maxTurn;
        levelData.center = levelDataSO.center;
        levelData.distance = levelDataSO.distance;
        levelData.id = levelDataSO.id;
    }
    void SetRemainTurn(int value)
    {
        remainTurn = value;
    }
    public void ChangeTurn()
    {
        isAnimMoving = false;
        StartCoroutine(Cor_EndTurn());
    }
    IEnumerator Cor_EndTurn()
    {
        yield return new WaitUntil(() => isEndTurn);
        ChangeTurn(!enemyTurn);
    }
    void ChangeTurn(bool enemyTurn)
    {
        if (CheckWin())
        {
            Win();
            return;
        }
        else if (CheckLose())
        {
            Lose();
            return;
        }

        this.enemyTurn = enemyTurn;
        if (enemyTurn)
        {
            EnemyTurn();
        }
        else
        {
            PlayerTurn();
        }
        
    }
    void EnemyTurn()
    {
        Debug.Log("Enemy Turn!");

        foreach (var enemy in enemyArmy)
        {
            foreach (var player in playerArmy)
            {
                foreach (var move in enemy.config.Move(enemy.posIndex))
                {
                    if (GameUtils.CompareVector3(player.posIndex, move))
                    {
                        MakeMove(enemy, move, player);
                        return;
                    }
                }
            }
        }

        if (listEnemyPriorityLowest.Count > 0)
        {
            if (listEnemyPriorityLowest[0].EnemyMove())
            {
                ChessMan temp = listEnemyPriorityLowest[0]; //Thuc hien dua vi tri dau xuong vi tri cuoi sau khi di chuyen xong
                listEnemyPriorityLowest.RemoveAt(0);
                listEnemyPriorityLowest.Add(temp);
            }
            else
            {
                if (enemyArmy.Count > 0)
                {
                    foreach (var enemy in enemyArmy)
                    {
                        if (enemy.EnemyMove()) //trong EnemyMove da thuc hien di chuyen neu true roi
                        {
                            return;
                        }
                    }
                    Vector3 randomMove = enemyArmy[0].config.PatrolState(enemyArmy[0].posIndex);
                    MakeMove(enemyArmy[0], randomMove);
                    Debug.Log(enemyArmy[0].name + " di chuyen random");
                }
            }            
        }
        
        //ChangeTurn(false);
    }
    void PlayerTurn()
    {
        Debug.Log("Player Turn");
        //dosomething
    }
    public void DefeatEnemyChessMan(int enemyIndex)
    {
        foreach(var chessman in listEnemyPriorityLowest)
        {
            if (chessman.index == enemyIndex)
            {
                listEnemyPriorityLowest.Remove(chessman);
                break;
            }
        }
        foreach(var chessman in enemyArmy)
        {
            if (chessman.index == enemyIndex)
            {
                enemyArmy.Remove(chessman);
                break;
            }
        }
    }
    public void DefeatPlayerChessMan(int playerIndex)
    {
        foreach (var chessman in playerArmy)
        {
            if (chessman.index == playerIndex)
            {
                playerArmy.Remove(chessman);
                break;
            }
        }
    }
    public void ShowAvailableMove(ChessManConfig chessManConfig, Vector3 curPosIndex)
    {
        if (chessManConfig == null)
        {
            Debug.LogError("chessman chua co config");
            return;
        }

        HideAvailableMove();
        List<Vector3> moves = chessManConfig.Move(curPosIndex);
        if (moves != null)
        {
            availableMoveTrans.Clear();
            foreach (Vector3 move in moves)
            {
                TileInfo tileInfo = levelData.GetTileInfoNoDeep(move + Vector3.down);
                Transform tran = Instantiate(availableMovePrefab, move, Quaternion.identity);
                switch (tileInfo.tileType)
                {
                    case TileType.GROUND:
                        tran.localScale = new Vector3(1, 1, 1);
                        tran.rotation = Quaternion.Euler(90, 0, 0);
                        break;
                    case TileType.SLOPE_0:
                        tran.position += new Vector3(0f, -0.45f, 0.1f);
                        tran.localScale = new Vector3(1, 1.3f, 1);
                        tran.rotation = Quaternion.Euler(45, 180, 0);
                        break;
                    case TileType.SLOPE_90:
                        tran.position += new Vector3(-0.1f, -0.45f, 0f);
                        tran.localScale = new Vector3(1, 1.3f, 1);
                        tran.rotation = Quaternion.Euler(45, 90, 0);
                        Debug.Log(tran.rotation.ToString());
                        break;
                    case TileType.SLOPE_180:
                        tran.position += new Vector3(0f, -0.45f, -0.1f);
                        tran.localScale = new Vector3(1, 1.3f, 1);
                        tran.rotation = Quaternion.Euler(45, 0, 0);
                        break;
                    case TileType.SLOPE_270:
                        tran.position += new Vector3(0.1f, -0.45f, 0f);
                        tran.localScale = new Vector3(1, 1.3f, 1);
                        tran.rotation = Quaternion.Euler(45, 270, 0);
                        break;
                }
                availableMoveTrans.Add(tran);
                CheckShowOutlineGameplayObject(move);

                foreach(var enemy in enemyArmy)
                {
                    if (enemy.posIndex.Compare(move,1))
                    {
                        ShowOutlineChessMan(enemy);   
                    }
                }
            }

            if (moves.Count == 0)
            {
                Debug.Log("Ko co nuoc di nao co the thuc hien");
            }
        }

        if (moves == null)
        {
            Debug.LogError("move is null");
        }
    }
    void CheckShowOutlineGameplayObject(Vector3 pos)
    {
        TileInfo tileInfo = levelData.GetTileInfoNoDeep(pos);
        if (tileInfo.tileType == TileType.BOX || tileInfo.tileType == TileType.BOULDER)
        {
            Collider[] colls = Physics.OverlapBox(pos, Vector3.one / 5);
            foreach (Collider coll in colls) 
            {
                GameplayObject gameplayObj = coll.transform.GetComponent<GameplayObject>();

                //Debug.Log(gameplayObj);
                if (gameplayObj == null) continue;

                gameplayObj.SetOutline(10, Color.yellow);
                if (outlineGameplayObj == null)
                {
                    outlineGameplayObj = new List<GameplayObject>();
                }
                outlineGameplayObj.Add(gameplayObj);
            }
        }
    }
    void ShowOutlineChessMan(ChessMan chessMan)
    {
        chessMan.SetOutline(10, Color.yellow);
        if (outlineChessMan == null)
        {
            outlineChessMan = new List<ChessMan>();
        }
        outlineChessMan.Add(chessMan);
    }
    public void HideOutLineAllChessMan()
    {
        if (outlineChessMan == null) return;
        foreach (var chessMan in outlineChessMan)
        {
            chessMan.SetOutline(0);
        }
        outlineChessMan.Clear();
    }
    void HideOutLineAllGameplayObject()
    {
        if (outlineGameplayObj == null) return;
        foreach(var obj in outlineGameplayObj)
        {
            obj.SetOutline(0);
        }
        outlineGameplayObj.Clear();
    }
    public void HideAvailableMove()
    {
        if (availableMoveTrans != null)
        {
            foreach (var tran in availableMoveTrans)
            {
                Destroy(tran.gameObject);
            }
            availableMoveTrans.Clear();
        }
        HideOutLineAllGameplayObject();
    }
    public bool CheckMove(ChessManConfig chessManConfig, Vector3 curPosIndex, Vector3 posIndexToMove)
    {
        List<Vector3> moves = chessManConfig.Move(curPosIndex);
        foreach (Vector3 move in moves)
        {
            if (GameUtils.CompareVector3(move, posIndexToMove)) return true;
        }
        return false;
    }    
    public void MakeMove(ChessMan chessMan, Vector3 posIndexToMove, ChessMan defeatedChessMan = null)
    {
        isAnimMoving = true;
        isEndTurn = false;

        chessMan.Move(posIndexToMove);
        if (defeatedChessMan != null)
        {
            StartCoroutine(Cor_DefeatedChessMan(chessMan, defeatedChessMan));
        }

        //StartCoroutine(Cor_AfterAnim(chessMan,posIndexToMove));

        if (enemyTurn == false)
            SetRemainTurn(remainTurn - 1);
    }
    public void UpdateTile(Vector3 oldPos, Vector3 newPos, TileInfo tileInfo = null) //Cap nhat toa do tile oldPos thanh None, va cap nhat tileInfo cho new pos
    {
        if (tileInfo == null)
        {
            levelData.SetTileInfoNoDeep(newPos, 0, TileType.NONE);
        }
        else
        {
            levelData.SetTileInfoNoDeep(newPos, tileInfo.id, tileInfo.tileType);
        }
        levelData.SetTileInfoNoDeep(oldPos, 0, TileType.NONE);
    }
    public void EndTurn() //Duoc goi sau khi ket thuc luot
    {
        isEndTurn = true;
        ChangeTurn();
    }
    //IEnumerator Cor_AfterAnim(ChessMan chessMan, Vector3 posIndexToMove)
    //{
    //    yield return new WaitUntil(() => isAnimMoving == false);
        
    //    //Debug.Log("AfterAnim");
    //    if (chessMan.config.chessManType != ChessManType.KNIGHT)
    //    {
    //        Vector3 direct = new Vector3(posIndexToMove.x - chessMan.posIndex.x, 0, posIndexToMove.z - chessMan.posIndex.z).normalized;
    //        Vector3 towardPos = Vector3.zero;
    //        TileInfo boxTileInfo = null;
    //        foreach (var move in chessMan.config.Move(chessMan.posIndex))
    //        {
    //            Vector3 moveDirect = new Vector3(move.x - chessMan.posIndex.x, 0, move.z - chessMan.posIndex.z).normalized;
    //            //Debug.Log(direct + " " + moveDirect);
    //            if ((int)Mathf.Round(moveDirect.x) != (int)Mathf.Round(direct.x) || (int)Mathf.Round(moveDirect.z) != (int)Mathf.Round(direct.z)) continue;
    //            if (Vector3.Distance(chessMan.posIndex, move) > Vector3.Distance(chessMan.posIndex, posIndexToMove)) continue;

    //            TileInfo tempTileInfo = levelData.GetTileInfoNoDeep((int)Mathf.Round(move.x), (int)Mathf.Round(move.y), (int)Mathf.Round(move.z));
    //            Debug.Log("MOVE " + move + "  " + posIndexToMove + tempTileInfo.tileType);
    //            if (tempTileInfo.tileType == TileType.BOX || tempTileInfo.tileType == TileType.BOULDER)
    //            {
    //                towardPos = move;
    //                boxTileInfo = tempTileInfo;
    //                Debug.Log("Find " + boxTileInfo);
    //                break;
    //            }
    //        }

    //        if (boxTileInfo != null)
    //        {
    //            //TileInfo boxTileInfo = levelData.GetTileInfoNoDeep((int)towardPos.x, (int)towardPos.y, (int)towardPos.z);
    //            TileInfo underBoxTileInfo = levelData.GetTileInfoNoDeep((int)towardPos.x, (int)towardPos.y - 1, (int)towardPos.z);
    //            Vector3 boxPosToMove = posIndexToMove + direct;
    //            while ((int)boxPosToMove.y - 1 > 0 && underBoxTileInfo.tileType == TileType.NONE ||
    //                underBoxTileInfo.tileType == TileType.ENEMY_CHESS || underBoxTileInfo.tileType == TileType.PLAYER_CHESS) //box/boulder roi xuong
    //            {
    //                levelData.SetTileInfoNoDeep((int)boxPosToMove.x, (int)boxPosToMove.y, (int)boxPosToMove.z, 0, TileType.NONE);
    //                boxPosToMove.y--;
    //                underBoxTileInfo = levelData.GetTileInfoNoDeep((int)towardPos.x, (int)towardPos.y - 1, (int)towardPos.z);
    //            }

    //            while ((int)boxPosToMove.y - 1 > 0 && underBoxTileInfo.tileType == TileType.WATER) // doi voi Boulder thi tiep tuc chim xuong nuoc
    //            {
    //                boxPosToMove.y--;
    //                underBoxTileInfo = levelData.GetTileInfoNoDeep((int)towardPos.x, (int)towardPos.y - 1, (int)towardPos.z);
    //            }

    //            if (boxTileInfo.tileType == TileType.BOULDER) //Doi voi boulder thi lan tren mat phang nghieng (ko co xet dang o trong nuoc)
    //            {
    //                while ((int)boxPosToMove.y - 1 > 0 && levelData.GetTileInfoNoDeep((int)boxPosToMove.x, (int)boxPosToMove.y - 1, (int)boxPosToMove.z).tileType == TileType.SLOPE_0)
    //                {
    //                    levelData.SetTileInfoNoDeep((int)boxPosToMove.x, (int)boxPosToMove.y, (int)boxPosToMove.z, 0, TileType.NONE);
    //                    boxPosToMove.y--;
    //                    boxPosToMove.z++;
    //                }
    //                while ((int)boxPosToMove.y - 1 > 0 && levelData.GetTileInfoNoDeep((int)boxPosToMove.x, (int)boxPosToMove.y - 1, (int)boxPosToMove.z).tileType == TileType.SLOPE_90)
    //                {
    //                    levelData.SetTileInfoNoDeep((int)boxPosToMove.x, (int)boxPosToMove.y, (int)boxPosToMove.z, 0, TileType.NONE);
    //                    boxPosToMove.y--;
    //                    boxPosToMove.x--;
    //                }
    //                while ((int)boxPosToMove.y - 1 > 0 && levelData.GetTileInfoNoDeep((int)boxPosToMove.x, (int)boxPosToMove.y - 1, (int)boxPosToMove.z).tileType == TileType.SLOPE_180)
    //                {
    //                    levelData.SetTileInfoNoDeep((int)boxPosToMove.x, (int)boxPosToMove.y, (int)boxPosToMove.z, 0, TileType.NONE);
    //                    boxPosToMove.y--;
    //                    boxPosToMove.z--;
    //                }
    //                while ((int)boxPosToMove.y - 1 > 0 && levelData.GetTileInfoNoDeep((int)boxPosToMove.x, (int)boxPosToMove.y - 1, (int)boxPosToMove.z).tileType == TileType.SLOPE_270)
    //                {
    //                    levelData.SetTileInfoNoDeep((int)boxPosToMove.x, (int)boxPosToMove.y, (int)boxPosToMove.z, 0, TileType.NONE);
    //                    boxPosToMove.y--;
    //                    boxPosToMove.x++;
    //                }
    //            }

    //            if ((int)boxPosToMove.x >= 0 && (int)boxPosToMove.y > 0 && (int)boxPosToMove.z >= 0) //box/boulder chua roi ra khoi map
    //            {
    //                levelData.SetTileInfoNoDeep((int)boxPosToMove.x, (int)boxPosToMove.y, (int)boxPosToMove.z, boxTileInfo.id, boxTileInfo.tileType);
    //                levelData.SetTileInfoNoDeep((int)towardPos.x, (int)towardPos.y, (int)towardPos.z, 0, TileType.NONE);
    //            }

    //            //levelData.SetTileInfoNoDeep((int)chessMan.posIndex.x, (int)chessMan.posIndex.y, (int)chessMan.posIndex.z, 0, TileType.NONE);
    //        }
    //        //}
    //    }
    //    TileInfo curTileInfo = levelData.GetTileInfoNoDeep((int)chessMan.posIndex.x, (int)chessMan.posIndex.y, (int)chessMan.posIndex.z);
    //    levelData.SetTileInfoNoDeep((int)posIndexToMove.x, (int)posIndexToMove.y, (int)posIndexToMove.z, curTileInfo.id, curTileInfo.tileType);
    //    levelData.SetTileInfoNoDeep((int)chessMan.posIndex.x, (int)chessMan.posIndex.y, (int)chessMan.posIndex.z, 0, TileType.NONE);
    //    chessMan.posIndex = posIndexToMove;
    //    isEndTurn = true;
    //}

    public int GetStarOfCurrentLevel()
    {
        if (remainTurn == 0) return 0;
        if (remainTurn < levelData.starTurn2) return 1;
        if (remainTurn < levelData.starTurn3) return 2;
        return 3;
    }
    void Win()
    {
        Debug.Log("Win");
        SaveLoadManager.Instance.GameData.SetLevelData(chapterData.id, levelData.id, GetStarOfCurrentLevel(), remainTurn);
        uiGameplayManager.ShowWin();
    }
    void Lose()
    {
        Debug.Log("Lose");
        uiGameplayManager.ShowLose();
    }
    bool CheckLose()
    {
        bool isHaveKing = false;
        foreach(var chess in playerArmy)
        {
            if (chess.config.chessManType == ChessManType.KING)
            {
                isHaveKing = true;
                break;
            }
        }
        if (playerArmy.Count == 0 || isHaveKing == false || remainTurn <=0) return true;
        return false;
    }
    bool CheckWin()
    {
        bool isHaveKing = false;
        foreach (var chess in enemyArmy)
        {
            if (chess.config.chessManType == ChessManType.KING)
            {
                isHaveKing = true;
                break;
            }
        }
        if (isHaveKing == false) return true;
        return false;
    }
    IEnumerator Cor_DefeatedChessMan(ChessMan defeatChessMan, ChessMan defeatedChessMan)
    {
        yield return new WaitUntil(() => Vector3.Distance(defeatChessMan.transform.position, defeatedChessMan.transform.position) < 1);
        defeatedChessMan.Defeated();
    }
    [SerializeField] Vector3 logTestPos;
    [Button]
    void LogTileInfo()
    {
        TileInfo tileInfo = levelData.GetTileInfoNoDeep(logTestPos);
        Debug.Log(tileInfo.tileType);
    }
}
