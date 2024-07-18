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
    [SerializeField] Button turnBtn;
    [SerializeField] TMP_Text turnNumber;

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
        turnBtn.onClick.AddListener(OnTurnBtnClicked);
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
        GameplayManager.Instance.Undo();
        // Update Number
        UpdateNumber();

    }

    private void OnSolveBtnClicked()
    {
        // Call Solve method from GamePlay
        PlayHintAnim();
        GameplayManager.Instance.ShowHint();
        // Update Number
        UpdateNumber();

        DisableHintButton();
    }

    

    private void OnTurnBtnClicked()
    {
        GameplayManager.Instance.IncreaseTurn();
        UpdateNumber();
    }
    private void UpdateNumber()
    {
        backNumber.text = SaveLoadManager.Instance.GameData.undoNum.ToString();
        solveNumber.text = SaveLoadManager.Instance.GameData.solveNum.ToString();
        turnNumber.text = SaveLoadManager.Instance.GameData.turnNum.ToString();
        RecheckItemNumber();
    }

    public void RecheckItemNumber()
    {
        int undoNum = SaveLoadManager.Instance.GameData.undoNum;
        int solveNum = SaveLoadManager.Instance.GameData.solveNum;
        int turnNum = SaveLoadManager.Instance.GameData.turnNum;

        //
        backBtn.interactable = undoNum > 0;
        solveBtn.interactable= solveNum > 0;
        turnBtn.interactable = turnNum > 0;

        if (!GameplayManager.Instance.isShowHint) DisableHintButton();

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
        iconTranfrom.DOScale(2f, 0.5f)
            .SetEase(Ease.OutBounce)
            .OnComplete(() =>
            {
                iconTranfrom.DOScale(0f, 0.5f)
                .SetEase(Ease.InBack)
                .OnComplete(() => { hintEffectCanvas.SetActive(false); });
            });

        
    }
}
