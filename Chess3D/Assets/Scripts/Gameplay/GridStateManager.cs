using GDC.Enums;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

public class GameplayObjectData
{
    public Vector3 posIndex;
    public int index;
    public string objName;

    public GameplayObjectData()
    {

    }
    public GameplayObjectData(GameplayObject gameplayObject)
    {
        posIndex = gameplayObject.posIndex;
        index = gameplayObject.gameObject.GetInstanceID();
        if (gameplayObject.name.Contains("200")) //Day la box
        {
            objName = "200";
        }
        else //Day la boulder
        {
            objName = "201";
        }
        
    }
}
public class ChessManData : GameplayObjectData
{
    public ChessManType chessManType;
    public ChessManData()
    {

    }
    public ChessManData(ChessMan chessMan)
    {
        posIndex = chessMan.posIndex;
        chessManType = chessMan.config.chessManType;
        index = chessMan.index;
    }
}
public class PlayerChessManData : ChessManData
{
    public PlayerChessManData()
    {

    }
    public PlayerChessManData(ChessMan chessMan)
    {
        posIndex = chessMan.posIndex;
        chessManType = chessMan.config.chessManType;
        index = chessMan.index;
    }
}
public class EnemyChessManData : ChessManData
{
    public int moveIndex;
    public int deltaMoveIndex;
    public bool isAI;
    public EnemyChessManData()
    {

    }
    public EnemyChessManData(ChessMan chessMan)
    {
        posIndex = chessMan.posIndex;
        moveIndex = chessMan.moveIndex;
        deltaMoveIndex = chessMan.deltaMoveIndex;
        chessManType = chessMan.config.chessManType;
        index = chessMan.index;
        isAI = chessMan.isAI;
    }
}
public class GridState
{
    public List<TileInfo> tileInfos;
    public List<PlayerChessManData> playerChessManDatas;
    public List<EnemyChessManData> enemyChessManDatas, enemyChessManDataPrioritys;
    public List<GameplayObjectData> gameplayObjectDatas;

    public GridState(List<TileInfo> tileInfos, List<ChessMan> playerArmy, List<ChessMan> enemyArmy, List<ChessMan> listEnemyPriorityLowest)
    {
        this.tileInfos = new List<TileInfo>();
        foreach (TileInfo tileInfo in tileInfos)
        {
            this.tileInfos.Add(new TileInfo(tileInfo.id, tileInfo.tileType));
        }

        this.playerChessManDatas = new List<PlayerChessManData>();
        foreach(ChessMan chessMan in playerArmy)
        {
            this.playerChessManDatas.Add(new PlayerChessManData(chessMan));
        }

        this.enemyChessManDatas = new List<EnemyChessManData>();
        foreach (ChessMan chessMan in enemyArmy)
        {
            this.enemyChessManDatas.Add(new EnemyChessManData(chessMan));
        }

        this.enemyChessManDataPrioritys = new List<EnemyChessManData>();
        foreach (ChessMan chessMan in listEnemyPriorityLowest)
        {
            this.enemyChessManDataPrioritys.Add(new EnemyChessManData(chessMan));
        }

        this.gameplayObjectDatas = new List<GameplayObjectData>();
        List<GameplayObject> objs = GameObject.FindObjectsOfType<GameplayObject>().OfType<GameplayObject>().ToList();
        List<GameplayObject> gameplayObjs = objs.FindAll(x => x.CompareTag("Object"));

        foreach (var gameplayObj in gameplayObjs)
        {
            this.gameplayObjectDatas.Add(new GameplayObjectData(gameplayObj));
        }
    }
}
public class GridStateManager : MonoBehaviour
{
    private Stack<GridState> gridStateStack;

    public void Setup()
    {
        gridStateStack = new Stack<GridState>();
    }
    public void AddState(List<TileData> tileDatas, List<ChessMan> playerArmy, List<ChessMan> enemyArmy, List<ChessMan> listEnemyPriorityLowest)
    {
        List<TileInfo> tileInfos = new List<TileInfo>();
        foreach(var tileData in tileDatas)
        {
            tileInfos.Add(tileData.tileInfo);
        }
        GridState newGridState = new GridState(tileInfos, playerArmy, enemyArmy, listEnemyPriorityLowest);
        gridStateStack.Push(newGridState);
    }
    public bool CheckCanUndo()
    {
        if (gridStateStack.Count <= 0)
        {
            Debug.Log("Da het nuoc di de undo");
            return false;
        }
        return true;
    }
    public void Undo()
    {
        if (CheckCanUndo() == false)
        {
            return;
        }
        GridState gridState = gridStateStack.Pop();      

        LevelData levelData = GameplayManager.Instance.levelData;
        for (int i = 0; i < gridState.tileInfos.Count; i++)
        {
            levelData.tileInfo[i].tileInfo = gridState.tileInfos[i];
        }

        

        List<GameplayObject> objs = FindObjectsOfType<GameplayObject>().OfType<GameplayObject>().ToList();
        List<GameplayObject> gameplayObjs = objs.FindAll(x => x.CompareTag("Object"));
        //List<GameplayObject> gameplayObjs = GameObject.FindGameObjectsWithTag("Object").OfType<GameplayObject>().ToList();
        //Debug.Log("So box find " + gameplayObjs.Count + " va so box data " + gridState.gameplayObjectDatas.Count.ToString());
        foreach (var gameplayObjData in gridState.gameplayObjectDatas)
        {
            bool isFind = false;
            foreach (var gameplayObj in gameplayObjs)
            {
                if (gameplayObj.gameObject.GetInstanceID() == gameplayObjData.index)
                {
                    gameplayObj.SetGameplayObjectData(gameplayObjData);
                    isFind = true;
                    break;
                }
            }

            if (isFind == false) //Gameplay này đã bị hủy, cần phải sinh ra lại
            {
                GameplayObject obj = SpawnGameplayObject(gameplayObjData.objName);
                obj.SetGameplayObjectData(gameplayObjData);
            }
        }

        foreach (var chessManData in gridState.playerChessManDatas)
        {
            bool isFind = false;
            foreach (var gameplayPlayer in GameplayManager.Instance.playerArmy)
            {
                if (gameplayPlayer.index == chessManData.index)
                {
                    gameplayPlayer.SetChessManData(chessManData);
                    isFind = true;
                    break;
                }
            }

            if (isFind == false) //ChessMan này đã bị hủy, cần phải sinh ra lại
            {
                ChessMan chessMan = SpawnChessMan(chessManData.chessManType, false);
                chessMan.SetChessManData(chessManData);
                GameplayManager.Instance.playerArmy.Add(chessMan);
            }
        }

        foreach (var enemy in GameplayManager.Instance.enemyArmy)
        {
            Destroy(enemy.gameObject);
        }
        GameplayManager.Instance.enemyArmy.Clear();
        foreach (var chessManData in gridState.enemyChessManDatas)
        {
            ChessMan chessMan = SpawnChessMan(chessManData.chessManType, true);
            chessMan.SetChessManData(chessManData);
            GameplayManager.Instance.enemyArmy.Add(chessMan);
        }

        GameplayManager.Instance.listEnemyPriorityLowest.Clear();
        foreach (var chessManData in gridState.enemyChessManDataPrioritys)
        {
            foreach (var enemy in GameplayManager.Instance.enemyArmy)
            {
                if (enemy.index == chessManData.index)
                {
                    GameplayManager.Instance.listEnemyPriorityLowest.Add(enemy);
                    break;
                }
            }
        }
    }
    ChessMan SpawnChessMan(ChessManType chessManType, bool isEnemy, bool isAI = false)
    {
        GameObject chessManObj = null;
        switch (chessManType)
        {
            case ChessManType.PAWN:
                if (isEnemy)
                    if (isAI)
                        chessManObj = Resources.Load<GameObject>("ChessManPrefabs/406");
                    else
                        chessManObj = Resources.Load<GameObject>("ChessManPrefabs/400");
                else
                    chessManObj = Resources.Load<GameObject>("ChessManPrefabs/300");
                break;
            case ChessManType.CASTLE:
                if (isEnemy)
                    if (isAI)
                        chessManObj = Resources.Load<GameObject>("ChessManPrefabs/407");
                    else
                        chessManObj = Resources.Load<GameObject>("ChessManPrefabs/401");
                else
                    chessManObj = Resources.Load<GameObject>("ChessManPrefabs/301");
                break;
            case ChessManType.KNIGHT:
                if (isEnemy)
                    if (isAI)
                        chessManObj = Resources.Load<GameObject>("ChessManPrefabs/408");
                    else
                        chessManObj = Resources.Load<GameObject>("ChessManPrefabs/402");
                else
                    chessManObj = Resources.Load<GameObject>("ChessManPrefabs/302");
                break;
            case ChessManType.BISHOP:
                if (isEnemy)
                    if (isAI)
                        chessManObj = Resources.Load<GameObject>("ChessManPrefabs/409");
                    else
                        chessManObj = Resources.Load<GameObject>("ChessManPrefabs/403");
                else
                    chessManObj = Resources.Load<GameObject>("ChessManPrefabs/303");
                break;
            case ChessManType.QUEEN:
                if (isEnemy)
                    if (isAI)
                        chessManObj = Resources.Load<GameObject>("ChessManPrefabs/410");
                    else
                        chessManObj = Resources.Load<GameObject>("ChessManPrefabs/404");
                else
                    chessManObj = Resources.Load<GameObject>("ChessManPrefabs/304");
                break;
            case ChessManType.KING:
                if (isEnemy)
                    if (isAI)
                        chessManObj = Resources.Load<GameObject>("ChessManPrefabs/411");
                    else
                        chessManObj = Resources.Load<GameObject>("ChessManPrefabs/405");
                else
                    chessManObj = Resources.Load<GameObject>("ChessManPrefabs/305");
                break;
        }
        return Instantiate(chessManObj).GetComponent<ChessMan>();
    }
    GameplayObject SpawnGameplayObject(string objName) //200 is box, 201 is boulder
    {
        string path = "ObjectPrefabs/" + objName;
        GameObject obj = Resources.Load<GameObject>(path);
        return Instantiate(obj).GetComponent<GameplayObject>();
    }
}
