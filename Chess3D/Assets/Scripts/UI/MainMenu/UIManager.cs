using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    public RectTransform holderSystem;
    public RectTransform sliderSystem;
    public RectTransform textSystem;
    public RectTransform buttonSystem;
    public RectTransform levelSystem;

    public RectTransform title;

    public RectTransform topSlider;
    public RectTransform topChessHolder;

    public RectTransform bottomSlider;
    public RectTransform bottomChessHolder;

    public Transform startButton;
    public Transform settingButton;
    public Transform returnButton;

    public UIMainMenu mainMenu;
    public UILevelMenu levelMenu;
    public UILevelSlot levelSlotPrefab;

    private float hidePosition = 600f;

    private Color leftCircleColor = new Color(200f / 255f, 150f / 255f, 1f);
    private Color rightCircleColor = new Color(1f, 150f / 255f, 200f / 255f);

    private Color leftChessPieceColor = new Color(0f, 0.5f, 1f);
    private Color rightChessPieceColor = new Color(1f, 0.5f, 0f);

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        Preset();
        mainMenu.Anim();
    }

    // Button setup
    private void Preset()
    {
        SliderPreset();
        HolderPreset();
        ButtonPreset();
        LevelPreset();
        TextPreset();
    }

    private void SliderPreset()
    {
        topSlider = sliderSystem.GetChild(0).GetComponent<RectTransform>();
        topSlider.anchoredPosition = Vector3.up * hidePosition;


        bottomSlider = sliderSystem.GetChild(1).GetComponent<RectTransform>();
        bottomSlider.anchoredPosition = Vector3.down * hidePosition;
    }

    private void HolderPreset()
    {
        topChessHolder = holderSystem.GetChild(0).GetComponent<RectTransform>();
        topChessHolder.GetChild(0).GetComponent<Image>().color = rightCircleColor;
        RectTransform topChessContainer = topChessHolder.GetChild(1).GetComponent<RectTransform>();
        for (int idx = 0; idx < topChessContainer.childCount; ++idx)
        {
            topChessContainer.GetChild(idx).GetComponent<Image>().color = rightChessPieceColor;
        }
        topChessHolder.anchoredPosition = Vector3.one * hidePosition;

        bottomChessHolder = holderSystem.GetChild(1).GetComponent<RectTransform>();
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
        startButton = buttonSystem.GetChild(0);
        startButton.GetComponent<Button>().onClick.AddListener(StartButton);

        settingButton = buttonSystem.GetChild(1);
        settingButton.GetComponent<Button>().onClick.AddListener(SettingButton);

        returnButton = buttonSystem.GetChild(2);
        returnButton.GetComponent<Button>().onClick.AddListener(ReturnButton);

        // POSITION SETTTER
        startButton.GetComponent<RectTransform>().anchoredPosition = Vector3.right * 600f + Vector3.up * 25;
        settingButton.GetComponent<RectTransform>().anchoredPosition = Vector3.left * 600f + Vector3.down * 50;
        returnButton.GetComponent<RectTransform>().anchoredPosition = Vector3.left * 600f + Vector3.up * 100;
    }

    private void LevelPreset()
    {
        levelSystem.anchoredPosition = Vector3.down * 1200f;
        Transform content = levelSystem.GetChild(0).GetChild(0).GetChild(0);
        for(int i = 0; i < 20; ++i)
        {
            levelSlotPrefab.Setup(i);
            Instantiate(levelSlotPrefab, content);
        }
    }

    private void TextPreset()
    {
        title = textSystem.GetChild(0).GetComponent<RectTransform>();
        title.anchoredPosition = Vector3.right * 400f + Vector3.up * 300f;
    }

    private void StartButton()
    {
        Debug.Log("Start");
        levelMenu.Anim();
    }

    private void SettingButton()
    {
        Debug.Log("Settings");
    }

    private void ReturnButton()
    {
        Debug.Log("Return");
        mainMenu.Anim();
    }


}
