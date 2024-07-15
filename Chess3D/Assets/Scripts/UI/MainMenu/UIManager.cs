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

    private bool isButtonLoad = false;
    private bool isCustomLoad = false;

    private readonly Color leftCircleColor = new Color(200f / 255f, 150f / 255f, 1f);
    private readonly Color rightCircleColor = new Color(1f, 150f / 255f, 200f / 255f);

    private readonly Color leftChessPieceColor = new Color(0f, 0.5f, 1f);
    private readonly Color rightChessPieceColor = new Color(1f, 0.5f, 0f);

    private readonly Stack<UI> UIStack = new Stack<UI>();
    private readonly List<Button> chapterButton = new List<Button>();
    private readonly List<Button> levelButton = new List<Button>();

    private readonly float hidePosition = 600f;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        Preset();
    }

    public void IntoChapterMenu()
    {
        StartCoroutine(Cor_IntoChapterMenu());
    }

    IEnumerator Cor_IntoChapterMenu()
    {
        isCustomLoad = true;
        yield return new WaitUntil(() => isButtonLoad);
        UIStack.Push(mainMenu);
        chapterMenu.Anim();
    }

    public void IntoLevelMenu(int chapterIndex)
    {
        StartCoroutine(Cor_IntoLevelMenu(chapterIndex));
    }

    IEnumerator Cor_IntoLevelMenu(int chapterIndex)
    {
        isCustomLoad = true;
        yield return new WaitUntil(() => isButtonLoad);
        UIStack.Push(mainMenu);
        LevelPreset(chapterIndex, GameUtils.GetChapterData(chapterIndex).levelDatas.Count);
    }

    // Setup
    private void Preset()
    {
        StartCoroutine(Cor_MenuSetup());
    }

    IEnumerator Cor_MenuSetup()
    {
        UIStack.Clear();
        TextPreset();
        SliderPreset();
        HolderPreset();
        ButtonPreset();
        ChapterPreset();
        yield return new WaitUntil(() => SoundManager.Instance != null);
        SoundManager.Instance.PlayMusic(AudioPlayer.SoundID.MUSIC_MAIN_MENU);
        if (!isCustomLoad) mainMenu.Anim();
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

        isButtonLoad = true;
    }

    private void ChapterPreset()
    {
        chapterHolder.anchoredPosition = Vector3.down * 2000f;
        for (int chapterIndex = 0; chapterIndex < GDC.Constants.GameConstants.MAX_CHAPTER; ++chapterIndex)
        {
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
        levelHolder.anchoredPosition = Vector3.down * 1500;
        for (int levelIndex = 0; levelIndex < maxLevelIndex; ++levelIndex)
        {
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
            if (levelButton[i] != null)
            {
            levelButton[i].interactable = false;
            }
        }

        for (int i = 0; i < chapterButton.Count; ++i)
        {
            if (chapterButton[i] != null)
            {
                chapterButton[i].interactable = false;
            }
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
