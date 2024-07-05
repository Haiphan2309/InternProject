using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGameplayManager : MonoBehaviour
{
    public static UIGameplayManager Instance { get; private set; }

    [SerializeField] Button settingBtn;
    [SerializeField] Button toggleChessManBtn;

    public UIChessManPanel uIChessManPanel;
    //[SerializeField] UIGameplaySlider uIGameplaySlider;
    public UIInformationPanel uIInformationPanel;

    [SerializeField] UIWinPanel uiWinPanel;
    [SerializeField] UILosePanel uiLosePanel;
    [SerializeField] UISetting uiSetting;
    [SerializeField] UITutorial uiTutorial;
    

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
        Debug.Log("Setup UI");
        //
        isChessManPanelOn = false;
        // Setup UI
        uIChessManPanel.Setup();
        uIInformationPanel.Setup();
        //uIGameplaySlider.Setup();
        // Assign btn
        settingBtn.onClick.AddListener(OnSetting);
        toggleChessManBtn.onClick.AddListener(OnToggleBtnClicked);
        
    }

    public void ShowTutorial(Sprite tutorialSprite)
    {
        uiTutorial.Show(tutorialSprite);

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
