using GDC.Managers;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] Transform mouseOver, posIcon;
    [SerializeField] LayerMask tileLayerMask, chessLayerMask, gameplayObjectLayerMask;
    [SerializeField, ReadOnly] bool isPicking;

    ChessMan preChessMan, curChessMan, hitChessMan;
    GameplayObject hitObject;
    //bool isHitChessMan = false;
    float timeMouseDown;
    [SerializeField] float deltaTimeToHold;
    private void Update()
    {
        //DisplayMouseOver();
        
        if (Input.GetMouseButtonDown(0))
        {
            timeMouseDown = Time.time;            
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (Time.time - timeMouseDown > deltaTimeToHold)
            {
                //this is holver
            }
            else
            {
                //this is click
                MouseClickInput();
            }
        }
    }
    //void DisplayMouseOver()
    //{
    //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //    RaycastHit hit;
    //    bool isHit = Physics.Raycast(ray, out hit, 100);
        
    //    isHitChessMan = false;
    //    if (isHit)
    //    {
    //        if ((chessLayerMask & (1 << hit.transform.gameObject.layer)) != 0)
    //        {
    //            ChessMan tempChessMan = hit.transform.GetComponent<ChessMan>();
    //            if (hitChessMan == null || (hitChessMan != null && hitChessMan.posIndex == tempChessMan.posIndex))
    //            {
    //                hitChessMan = tempChessMan;
    //                hitChessMan.outline.OutlineWidth = 10;
    //                isHitChessMan = true;
    //            }
    //        }
    //        else if ((tileLayerMask & (1 << hit.transform.gameObject.layer)) != 0)
    //        {
                
    //            mouseOver.gameObject.SetActive(true);
    //            mouseOver.transform.position = hit.transform.position + Vector3.up * 1.05f;
    //        }
    //    }
    //    else
    //    {
    //        mouseOver.gameObject.SetActive(false);
    //    }       
    //}
    //private void LateUpdate()
    //{
    //    if (isHitChessMan == false && hitChessMan != null)
    //    {
    //        hitChessMan.outline.OutlineWidth = 0;
    //        hitChessMan = null;
    //    }
    //}
    void MouseClickInput()
    {
        posIcon.gameObject.SetActive(false);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        bool isHit = Physics.Raycast(ray, out hit, 100);
        if (isHit)
        {
            if (isPicking)//Neu da pick quan co
            {
                if ((tileLayerMask & (1 << hit.transform.gameObject.layer)) != 0)
                {
                    ShowObjectOver(mouseOver ,hit.transform.position);
                    HitTileToMove(hit);
                    HideOulineHitChessMan();
                    isPicking = false;
                }
                else if ((chessLayerMask & (1 << hit.transform.gameObject.layer)) != 0)
                {
                    HideOulineHitChessMan();
                    HitChessMan(hit);
                }
                else if ((gameplayObjectLayerMask & (1 << hit.transform.gameObject.layer)) != 0)
                {
                    HideOulineHitChessMan();
                    HitObject(hit);
                    //HitTileToMove(hit);
                }
                else
                {
                    HideMouseOver();
                    MoveInvalid();
                    HideOulineHitChessMan();
                    isPicking = false;
                    Debug.Log("Nhan vao tile ko phai ground");
                }
            }
            else
            {
                if ((chessLayerMask & (1 << hit.transform.gameObject.layer)) != 0)
                {
                    HideMouseOver();
                    HideOulineHitChessMan();
                    HitChessMan(hit);
                }
                else if ((tileLayerMask & (1 << hit.transform.gameObject.layer)) != 0)
                {
                    SoundManager.Instance.PlaySound(AudioPlayer.SoundID.SFX_CLICK_TILE);
                    ShowObjectOver(mouseOver, hit.transform.position);
                    HideOulineHitChessMan();
                }
                else if ((gameplayObjectLayerMask & (1 << hit.transform.gameObject.layer)) != 0)
                {
                    HideOulineHitChessMan();
                    HitObject(hit);
                }
            }

            //Debug.Log("hit: " + hit.transform.name);
        }
        else
        {
            Debug.Log("Nhan vao hu khong");
            HideMouseOver();
            HideOulineHitChessMan();
            GameplayManager.Instance.HideAvailableMove();
            isPicking = false;
        }
    }
    void HitObject(RaycastHit hit)
    {
        GameplayManager.Instance.HideAvailableMove();
        hitObject = hit.transform.GetComponent<GameplayObject>();
        hitObject.SetOutline(10, Color.white);

        if (GameplayManager.Instance.enemyTurn == false && GameplayManager.Instance.isAnimMoving == false && isPicking)
        {
            if (GameplayManager.Instance.CheckMove(curChessMan.config, curChessMan.posIndex, hitObject.posIndex))
            {
                Debug.Log("Object duoc tuong tac");
                GameplayManager.Instance.MakeMove(curChessMan, hitObject.posIndex);
                HideOutlineHitObject();
            }
            //else if (GameplayManager.Instance.CheckMove(curChessMan.config, curChessMan.posIndex, hitObject.posIndex + Vector3.up))
            //{
            //    Debug.Log("Dung phia tren object");
            //    GameplayManager.Instance.MakeMove(curChessMan, hitObject.posIndex + Vector3.up);
            //    HideOutlineHitObject();
            //}
            else HitTileToMove(hit, true);
        }
    }
    void HitChessMan(RaycastHit hit)
    {
        SoundManager.Instance.PlaySound(AudioPlayer.SoundID.SFX_CLICK_CHESSMAN);

        GameplayManager.Instance.HideAvailableMove();
        GameplayManager.Instance.uiGameplayManager.uIChessManPanel.HideOutlineFromPanel();
        preChessMan = curChessMan;
        curChessMan = hit.transform.GetComponent<ChessMan>();
        if (GameplayManager.Instance.enemyTurn == false && GameplayManager.Instance.isAnimMoving == false)
        {
            if (curChessMan.isEnemy == false)
            {
                GameplayManager.Instance.ShowAvailableMove(curChessMan.config, curChessMan.posIndex);
                //posIcon.gameObject.SetActive(true);
                //posIcon.position = curChessMan.posIndex + Vector3.one * 0.02f;
                ShowObjectOver(posIcon, curChessMan.posIndex + Vector3.down);
                isPicking = true;
                CheckShowOutlineChessMan();
            }
            else
            {
                if (isPicking)
                {
                    if (GameplayManager.Instance.CheckMove(preChessMan.config,preChessMan.posIndex,curChessMan.posIndex))
                    {
                        //Enemy co the an duoc
                        Debug.Log("Enemy bi an");
                        GameplayManager.Instance.MakeMove(preChessMan, curChessMan.posIndex, curChessMan);
                        
                    }
                    else
                    {
                        //Enemy khong the an duoc
                        isPicking = false;
                        CheckShowOutlineChessMan();
                    }
                }
                else
                {
                    CheckShowOutlineChessMan();
                }         
            }
        }
        else
        {
            CheckShowOutlineChessMan();
        }
    }
    void HitTileToMove(RaycastHit hit, bool isHitGameplayObject = false)
    {
        SoundManager.Instance.PlaySound(AudioPlayer.SoundID.SFX_CLICK_TILE);

        Vector3 tileToMoveIndex = Vector3.zero;
        if (isHitGameplayObject)
        {
            tileToMoveIndex = hit.transform.GetComponent<GameplayObject>().posIndex + Vector3.up;
            //Debug.Log("TILE TO MOVE " + tileToMoveIndex);
        }
        else
        {
            tileToMoveIndex = hit.transform.position + Vector3.up; //Hien tai position = tile index
        }

        if (GameplayManager.Instance.CheckMove(curChessMan.config, curChessMan.posIndex, tileToMoveIndex))
        {
            ChessMan defeatedEnemyChessMan = null;
            if (GameUtils.GetTile(tileToMoveIndex) == GDC.Enums.TileType.ENEMY_CHESS)
            {
                defeatedEnemyChessMan = GameUtils.GetGameplayObjectByPosition(tileToMoveIndex).transform.GetComponent<ChessMan>();
            }
            GameplayManager.Instance.MakeMove(curChessMan, tileToMoveIndex, defeatedEnemyChessMan);
            GameplayManager.Instance.HideAvailableMove();
        }
        else
        {
            MoveInvalid();
        }
    }
    void MoveInvalid()
    {
        GameplayManager.Instance.HideAvailableMove();
        Debug.Log("Nuoc di khong hop le");
    }
    void ShowObjectOver(Transform obj, Vector3 pos)
    {
        obj.gameObject.SetActive(true);
        
        switch (GameplayManager.Instance.levelData.GetTileInfoNoDeep(pos).tileType)
        {
            case GDC.Enums.TileType.SLOPE_0:
                obj.transform.position = pos + new Vector3(0, 0.52f, 0.05f);
                obj.localScale = new Vector3(1, 1.4f, 1);
                obj.rotation = Quaternion.Euler(45, 180, 0);
                break;
            case GDC.Enums.TileType.SLOPE_90:
                obj.transform.position = pos + new Vector3(-0.05f, 0.52f, 0f);
                obj.rotation = Quaternion.Euler(45, 90, 0);
                obj.localScale = new Vector3(1, 1.4f, 1);
                break;
            case GDC.Enums.TileType.SLOPE_180:
                obj.transform.position = pos + new Vector3(0, 0.52f, -0.05f);
                obj.rotation = Quaternion.Euler(45, 0, 0);
                obj.localScale = new Vector3(1, 1.4f, 1);
                break;
            case GDC.Enums.TileType.SLOPE_270:
                obj.transform.position = pos + new Vector3(0.05f, 0.52f, 0f);
                obj.rotation = Quaternion.Euler(45, 270, 0);
                obj.localScale = new Vector3(1, 1.4f, 1);
                break;
            default:
                obj.transform.position = pos + Vector3.up * 1.02f;
                obj.rotation = Quaternion.Euler(90, 0, 0);
                obj.localScale = new Vector3(1, 1, 1);
                break;
        }
    }
    void HideMouseOver()
    {
        mouseOver.gameObject.SetActive(false);
    }
    void CheckShowOutlineChessMan()
    {
        if (hitChessMan == null || (hitChessMan != null && hitChessMan.posIndex != curChessMan.posIndex))
        {
            hitChessMan = curChessMan;
            hitChessMan.SetOutline(10, Color.white);
        }
    }
    void HideOulineHitChessMan()
    {
        GameplayManager.Instance.HideOutLineAllChessMan();
        if (hitChessMan != null)
        {
            hitChessMan.SetOutline(0);
            hitChessMan = null;
        }
        HideOutlineHitObject();
    }
    void HideOutlineHitObject()
    {
        if (hitObject != null)
        {
            hitObject.SetOutline(0);
            hitObject = null;
        }
    }
}
