using GDC.Managers;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIButtonManager : MonoBehaviour
{
    [SerializeField] Button settingBtn;
    [SerializeField] Button toggleChessManBtn;
    [SerializeField] Button cameraModeBtn;
    [SerializeField] Button backBtn;
    [SerializeField] TMP_Text backNumber;
    [SerializeField] Button solveBtn;
    [SerializeField] TMP_Text solveNumber;

    public void Setup()
    {
        settingBtn.onClick.AddListener(OnSetting);
        toggleChessManBtn.onClick.AddListener(OnToggleBtnClicked);
        cameraModeBtn.onClick.AddListener(OnCameraModeBtnClicked);
        backBtn.onClick.AddListener(OnBackBtnClicked);
        solveBtn.onClick.AddListener(OnSolveBtnClicked);
    }

    private void OnSetting()
    {
        UIGameplayManager.Instance.OnSetting();
    }
    private void OnToggleBtnClicked()
    {

        UIGameplayManager.Instance.OnToggleBtnClicked();

    }
    private void OnCameraModeBtnClicked()
    {
        GameplayManager.Instance.camController.ChangeCameraMode();
        // Change button appearence when clicke 
    }

    private void OnBackBtnClicked()
    {
        // Call Back method from GamePlay 

        // Update Number
        backNumber.text = SaveLoadManager.Instance.GameData.undoNum.ToString();

    }

    private void OnSolveBtnClicked()
    {
        // Call Solve method from GamePlay
        GameplayManager.Instance.ShowHint();
        // Update Number
        solveNumber.text = SaveLoadManager.Instance.GameData.solveNum.ToString();


    }
}
