using DG.Tweening;
using GDC.Managers;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    public RectTransform topSlider;
    public RectTransform topChessHolder;

    public RectTransform bottomSlider;
    public RectTransform bottomChessHolder;

    public RectTransform title;
    public RectTransform chapter;

    public Transform startButton;
    public Transform settingButton;
    public Transform returnButton;
    public Transform creditButton;

    public Transform pageSystem;

    public RectTransform chapterHolder;
    public Transform chapterContent;

    public RectTransform levelHolder;
    public Transform levelContent;

    public UIMainMenu mainMenu;
    public UIChapterMenu chapterMenu;
    public UILevelMenu levelMenu;
    public UIChapterSlot chapterSlotPrefab;
    public UILevelSlot levelSlotPrefab;
    public UIPopupAnim creditPagePrefab;
    public UISetting settingPagePrefab;

    private readonly float hidePosition = 600f;

    private Color leftCircleColor = new Color(200f / 255f, 150f / 255f, 1f);
    private Color rightCircleColor = new Color(1f, 150f / 255f, 200f / 255f);

    private Color leftChessPieceColor = new Color(0f, 0.5f, 1f);
    private Color rightChessPieceColor = new Color(1f, 0.5f, 0f);

    private Stack<UI> UIStack = new Stack<UI>();
    private List<Button> chapterButton = new List<Button>();
    private List<Button> levelButton = new List<Button>();

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        Preset();
        mainMenu.Anim();
        SoundManager.Instance.PlayMusic(AudioPlayer.SoundID.MUSIC_MAIN_MENU);
    }

    public void IntoChapterMenu()
    {
        UIStack.Push(mainMenu);
        chapterMenu.Anim();
    }

    public void IntoLevelMenu(int chapterIndex)
    {
        UIStack.Push(mainMenu);
        LevelPreset(chapterIndex, GameUtils.GetChapterData(chapterIndex).levelDatas.Count);
    }

    // Setup
    private void Preset()
    {
        SliderPreset();
        HolderPreset();
        ButtonPreset();
        ChapterPreset();
        TextPreset();
    }

    private void SliderPreset()
    {
        topSlider.anchoredPosition = Vector3.up * hidePosition;
        bottomSlider.anchoredPosition = Vector3.down * hidePosition;
    }

    private void HolderPreset()
    {
        topChessHolder.GetChild(0).GetComponent<Image>().color = rightCircleColor;
        RectTransform topChessContainer = topChessHolder.GetChild(1).GetComponent<RectTransform>();
        for (int idx = 0; idx < topChessContainer.childCount; ++idx)
        {
            topChessContainer.GetChild(idx).GetComponent<Image>().color = rightChessPieceColor;
        }
        topChessHolder.anchoredPosition = Vector3.one * hidePosition;

        bottomChessHolder.GetChild(0).GetComponent<Image>().color = leftCircleColor;
        RectTransform bottomChessContainer = bottomChessHolder.GetChild(1).GetComponent<RectTransform>();
        for (int idx = 0; idx < bottomChessContainer.childCount; ++idx)
        {
            bottomChessContainer.GetChild(idx).GetComponent<Image>().color = leftChessPieceColor;
        }
        bottomChessHolder.anchoredPosition = -Vector3.one * hidePosition;
    }

    private void ButtonPreset()
    {
        startButton.GetComponent<Button>().onClick.AddListener(StartButton);
        settingButton.GetComponent<Button>().onClick.AddListener(SettingButton);
        returnButton.GetComponent<Button>().onClick.AddListener(ReturnButton);
        creditButton.GetComponent<Button>().onClick.AddListener(CreditButton);

        // POSITION SETTTER
        startButton.GetComponent<RectTransform>().anchoredPosition = Vector3.right * hidePosition + Vector3.up * 25f;
        settingButton.GetComponent<RectTransform>().anchoredPosition = Vector3.left * hidePosition + Vector3.down * 25f;
        returnButton.GetComponent<RectTransform>().anchoredPosition = Vector3.left * hidePosition + Vector3.up * 25f;
        creditButton.GetComponent<RectTransform>().anchoredPosition = Vector3.left * hidePosition + Vector3.down * 25f;
    }

    private void ChapterPreset()
    {
        chapterHolder.anchoredPosition = Vector3.down * 2000f;
        for (int idx = 0; idx < GDC.Constants.GameConstants.MAX_CHAPTER; ++idx)
        {
            int chapterIndex = idx;
            UIChapterSlot chapterSlot = Instantiate(chapterSlotPrefab, chapterContent);
            chapterSlot.ChapterSetup(chapterIndex);
            chapterButton.Add(chapterSlot.GetComponent<Button>());
        }
    }

    public void LevelPreset(int chapterIndex, int maxLevelIndex)
    {
        chapter.GetChild(0).GetComponent<TMP_Text>().text = $"Chapter {chapterIndex + 1}";
        chapter.GetChild(1).GetComponent<TMP_Text>().text = $"Chapter {chapterIndex + 1}";
        UIStack.Push(chapterMenu);
        levelHolder.anchoredPosition = Vector3.down * 2000f;
        for (int idx = 0; idx < maxLevelIndex; ++idx)
        {
            int levelIndex = idx;
            UILevelSlot levelSlot = Instantiate(levelSlotPrefab, levelContent);
            levelSlot.LevelSetup(chapterIndex,levelIndex);
            levelButton.Add(levelSlot.GetComponent<Button>());
        }
        levelMenu.Anim();
    }

    private void TextPreset()
    {
        title.anchoredPosition = Vector3.right * 500f + Vector3.up * 600;
        chapter.anchoredPosition = Vector3.left * 300f + Vector3.up * 600;
    }

    private void StartButton()
    {
        SoundManager.Instance.PlaySound(AudioPlayer.SoundID.SFX_BUTTON_CLICK);
        SoundManager.Instance.PlaySound(AudioPlayer.SoundID.SFX_TRANSITION_IN);
        Debug.Log("Start");
        UIStack.Push(mainMenu);
        chapterMenu.Anim();
    }

    private void SettingButton()
    {
        SoundManager.Instance.PlaySound(AudioPlayer.SoundID.SFX_BUTTON_CLICK);
        HideAllButtons();
        Debug.Log("Settings");
        UISetting settingPage = Instantiate(settingPagePrefab, pageSystem);
        settingPage.Show();
    }

    private void ReturnButton()
    {
        SoundManager.Instance.PlaySound(AudioPlayer.SoundID.SFX_BUTTON_CLICK);
        SoundManager.Instance.PlaySound(AudioPlayer.SoundID.SFX_TRANSITION_OUT);
        Debug.Log("Return");
        UI lastUI = UIStack.Peek();
        if (lastUI.GetType() == typeof(UIChapterMenu))
        {
            for(int i = 0; i < levelContent.childCount; ++i)
            {
                Destroy(levelContent.GetChild(i).gameObject);
            }
            levelButton.Clear();
        }
        else if (lastUI.GetType() == typeof(UIMainMenu))
        {
            chapterButton.Clear();
        }
        UIStack.Pop().Anim();
    }

    private void CreditButton()
    {
        SoundManager.Instance.PlaySound(AudioPlayer.SoundID.SFX_BUTTON_CLICK);
        HideAllButtons();
        Debug.Log("Credit");
        UIPopupAnim creditPage = Instantiate(creditPagePrefab, pageSystem);
        creditPage.Show();
    }

    public void HideAllButtons()
    {
        for (int i = 0; i < levelButton.Count; ++i)
        {
            levelButton[i].interactable = false;
        }
        for(int i = 0; i < chapterButton.Count; ++i)
        {
            chapterButton[i].interactable = false;
        }
        startButton.GetComponent<Button>().interactable = false;
        settingButton.GetComponent<Button>().interactable = false;
        returnButton.GetComponent<Button>().interactable = false;
        creditButton.GetComponent<Button>().interactable = false;
    }

    public void ShowAllButtons()
    {
        for (int i = 0; i < levelButton.Count; ++i)
        {
            levelButton[i].interactable = true;
        }
        for (int i = 0; i < chapterButton.Count; ++i)
        {
            chapterButton[i].interactable = true;
        }
        startButton.GetComponent<Button>().interactable = true;
        settingButton.GetComponent<Button>().interactable = true;
        returnButton.GetComponent<Button>().interactable = true;
        creditButton.GetComponent<Button>().interactable = true;
    }
}
