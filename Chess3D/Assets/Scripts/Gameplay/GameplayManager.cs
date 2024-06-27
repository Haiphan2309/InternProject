using GDC.Enums;
using GDC.Managers;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager Instance { get; private set; }

    [SerializeField] LevelSpawner levelSpawner;
    [ReadOnly] public bool enemyTurn;
    public LevelData levelData;
    [SerializeField] Transform availableMovePrefab;
    List<Transform> availableMoveTrans = new List<Transform>();

    [SerializeField, ReadOnly] List<ChessMan> playerArmy, enemyArmy, listEnemyPriorityLowest; 
    private void Awake()
    {
        Instance = this;
    }

    [Button]
    public void LoadLevel()
    {
        levelSpawner.Setup();
        levelSpawner.SpawnLevel();
        DeepCopyLevelData(levelSpawner.levelData,out levelData);
        levelData = levelSpawner.levelData;
        playerArmy = levelSpawner.playerArmy;
        enemyArmy = levelSpawner.enemyArmy;
    }

    void DeepCopyLevelData(LevelData levelDataSO, out LevelData levelData)
    {
        levelData = new LevelData();
        levelData.SetData(levelDataSO.GetTileInfo(), levelDataSO.GetPlayerArmies(), levelDataSO.GetEnemyArmies());
    }

    public void ChangeTurn()
    {
        ChangeTurn(!enemyTurn);
    }
    public void ChangeTurn(bool enemyTurn)
    {
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
        //dosomething
        Debug.Log("Enemy Turn!");

        foreach (var enemy in enemyArmy)
        {
            foreach (var player in playerArmy)
            {
                foreach (var move in enemy.config.Move(enemy.posIndex))
                {
                    if ((int)Mathf.Round(move.x) == (int)Mathf.Round(player.posIndex.x)
                        && (int)Mathf.Round(move.y) == (int)Mathf.Round(player.posIndex.y)
                        && (int)Mathf.Round(move.z) == (int)Mathf.Round(player.posIndex.z))
                    {
                        MakeMove(enemy, move, player);
                        return;
                    }
                }
            }
        }

        if (listEnemyPriorityLowest == null || listEnemyPriorityLowest.Count == 0)
        {
            listEnemyPriorityLowest = new List<ChessMan>();
            if (enemyArmy.Count > 0)
            {
                foreach (var enemy in enemyArmy)
                {
                    if (levelData.GetEnemyArmies()[enemy.index].priority == levelData.GetEnemyArmies()[0].priority)
                    {
                        listEnemyPriorityLowest.Add(enemy);
                    }
                }
            }
            else
            {
                Debug.LogError("Da het quan dich");
            }
        }

        if (listEnemyPriorityLowest.Count > 0)
        {
            listEnemyPriorityLowest[0].EnemyMove();
            ChessMan temp = listEnemyPriorityLowest[0]; //Thuc hien dua vi tri dau xuong vi tri cuoi sau khi di chuyen xong
            listEnemyPriorityLowest.RemoveAt(0);
            listEnemyPriorityLowest.Add(temp);
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
        enemyArmy.RemoveAt(enemyIndex);
    }
    public void DefeatPlayerChessMan(int playerIndex)
    {
        playerArmy.RemoveAt(playerIndex);
    }
    public void ShowAvailableMove(ChessManConfig chessManConfig, Vector3 curPosIndex)
    {
        if (chessManConfig == null)
        {
            Debug.LogError("chessman chua co config");
            return;
        }
        List<Vector3> moves = chessManConfig.Move(curPosIndex);
        if (moves != null)
        {
            availableMoveTrans.Clear();
            foreach (Vector3 move in moves)
            {
                
                TileInfo tileInfo = levelData.GetTileInfo()[(int)Mathf.Round(move.x), (int)Mathf.Round(move.y)-1, (int)Mathf.Round(move.z)];
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
    }
    public bool CheckMove(ChessManConfig chessManConfig, Vector3 curPosIndex, Vector3 posIndexToMove)
    {
        List<Vector3> moves = chessManConfig.Move(curPosIndex);
        foreach (Vector3 move in moves)
        {
            //if (Vector3Int.FloorToInt(move) == Vector3Int.FloorToInt(posIndexToMove)) return true;
            if ((int)Mathf.Round(move.x) == (int)Mathf.Round(posIndexToMove.x) 
                && (int)Mathf.Round(move.y) == (int)Mathf.Round(posIndexToMove.y) 
                && (int)Mathf.Round(move.z) == (int)Mathf.Round(posIndexToMove.z)) return true;
        }
        return false;
    }    
    public void MakeMove(ChessMan chessMan, Vector3 posIndexToMove, ChessMan defeatedChessMan = null)
    {
        chessMan.Move(posIndexToMove);
        levelData.GetTileInfo()[(int)Mathf.Round(chessMan.posIndex.x), (int)Mathf.Round(chessMan.posIndex.y), (int)Mathf.Round(chessMan.posIndex.z)] = new TileInfo();
    }
    IEnumerator Cor_DefeatedChessMan(ChessMan defeatChessMan, ChessMan defeatedChessMan)
    {
        yield return new WaitUntil(() => Vector3.Distance(defeatChessMan.transform.position, defeatedChessMan.transform.position) < 1);
        defeatedChessMan.Defeated();
    }
}
