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
    ChessManType promoteType;

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
            OnButtonCLicked(ChessManType.CASTLE);
        });

        bishopNode.GetComponentInChildren<Button>().onClick.AddListener(() => {
            OnButtonCLicked(ChessManType.BISHOP);
        });

        knightNode.GetComponentInChildren<Button>().onClick.AddListener(() => {
            OnButtonCLicked(ChessManType.KNIGHT);
        });

        queenNode.GetComponentInChildren<Button>().onClick.AddListener(() => {
            OnButtonCLicked(ChessManType.QUEEN);
        });
    }
    private void OnButtonCLicked(ChessManType chessManType)
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
        gameObject.SetActive(false);
      //  GameplayManager.Instance.camController.Unlock();

    }
    public ChessManType GetPromoteType()
    {
        return promoteType;
    }
}
