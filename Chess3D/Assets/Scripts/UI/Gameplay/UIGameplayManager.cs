using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGameplayManager : MonoBehaviour
{
    public static UIGameplayManager Instance { get; private set; }

    [SerializeField] Button settingBtn;
    [SerializeField] Button toggleChessManBtn;

    [SerializeField] UIChessManPanel uIChessManPanel;
    [SerializeField] UIGameplaySlider uIGameplaySlider;

    [SerializeField] UIWinPanel uiWinPanel;
    [SerializeField] UILosePanel uiLosePanel;
    [SerializeField] UISetting uiSetting;
    

    bool isChessManPanelOn;


    private void Awake()
    {
        Instance = this;
    }
    //private void Start()
    //{
        
    //    //Setup();
    //}

    public void Setup()
    {
        //
        isChessManPanelOn = false;
        // Setup UI
        uIChessManPanel.Setup();
        uIGameplaySlider.Setup();
        // Assign btn
        settingBtn.onClick.AddListener(OnSetting);
        toggleChessManBtn.onClick.AddListener(OnToggleBtnClicked);
        
    }

    public void ShowWin()
    {
        uiWinPanel.Show();
    }
    public void ShowLose()
    {
        uiLosePanel.Show();
    }
    void OnSetting()
    {
        uiSetting.Show();
    }
    private void OnToggleBtnClicked()
    {
   
        if (!isChessManPanelOn)
        {
            uIChessManPanel.TurnOnPanel();
        }
        else
        {
            uIChessManPanel.TurnOffPanel();
        }
        isChessManPanelOn = !isChessManPanelOn;
        
    }
}
