using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] Transform mouseOver, posIcon;
    [SerializeField] LayerMask tileLayerMask, chessLayerMask;
    [SerializeField, ReadOnly] bool isPicking;

    ChessMan preChessMan, curChessMan, hitChessMan;
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
                    ShowMouseOver(hit.transform.position);
                    HitTileToMove(hit);
                    HideOulineHitChessMan();
                    isPicking = false;
                }
                else if ((chessLayerMask & (1 << hit.transform.gameObject.layer)) != 0)
                {
                    HideOulineHitChessMan();
                    HitChessMan(hit);
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
                    HitChessMan(hit);
                }
                else if ((tileLayerMask & (1 << hit.transform.gameObject.layer)) != 0)
                {
                    ShowMouseOver(hit.transform.position);
                    HideOulineHitChessMan();
                }
            }

            Debug.Log("hit: " + hit.transform.name);
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
    void HitChessMan(RaycastHit hit)
    {
        preChessMan = curChessMan;
        curChessMan = hit.transform.GetComponent<ChessMan>();
        if (GameplayManager.Instance.enemyTurn == false)
        {
            if (curChessMan.isEnemy == false)
            {
                GameplayManager.Instance.ShowAvailableMove(curChessMan.config, curChessMan.posIndex);
                posIcon.gameObject.SetActive(true);
                posIcon.position = curChessMan.posIndex + Vector3.one * 0.02f;
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
                        GameplayManager.Instance.MakeMove(preChessMan, curChessMan.posIndex);
                        
                    }
                    else
                    {
                        //Enemy khong the an duoc
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
    void HitTileToMove(RaycastHit hit)
    {
        Vector3 tileToMoveIndex = hit.transform.position + Vector3.up; //Hien tai position = tile index
        if (GameplayManager.Instance.CheckMove(curChessMan.config, curChessMan.posIndex, tileToMoveIndex))
        {
            GameplayManager.Instance.MakeMove(curChessMan, tileToMoveIndex);
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
    void ShowMouseOver(Vector3 pos)
    {
        mouseOver.gameObject.SetActive(true);
        
        switch (GameplayManager.Instance.levelData.GetTileInfo()[(int)Mathf.Round(pos.x), (int)Mathf.Round(pos.y), (int)Mathf.Round(pos.z)].tileType)
        {
            case GDC.Enums.TileType.SLOPE_0:
                mouseOver.transform.position = pos + new Vector3(0, 0.55f, 0.1f);
                mouseOver.localScale = new Vector3(1, 1.3f, 1);
                mouseOver.rotation = Quaternion.Euler(45, 180, 0);
                break;
            case GDC.Enums.TileType.SLOPE_90:
                mouseOver.transform.position = pos + new Vector3(-0.1f, 0.55f, 0f);
                mouseOver.rotation = Quaternion.Euler(45, 90, 0);
                mouseOver.localScale = new Vector3(1, 1.3f, 1);
                break;
            case GDC.Enums.TileType.SLOPE_180:
                mouseOver.transform.position = pos + new Vector3(0, 0.55f, -0.1f);
                mouseOver.rotation = Quaternion.Euler(45, 0, 0);
                mouseOver.localScale = new Vector3(1, 1.3f, 1);
                break;
            case GDC.Enums.TileType.SLOPE_270:
                mouseOver.transform.position = pos + new Vector3(0.1f, 0.55f, 0f);
                mouseOver.rotation = Quaternion.Euler(45, 270, 0);
                mouseOver.localScale = new Vector3(1, 1.3f, 1);
                break;
            default:
                mouseOver.transform.position = pos + Vector3.up * 1.02f;
                mouseOver.rotation = Quaternion.Euler(90, 0, 0);
                mouseOver.localScale = new Vector3(1, 1, 1);
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
            hitChessMan.outline.OutlineWidth = 10;
        }
    }
    void HideOulineHitChessMan()
    {
        if (hitChessMan != null)
        {
            hitChessMan.outline.OutlineWidth = 0;
            hitChessMan = null;
        }
    }
}
