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

    ChessMan curChessMan, hitChessMan;
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
                }
                else
                {
                    HideMouseOver();
                    MoveInvalid();
                    HideOulineHitChessMan();
                    Debug.Log("Nhan vao tile ko phai ground");
                }
                isPicking = false;
            }

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
        curChessMan = hit.transform.GetComponent<ChessMan>();
        GameplayManager.Instance.ShowAvailableMove(curChessMan.config, curChessMan.posIndex);
        posIcon.gameObject.SetActive(true);
        posIcon.position = curChessMan.posIndex + Vector3.one * 0.02f;
        isPicking = true;

        if (hitChessMan == null || (hitChessMan != null && hitChessMan.posIndex != curChessMan.posIndex))
        {
            hitChessMan = curChessMan;
            hitChessMan.outline.OutlineWidth = 10;
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
        mouseOver.transform.position = pos + Vector3.up * 1.02f;
    }
    void HideMouseOver()
    {
        mouseOver.gameObject.SetActive(false);
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
