using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] Transform mouseOver;
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
            mouseOver.transform.position = hit.transform.position + Vector3.up * 0.51f;
        }
        else
        {
            mouseOver.gameObject.SetActive(false);
        }
    }
    void MouseDownInput()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        bool isHit = Physics.Raycast(ray, out hit, 100);
        if (isHit)
        {
            if ((chessLayerMask & (1 << hit.transform.gameObject.layer)) != 0)
            {
                curChessMan = hit.transform.GetComponent<ChessMan>();
                GameplayManager.Instance.ShowAvailableMove(curChessMan.config, curChessMan.posIndex);
                isPicking = true;

            }
            
            if (isPicking)//Neu da pick quan co
            {
                if ((tileLayerMask & (1 << hit.transform.gameObject.layer)) != 0)
                {
                    Vector3 tileToMoveIndex = hit.transform.position; //Hien tai position = tile index
                    if (GameplayManager.Instance.CheckMove(curChessMan.config, curChessMan.posIndex, tileToMoveIndex))
                    {
                        GameplayManager.Instance.MakeMove(curChessMan, tileToMoveIndex);
                    }
                    else
                    {
                        Debug.Log("Nuoc di khong hop le");
                    }
                }
                else
                {
                    Debug.Log("Nhan vao o khong the di duoc");
                }
                isPicking = false;
            }
            Debug.Log("hit: " + hit.transform.name);
        }
        else
        {
            Debug.Log("Nhan vao hu khong");
            isPicking = false;

        }
    }
}
