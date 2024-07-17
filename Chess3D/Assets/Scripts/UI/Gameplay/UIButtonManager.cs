using DG.Tweening;
using GDC.Managers;
using NaughtyAttributes;
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

    [SerializeField] Sprite MoveIcon;
    [SerializeField] Sprite RotateIcon;

    [SerializeField] GameObject hintEffectCanvas;

    public void Setup()
    {
        settingBtn.onClick.AddListener(OnSetting);
        toggleChessManBtn.onClick.AddListener(OnToggleBtnClicked);
        cameraModeBtn.onClick.AddListener(OnCameraModeBtnClicked);
        backBtn.onClick.AddListener(OnBackBtnClicked);
        solveBtn.onClick.AddListener(OnSolveBtnClicked);
        //
        UpdateNumber();
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
        bool isMove = GameplayManager.Instance.camController.ChangeCameraMode();
        Image btnImg = cameraModeBtn.transform.GetChild(0).GetComponent<Image>();
        // Change button appearence when clicked
        if (isMove)
        {
            btnImg.sprite = MoveIcon;
        }
        else
        {
            btnImg.sprite = RotateIcon;
        }
    }

    private void OnBackBtnClicked()
    {
        // Call Back method from GamePlay 
            
        // Update Number
        UpdateNumber();

    }

    private void OnSolveBtnClicked()
    {
        // Call Solve method from GamePlay
        GameplayManager.Instance.ShowHint();
        // Update Number
        UpdateNumber();
        

    }

    private void UpdateNumber()
    {
        backNumber.text = SaveLoadManager.Instance.GameData.undoNum.ToString();
        solveNumber.text = SaveLoadManager.Instance.GameData.solveNum.ToString();
        RecheckItemNumber();
    }

    public void RecheckItemNumber()
    {
        int undoNum = SaveLoadManager.Instance.GameData.undoNum;
        int solveNum = SaveLoadManager.Instance.GameData.solveNum;
        if (undoNum <= 0)
        {
            backBtn.interactable = false;
        }
        else
        {
            backBtn.interactable = true;
        }

        if (solveNum <= 0)
        {
            solveBtn.interactable= false;
        }
        else
        {
            solveBtn.interactable = true;
            
        }
    }

    public void DisableHintButton()
    {
        solveBtn.interactable = false;
    }
    [Button]
    private void PlayHintAnim()
    {
        // Call anim when press Hint button
        hintEffectCanvas.SetActive(true);
        RectTransform iconTranfrom = hintEffectCanvas.transform.GetChild(0).GetComponent<RectTransform>();
        iconTranfrom.DOScale(2f, 1.5f)
            .SetEase(Ease.InOutBounce)
            .OnComplete(() =>
            {
                iconTranfrom.DOScale(0f, 0.5f)
                .SetEase(Ease.OutBack)
                .OnComplete(() => { hintEffectCanvas.SetActive(false); });
            });

        
    }
}
