using GDC.Enums;
using GDC.Managers;
using NaughtyAttributes;
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
    [SerializeField] UIPawnPromotion uiPawnPromotion;
    [SerializeField] TutorialConfig tutorialConfig;
    

    bool isChessManPanelOn;


    private void Awake()
    {
        Instance = this;
    }
    //private void Start()
    //{

    //    //Setup();
    //}

    [Button]
    public void Setup()
    {
        Debug.Log("Setup UI");
        //
        isChessManPanelOn = false;
        // Setup UI
        uIChessManPanel.Setup();
        uIInformationPanel.Setup();
        uiPawnPromotion.Setup();
        //uIGameplaySlider.Setup();
        // Assign btn
        settingBtn.onClick.AddListener(OnSetting);
        toggleChessManBtn.onClick.AddListener(OnToggleBtnClicked);

        CheckShowTutorial();
    }

    public void CheckShowTutorial()
    {
        int chapterId = GameplayManager.Instance.chapterData.id;
        int levelId = GameplayManager.Instance.levelData.id;

        //Neu man nay da duoc choi qua thi ko can show tutorial nua
        if (SaveLoadManager.Instance.GameData.CheckPlayedLevelBefore(chapterId, levelId)) return;

        foreach (var tutorialData in tutorialConfig.tutorialDatas)
        {
            if (chapterId == tutorialData.chapterIndex && levelId == tutorialData.levelIndex)
            {
                uiTutorial.EnqueueTutorial(tutorialData);
            }
        }
        StartCoroutine(Cor_ShowTutorial());
    }
    IEnumerator Cor_ShowTutorial()
    {
        yield return new WaitForSeconds(1f);
        uiTutorial.Show();
    }
    public void ShowWin(bool isNewRecord)
    {
        uiWinPanel.Show(isNewRecord);
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

    public void ShowPromote()
    {
        uiPawnPromotion.Open();
    }

    public ChessManType GetPromoteType()
    {
        return uiPawnPromotion.GetPromoteType();
    }

    public void UpdateHolder(ChessMan chessMan)
    {
        uIChessManPanel.UpdateHolder(chessMan);
    }
    
    public void ChangeTurn(bool enemyturn)
    {
        if (enemyturn)
        {
            uIInformationPanel.ChangeToEnemyTurn();
        }
        else
        {
            uIInformationPanel.ChangeToPlayerTurn();
        }
    }
   
}
