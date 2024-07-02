using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGameplayManager : MonoBehaviour
{

    [SerializeField] Button settingBtn;
    [SerializeField] Button toggleChessManBtn;

    [SerializeField] UIChessManPanel uIChessManPanel;
    [SerializeField] UIGameplaySlider uIGameplaySlider;


    bool isChessManPanelOn;

    private void Start()
    {
        
        Setup();
    }

    public void Setup()
    {
        //
        isChessManPanelOn = false;
        // Setup UI
        uIChessManPanel.Setup();
        uIGameplaySlider.Setup();
        // Assign btn
        toggleChessManBtn.onClick.AddListener(OnToggleBtnClicked);
    }

    private void OnToggleBtnClicked()
    {
   
        if (!isChessManPanelOn)
        {
            uIChessManPanel.TurnOn();
        }
        else
        {
            uIChessManPanel.TurnOff();
        }
        isChessManPanelOn = !isChessManPanelOn;
        
    }
}
