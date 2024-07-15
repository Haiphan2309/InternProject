using GDC.Enums;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIPawnPromotion : MonoBehaviour
{
    

    [SerializeField] GameObject rookNode;
    [SerializeField] GameObject bishopNode;
    [SerializeField] GameObject knightNode;
    [SerializeField] GameObject queenNode;

    [SerializeField] UIPopupAnim uiPopupAnim;



    List<Button> buttonList;
    public ChessManType promoteType;

    //private void Start()
    //{
    //    Setup();
    //}

    public void Setup()
    { 
       
        uiPopupAnim = GetComponent<UIPopupAnim>();
        uiPopupAnim.Hide();
        SetupHolderButtons();

    }

    private void SetupHolderButtons()
    {
        rookNode.GetComponentInChildren<Button>().onClick.AddListener(() => {
            OnButtonClicked(ChessManType.CASTLE);
        });

        bishopNode.GetComponentInChildren<Button>().onClick.AddListener(() => {
            OnButtonClicked(ChessManType.BISHOP);
        });

        knightNode.GetComponentInChildren<Button>().onClick.AddListener(() => {
            OnButtonClicked(ChessManType.KNIGHT);
        });

        queenNode.GetComponentInChildren<Button>().onClick.AddListener(() => {
            OnButtonClicked(ChessManType.QUEEN);
        });
    }
    private void OnButtonClicked(ChessManType chessManType)
    {
        promoteType = chessManType;
        Close();
    }

   

    public void Open()
    {
        gameObject.SetActive(true);
        uiPopupAnim.Show();
       // GameplayManager.Instance.camController.Lock();
    }

    public void Close()
    {
        uiPopupAnim.Hide();
      //  GameplayManager.Instance.camController.Unlock();

    }
    public ChessManType GetPromoteType()
    {
        return promoteType;
    }
}
