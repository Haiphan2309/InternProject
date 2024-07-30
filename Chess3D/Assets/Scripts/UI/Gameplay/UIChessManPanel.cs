using DG.Tweening;
using GDC.Enums;
using GDC.Managers;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

 
public class ChessHolder
{
    public ChessMan chessMan;
    
    public GameObject holderObject;

    public int index;

    public bool isEnemy;
    
    public ChessHolder(ChessMan chessMan)
    {
        this.chessMan = chessMan;
        this.index = chessMan.index;
        this.isEnemy = chessMan.isEnemy;
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
    [SerializeField] ChessHolderConfig disableHolderConfig;
    
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
        CreateChessPanel();
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
    private void CreateChessPanel()
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
            ChangeHolderColor(holder, false);

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
            
            ChangeHolderColor(holder, false);
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
        SoundManager.Instance.PlaySound(AudioPlayer.SoundID.SFX_CLICK_CHESSMAN);
        if (activatingHolder != null && activatingHolder == holder)
        {
            ChangeHolderColor(activatingHolder, false);
            activatingHolder?.chessMan.SetOutline(0);

            activatingHolder = null;

            GameplayManager.Instance.camController.ChangeToDefaultCamera();

            return;
        } 
        Transform target = holder.chessMan.gameObject.transform;
        GameplayManager.Instance.camController.ChangeFollow(target);
        
        
        ChangeHolderColor(activatingHolder, false);
        activatingHolder?.chessMan.SetOutline(0);
 
        activatingHolder = holder;
        ChangeHolderColor(activatingHolder, true);
        activatingHolder?.chessMan.SetOutline(10, Color.white);
        
    }
    /// <summary>
    /// Change Color of a ChessHolder
    /// </summary>
    /// <param name="holder"></param>
    /// <param name="isOn">True if chess holder is activating</param>
    /// <param name="isDisable">True if chessMan is destroyed (null)</param>
    private void ChangeHolderColor(ChessHolder holder, bool isOn, bool isDisable = false)
    {
        if (holder == null) return;
        bool isEnemy = holder.isEnemy;
        Color backgroundColor, foregroundColor;
        ChessHolderConfig holderConfig = isEnemy ? enemyChessHolderConfig : playerChessHolderConfig;
        if (isDisable)
        {
            holderConfig = disableHolderConfig;
            backgroundColor = holderConfig.defaultBorder;
            foregroundColor = holderConfig.defaultBackground;
        }
        else if (isOn)
        {
            backgroundColor = holderConfig.activeBorder;
            foregroundColor = holderConfig.activeBackground;
        }
        else
        {
            backgroundColor = holderConfig.defaultBorder;
            foregroundColor = holderConfig.defaultBackground;
        }
        //Assign default Color to border and background
        Transform background = holder.holderObject.transform.Find("Background");
        Transform foreground = background.GetChild(0);
        background.gameObject.GetComponent<Image>().color = backgroundColor;
        foreground.gameObject.GetComponent<Image>().color = foregroundColor;

    }

    /// <summary>
    /// Update Holder after promote a Pawn
    /// </summary>
    /// <param name="chessMan"></param>
    public void UpdateHolder(ChessMan chessMan) 
    {
        foreach(var holder in playerHolderList)
        {
            if (holder.chessMan == chessMan)
            {
                ReloadHolderImg(holder);
            }
        }
    }


    private void ReloadHolderImg(ChessHolder holder)
    {
        Transform chessImg = holder.holderObject.transform.Find("ChessImage");
        chessImg.gameObject.GetComponent<Image>().sprite = chessSpriteDic[holder.chessMan.config.chessManType];
    }
    public void HideOutlineFromPanel()
    {
        activatingHolder?.chessMan.SetOutline(0);
    }

    /// <summary>
    /// Disable chess holder from chessMan
    /// </summary>
    /// <param name="chessMan"></param>
    public void DisableChess(ChessMan chessMan)
    {
   
        List<ChessHolder> chessHolders = chessMan.isEnemy ? enemyHolderList : playerHolderList; 

        foreach(var holder in chessHolders)
        {
            if (holder.chessMan == chessMan)
            {
                holder.holderObject.GetComponent<Button>().interactable = false;

                ChangeHolderColor(holder, false, true);
                
                if (activatingHolder == holder)
                {
                    GameplayManager.Instance.camController.ChangeToDefaultCamera();
                }
            }
        }
        
    }

    /// <summary>
    /// Disable a ChessHolder in panel
    /// </summary>
    /// <param name="holder"></param>
    private void DisableHolder(ChessHolder holder)
    {
        holder.holderObject.GetComponent<Button>().interactable = false;

        ChangeHolderColor(holder, false, true);
    }

    /// <summary>
    /// Enable a Chessholder in panel
    /// </summary>
    /// <param name="holder"></param>
    private void EnableHolder(ChessHolder holder)
    {
        bool isOn = false;
        
        if (holder.chessMan != null && activatingHolder != null)
        {
            isOn = (activatingHolder == holder);
        }

        holder.holderObject.GetComponent<Button>().interactable = true;
        
        ChangeHolderColor(holder, isOn, false);
    }


    /// <summary>
    /// Update Chessman Panel when game undo
    /// </summary>
    /// <param name="chessManList">
    /// List of all chessMans
    /// </param>
    public void OnGameUndo(List<ChessMan> chessManList)
    {
        // Update Chessman of Chessholder if destroyed
        foreach(var chessMan in chessManList) 
        { 
            ChessHolder holder = FindHolderFromChessMan(chessMan);
            if (holder == null)
            {
                Debug.LogError("Cannot find Holder from Chessman: "+ chessMan.index);
            }

            holder.chessMan = chessMan;
            UpdateHolderStatus(holder);



        }

        //Update holder status
       

    }

    
    private ChessHolder FindHolderFromChessMan(ChessMan chessMan)
    {
        List<ChessHolder> chessHolders = chessMan.isEnemy ? enemyHolderList : playerHolderList;

        foreach (var holder in chessHolders)
        {
            if (holder.index == chessMan.index)
            {
                return holder;
            }
           
        }
        return null;
    }

    /// <summary>
    /// Update all holders if Chessman is destroyed or not 
    /// </summary>
  

    private void UpdateHolderStatus(ChessHolder holder)
    {
        if (holder.chessMan == null) // Disable Holder when null
        {
            DisableHolder(holder);
        }
        else
        {
            EnableHolder(holder);
        }
    }
}
