using DG.Tweening;
using GDC.Enums;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class ChessHolder
{
    public ChessMan chessMan;
    
    public GameObject holderObject;
    
    public ChessHolder(ChessMan chessMan)
    {
        this.chessMan = chessMan;
    }

}
public class UIChessManPanel : MonoBehaviour
{
    [SerializeField] float turnOnPos = -220f;
    [SerializeField] float turnOffPos = 180f;

    [SerializeField] GameObject holderPrefab;
    [SerializeField] GameObject playerHolderContent;
    [SerializeField] GameObject enemyHolderContent;

    [SerializeField] ChessHolderConfig playerChessHolderConfig;
    [SerializeField] ChessHolderConfig enemyChessHolderConfig;
    //
    public float duration = 1f;
    //
    List<ChessHolder> playerHolderList;
    List<ChessHolder> enemyHolderList;
    
    Dictionary<ChessManType, Sprite> chessSpriteDic;

    RectTransform rectTransform;
    ChessHolder activatingHolder;
    [Button]
    public void Setup()
    {
        playerHolderList = new List<ChessHolder>();
        enemyHolderList = new List<ChessHolder>();
        chessSpriteDic = new Dictionary<ChessManType, Sprite>();
        rectTransform = GetComponent<RectTransform>();
        activatingHolder = null;

        LoadChessIcon();
        UpdateChessPanel();
        UpdateOnClickEvent();
        
        
    }
    
    private void LoadChessIcon()
    {
        Sprite[] spriteList = Resources.LoadAll<Sprite>("ChessIconSprite");
        foreach(var sprite in spriteList)
        {
            switch(sprite.name) 
            {
                case "KING":
                    chessSpriteDic.Add(ChessManType.KING, sprite);
                    break;

                case "QUEEN":
                    chessSpriteDic.Add(ChessManType.QUEEN, sprite);
                    break;

                case "BISHOP":
                    chessSpriteDic.Add(ChessManType.BISHOP, sprite);
                    break;

                case "CASTLE":
                    chessSpriteDic.Add(ChessManType.CASTLE, sprite);
                    break;

                case "KNIGHT":
                    chessSpriteDic.Add(ChessManType.KNIGHT, sprite);
                    break;

                case "PAWN":
                    chessSpriteDic.Add(ChessManType.PAWN, sprite);
                    break;

                default:
                    Debug.LogError("Wrong Sprite Name: " + sprite.name);
                    break;

            }
        }   
        
    }
    private void UpdateChessPanel()
    {
        foreach(var army in GameplayManager.Instance.playerArmy)
        {
            ChessHolder holder = new ChessHolder(army);
            holder.holderObject = Instantiate(holderPrefab, playerHolderContent.transform);
            // Assign Sprite to Chess Image
            Transform chessImg = holder.holderObject.transform.Find("ChessImage");
            chessImg.gameObject.GetComponent<Image>().sprite = chessSpriteDic[army.config.chessManType];
            chessImg.gameObject.GetComponent<Image>().color = playerChessHolderConfig.chessColor;
            // Assign default Color to border and background
            Transform background = holder.holderObject.transform.Find("Background");
            Transform foreground = background.GetChild(0);
            background.gameObject.GetComponent<Image>().color = playerChessHolderConfig.defaultBorder;
            foreground.gameObject.GetComponent<Image>().color = playerChessHolderConfig.defaultBackground;

            // Create Object
         
            
            playerHolderList.Add(holder);
        }
        foreach (var army in GameplayManager.Instance.enemyArmy)
        {
            ChessHolder holder = new ChessHolder(army);
            holder.holderObject = Instantiate(holderPrefab, enemyHolderContent.transform);
            // Assign Sprite to Chess Image
            Transform chessImg = holder.holderObject.transform.Find("ChessImage");
            chessImg.gameObject.GetComponent<Image>().sprite = chessSpriteDic[army.config.chessManType];
            chessImg.gameObject.GetComponent<Image>().color = enemyChessHolderConfig.chessColor;
            // Assign default Color to border and background
            Transform background = holder.holderObject.transform.Find("Background");
            Transform foreground = background.GetChild(0);
            background.gameObject.GetComponent<Image>().color = enemyChessHolderConfig.defaultBorder;
            foreground.gameObject.GetComponent<Image>().color = enemyChessHolderConfig.defaultBackground;
            
            //

            enemyHolderList.Add(holder);
        }
    }


    private void UpdateOnClickEvent()
    {
        foreach(var holder in playerHolderList) 
        {
            holder.holderObject.GetComponent<Button>().onClick.AddListener(
                () => OnHolderClicked(holder)
            );
        }
        foreach (var holder in enemyHolderList)
        {
            holder.holderObject.GetComponent<Button>().onClick.AddListener(
                () => OnHolderClicked(holder)
            );
        }
    }
    [Button]
    public void TurnOnPanel()
    {

        rectTransform.DOAnchorPosX(turnOnPos, duration).SetEase(Ease.OutBack);

    }

    [Button]
    public void TurnOffPanel()
    {

        rectTransform.DOAnchorPosX(turnOffPos, duration).SetEase(Ease.InBack);

    }

    private void OnHolderClicked(ChessHolder holder)
    {
        if (activatingHolder != null && activatingHolder == holder)
        {
            activatingHolder = null;
            GameplayManager.Instance.camController.ChangeToDefaultCamera();
            return;
        } 
        Transform target = holder.chessMan.gameObject.transform;
        GameplayManager.Instance.camController.ChangeFollow(target);
        activatingHolder = holder;
    }

}
