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

    ChessMan curChessMan;
    private void Update()
    {
        DisplayMouseOver();
        
        if (Input.GetMouseButtonDown(0))
        {
            MouseDownInput();
        }    
    }
    void DisplayMouseOver()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        bool isHit = Physics.Raycast(ray, out hit, 100, tileLayerMask);
        if (isHit)
        {
            mouseOver.gameObject.SetActive(true);
            mouseOver.transform.position = hit.transform.position + Vector3.up * 1.05f;
        }
        else
        {
            mouseOver.gameObject.SetActive(false);
        }
    }
    void MouseDownInput()
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
                    HitTileToMove(hit);
                }
                else
                {
                    MoveInvalid();
                    Debug.Log("Nhan vao tile ko phai ground");
                }
                isPicking = false;
            }

            if ((chessLayerMask & (1 << hit.transform.gameObject.layer)) != 0)
            {
                HitChessMan(hit);
            }      
            
            Debug.Log("hit: " + hit.transform.name);
        }
        else
        {
            Debug.Log("Nhan vao hu khong");
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
}
