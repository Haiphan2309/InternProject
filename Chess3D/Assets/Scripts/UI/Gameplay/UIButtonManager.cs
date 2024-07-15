using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIButtonManager : MonoBehaviour
{
    [SerializeField] Button settingBtn;
    [SerializeField] Button toggleChessManBtn;
    [SerializeField] Button cameraModeBtn;

    public void Setup()
    {
        settingBtn.onClick.AddListener(OnSetting);
        toggleChessManBtn.onClick.AddListener(OnToggleBtnClicked);
        cameraModeBtn.onClick.AddListener(OnCameraModeBtnClicked);
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
}
