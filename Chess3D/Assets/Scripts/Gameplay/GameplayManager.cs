using GDC.Enums;
using GDC.Managers;
using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager Instance { get; private set; }

    [SerializeField] private LevelSpawner levelSpawner;
    [SerializeField] private GridStateManager gridSateManager;
    public CameraController camController;
    public UIGameplayManager uiGameplayManager;
    //[SerializeField] TutorialConfig tutorialConfig;

    [SerializeField] private Transform availableMovePrefab;
    private List<Transform> availableMoveTrans = new List<Transform>();

    [HideInInspector] public LevelData levelData;
    [HideInInspector] public ChapterData chapterData;

    [SerializeField, ReadOnly] public List<ChessMan> playerArmy, enemyArmy, listEnemyPriorityLowest;
    [SerializeField, ReadOnly] private List<ChessMan> outlineChessMan;
    [SerializeField, ReadOnly] private List<GameplayObject> outlineGameplayObj;
    [ReadOnly] public int remainTurn;
    [ReadOnly] public bool enemyTurn;
    [HideInInspector] public bool isAnimMoving, isEndTurn, isEndGame;
    private void Awake()
    {
        Instance = this;
    }

    //[Button]
    //private void ResetLevelRef()
    //{
    //    playerArmy.Clear();
    //    enemyArmy.Clear();
    //    listEnemyPriorityLowest.Clear();
    //    outlineChessMan.Clear();
    //    levelData = null;
    //    chapterData = null;
    //    enemyTurn = false;
    //}

    public void LoadLevel(int chapterIndex, int levelIndex)
    {
        SoundManager.Instance.PlayMusic(AudioPlayer.SoundID.GAMEPLAY_1);

        levelSpawner.SpawnLevel(chapterIndex, levelIndex);
        DeepCopyLevelData(levelSpawner.levelData,out levelData);
        chapterData = levelSpawner.GetChapterData(chapterIndex);

        playerArmy = levelSpawner.playerArmy;
        enemyArmy = levelSpawner.enemyArmy;
        SetRemainTurn(levelSpawner.levelData.maxTurn, false);
        camController.Setup(levelSpawner.levelData.center, levelSpawner.levelData.distance);

        ResetEnemyPriorityLowestList();
        
        RenderSettings.skybox = chapterData.skyBox;

        isAnimMoving = false;
        isEndTurn = true;
        enemyTurn = false;
        isEndGame = false;

        uiGameplayManager.Setup();
        gridSateManager.Setup();
        SetPowerUpNum();

        SaveLoadManager.Instance.GameData.SetPlayedLevelBefore(chapterIndex, levelIndex, true);
    }

    private void SetPowerUpNum()
    {
        SaveLoadManager.Instance.GameData.undoNum = 3;
        SaveLoadManager.Instance.GameData.solveNum = 3;
    }
    private void ResetEnemyPriorityLowestList()
    {
        if (listEnemyPriorityLowest == null) listEnemyPriorityLowest = new List<ChessMan>();
        if (enemyArmy.Count > 0)
        {
            //Debug.Log("StartResetPriorityList");
            listEnemyPriorityLowest.Clear();
            foreach (var enemy in enemyArmy)
            {
                //Debug.Log(levelData.GetEnemyArmies()[enemy.index].priority);
                if (levelData.GetEnemyArmies()[enemy.index].priority == levelData.GetEnemyArmies()[enemyArmy[0].index].priority)
                {
                    listEnemyPriorityLowest.Add(enemy);
                }
            }
        }
    }


    private void DeepCopyLevelData(LevelData levelDataSO, out LevelData levelData)
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
    private void SetRemainTurn(int value, bool isSetTurnSlider = true)
    {
        remainTurn = value;
        if (isSetTurnSlider)
            uiGameplayManager.uIInformationPanel.SetUITurn(remainTurn);
    }
    private IEnumerator Cor_EndTurn()
    {
        yield return new WaitUntil(() => isEndTurn);
        ChangeTurn(!enemyTurn);
    }
    private void ChangeTurn()
    {
        isAnimMoving = false;
        StartCoroutine(Cor_EndTurn());
    }
    
    private void ChangeTurn(bool enemyTurn)
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
        uiGameplayManager.ChangeTurn(enemyTurn);
        if (enemyTurn)
        {
            EnemyTurn();
        }
        else
        {
            PlayerTurn();
        }
        
    }
    private void EnemyTurn() //Xac dinh thu tu di chuyen cua cac enemy
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
                        if (enemy.config.chessManType == ChessManType.KING && levelData.GetEnemyArmies()[enemy.index].isAI && enemy.config.CheckMoveIsSafe(enemy.posIndex) == false)
                        {
                            continue;
                        }
                        MakeMove(enemy, move, player);
                        return;
                    }
                }
            }
        }

        if (listEnemyPriorityLowest.Count <= 0)
        {
            ResetEnemyPriorityLowestList();
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

                    //Ko con ai co the di theo mac dinh duoc, cuoi cung thi di bat ki nuoc di nao co the di duoc:
                    foreach(var enemy in enemyArmy)
                    {
                        foreach(var move in enemy.config.Move(enemy.posIndex))
                        {
                            if (CheckMove(enemy.config, enemy.posIndex, move))
                            {
                                MakeMove(enemy, move);
                                Debug.Log(enemy + " Da het duong, enemy di bat ki nuoc di nao co the di duoc");
                                return;
                            }
                        }
                    }

                    //Neu ko co ai co the di duoc het thi skip turn
                    EndTurn();
                }
            }            
        }
        
        //ChangeTurn(false);
    }
    private void PlayerTurn()
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
                    case TileType.BOX:
                        tran.localScale = new Vector3(1, 1, 1);
                        tran.rotation = Quaternion.Euler(90, 0, 0);
                        break;
                    case TileType.SLOPE_0:
                        tran.position += new Vector3(0f, -0.48f, 0.05f);
                        tran.localScale = new Vector3(1, 1.4f, 1);
                        tran.rotation = Quaternion.Euler(45, 180, 0);
                        break;
                    case TileType.SLOPE_90:
                        tran.position += new Vector3(-0.05f, -0.48f, 0f);
                        tran.localScale = new Vector3(1, 1.4f, 1);
                        tran.rotation = Quaternion.Euler(45, 90, 0);
                        Debug.Log(tran.rotation.ToString());
                        break;
                    case TileType.SLOPE_180:
                        tran.position += new Vector3(0f, -0.48f, -0.05f);
                        tran.localScale = new Vector3(1, 1.4f, 1);
                        tran.rotation = Quaternion.Euler(45, 0, 0);
                        break;
                    case TileType.SLOPE_270:
                        tran.position += new Vector3(0.05f, -0.48f, 0f);
                        tran.localScale = new Vector3(1, 1.4f, 1);
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
    private void CheckShowOutlineGameplayObject(Vector3 pos)
    {
        GameplayObject gameplayObj = GameUtils.GetGameplayObjectByPosition(pos);
        if (gameplayObj != null)
        {
            gameplayObj.SetOutline(10, Color.cyan);
            if (outlineGameplayObj == null)
            {
                outlineGameplayObj = new List<GameplayObject>();
            }
            outlineGameplayObj.Add(gameplayObj);
        }
    }
    private void ShowOutlineChessMan(ChessMan chessMan)
    {
        chessMan.SetOutline(10, Color.cyan);
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
    private void HideOutLineAllGameplayObject()
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
        if (chessMan.isEnemy == false) //Nếu là player thì lưu vết để có thể undo nước đi được
        {
            gridSateManager.AddState(levelData.tileInfo, playerArmy, enemyArmy, listEnemyPriorityLowest);
        }

        isAnimMoving = true;
        isEndTurn = false;

        chessMan.Move(posIndexToMove);
        if (defeatedChessMan != null)
        {
            uiGameplayManager.uIChessManPanel.DisableChess(defeatedChessMan);
            if (defeatedChessMan.isEnemy)
            {
                DefeatEnemyChessMan(defeatedChessMan.index);
            }
            else
            {
                DefeatPlayerChessMan(defeatedChessMan.index);
            }
            StartCoroutine(Cor_DefeatedChessMan(chessMan, defeatedChessMan));
        }

        //StartCoroutine(Cor_AfterAnim(chessMan,posIndexToMove));

        if (enemyTurn == false)
        {
            SetRemainTurn(remainTurn - 1);
        }
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
    public void UpdateTile(Vector3 oldPos, TileInfo tileInfo = null)
    {
        levelData.SetTileInfoNoDeep(oldPos, 0, TileType.NONE);
    }
    public void EndTurn() //Duoc goi sau khi ket thuc luot
    {
        isEndTurn = true;
        ChangeTurn();
    }

    public int GetStarOfCurrentLevel()
    {
        if (remainTurn >= levelData.starTurn3) return 3;
        if (remainTurn >= levelData.starTurn2) return 2;
        if (remainTurn >= 0) return 1;
        Debug.LogError("How the heck the star of current level is 0???");
        return 0;
    }
    [Button]
    private void Win()
    {
        Debug.Log("Win");
        isEndGame = true;
        bool isNewRecord = SaveLoadManager.Instance.GameData.CheckNewRecord(chapterData.id, levelData.id, remainTurn);
        StartCoroutine(Cor_ShowWinPanel(isNewRecord));
        int star = GetStarOfCurrentLevel();
        SaveLoadManager.Instance.GameData.SetLevelData(chapterData.id, levelData.id, star, remainTurn);
        SaveLoadManager.Instance.GameData.CheckSetCurrentLevel(chapterData.id, levelData.id);
        SaveLoadManager.Instance.Save();
    }
    private IEnumerator Cor_ShowWinPanel(bool isNewRecord)
    {
        yield return new WaitForSeconds(1);
        uiGameplayManager.ShowWin(isNewRecord);
    }
    void Lose()
    {
        Debug.Log("Lose");
        isEndGame = true;
        StartCoroutine(Cor_ShowLosePanel());
    }
    private IEnumerator Cor_ShowLosePanel()
    {
        yield return new WaitForSeconds(1);
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

    [Button]
    public void Undo()
    {
        if (SaveLoadManager.Instance.GameData.undoNum <=0)
        {
            Debug.Log("Da het undo");
            return;
        }
        SaveLoadManager.Instance.GameData.undoNum--;
        SetRemainTurn(remainTurn + 1);
        gridSateManager.Undo();
    }
}
