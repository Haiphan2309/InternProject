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
    [SerializeField] private Button settingBtn;
    [SerializeField] private Button toggleChessManBtn;
    [SerializeField] private Button cameraModeBtn;
    [SerializeField] private Button backBtn;
    [SerializeField] private TMP_Text backNumber;
    [SerializeField] private Button solveBtn;
    [SerializeField] private TMP_Text solveNumber;
    [SerializeField] private Button turnBtn;
    [SerializeField] private TMP_Text turnNumber;

    [SerializeField] private Sprite MoveIcon;
    [SerializeField] private Sprite RotateIcon;

    [SerializeField] private GameObject hintEffectCanvas;
    [SerializeField] private GameObject turnEffectCanvas;


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
  
        PlayClickAnim(settingBtn);
        SoundManager.Instance.PlaySound(AudioPlayer.SoundID.SFX_BUTTON_CLICK);
        UIGameplayManager.Instance.OnSetting();
    }
    private void OnToggleBtnClicked()
    {
        PlayClickAnim(toggleChessManBtn);
        SoundManager.Instance.PlaySound(AudioPlayer.SoundID.SFX_BUTTON_CLICK);
        UIGameplayManager.Instance.OnToggleBtnClicked();

    }
    private void OnCameraModeBtnClicked()
    {
        PlayClickAnim(cameraModeBtn);
        SoundManager.Instance.PlaySound(AudioPlayer.SoundID.SFX_BUTTON_CLICK);

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
        PlayClickAnim(backBtn);
        SoundManager.Instance.PlaySound(AudioPlayer.SoundID.SFX_BUTTON_CLICK);

        // Call Back method from GamePlay 
        GameplayManager.Instance.Undo();

        // Update Number
        UpdateNumber();

    }

    private void OnSolveBtnClicked()
    {
        // Call Solve method from GamePlay
        PlayButtonAnim(hintEffectCanvas);
        PlayClickAnim(solveBtn);
        SoundManager.Instance.PlaySound(AudioPlayer.SoundID.SFX_BUTTON_CLICK);

        GameplayManager.Instance.ShowHint();

        // Update Number
        UpdateNumber();
        SoundManager.Instance.PlaySound(AudioPlayer.SoundID.SFX_STAR);

        DisableHintButton();
    }

    

    private void OnTurnBtnClicked()
    {
        PlayButtonAnim(turnEffectCanvas);
        PlayClickAnim(turnBtn);
        SoundManager.Instance.PauseSFX(AudioPlayer.SoundID.SFX_BUTTON_CLICK);

        GameplayManager.Instance.IncreaseTurn();
        UpdateNumber();
    }
    private void UpdateNumber()
    {
        backNumber.text = SaveLoadManager.Instance.GameData.undoNum.ToString();
        solveNumber.text = SaveLoadManager.Instance.GameData.solveNum.ToString();
        turnNumber.text = SaveLoadManager.Instance.GameData.turnNum.ToString();
        RecheckItems();
    }

    public void RecheckItems()
    {
        
        RecheckItemNumber();
        ChekcCanUndo();
        if (GameplayManager.Instance.isBeginRound == 0)
        {
            backBtn.interactable = false;
        }
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

        if (!GameplayManager.Instance.canHint) DisableHintButton();

    }

    public bool ChekcCanUndo()
    {
        int remainTurn = GameplayManager.Instance.remainTurn;
        int maxTurn = GameplayManager.Instance.levelData.maxTurn;
        if (remainTurn >= maxTurn)
        {
            
            turnBtn.interactable = false;
            return false;
        }
        else
        {
            return true;
        }
    }

    public void DisableHintButton()
    {
        solveBtn.interactable = false;
    }

    public void DisableAllButtons()
    {
        backBtn.interactable = false;
        solveBtn.interactable = false;
        turnBtn.interactable = false;
    } 



    private void PlayButtonAnim(GameObject canvas)
    {
        // Call anim when press  button
        canvas.SetActive(true);
        RectTransform iconTranfrom = canvas.transform.GetChild(0).GetComponent<RectTransform>();
        iconTranfrom.DOScale(2f, 0.5f)
            .SetEase(Ease.OutBounce)
            .OnComplete(() =>
            {
                iconTranfrom.DOScale(0f, 0.5f)
                .SetEase(Ease.InBack)
                .OnComplete(() => { canvas.SetActive(false); });
            });

    }
    private void PlayClickAnim(Button button)
    {
        Debug.Log("Play anim");
        var sequence = DOTween.Sequence();
        sequence.Append(button.gameObject.GetComponent<RectTransform>().DOScale(0.8f, 0.15f).SetEase(Ease.InBounce))
                .Append(button.gameObject.GetComponent<RectTransform>().DOScale(1f, 0.15f).SetEase(Ease.OutBounce));
    }
}
