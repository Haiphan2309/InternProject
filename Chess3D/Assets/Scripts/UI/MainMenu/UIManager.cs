using DG.Tweening;
using Mono.Cecil;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    public RectTransform backgroundSystem;
    public RectTransform textSystem;
    public RectTransform buttonSystem;
    public RectTransform levelSystem;

    public UIMainMenu mainMenu;
    public UILevelMenu levelMenu;
    public UILevelSlot levelSlotPrefab;

    RectTransform content;

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
        ButtonPreset();
        LevelPreset();
        TitlePreset();
    }

    private void SliderPreset()
    {
        RectTransform topSlider = backgroundSystem.Find("TopSlider") as RectTransform;
        topSlider.anchoredPosition = Vector3.up * 600f;
        RectTransform bottomSlider = backgroundSystem.Find("BottomSlider") as RectTransform;
        bottomSlider.anchoredPosition = Vector3.down * 600f;
    }

    private void ButtonPreset()
    {
        RectTransform startButton = buttonSystem.Find("StartButton") as RectTransform;
        startButton.anchoredPosition = Vector3.right * 600f + Vector3.up * 25;
        startButton.GetComponent<Button>().onClick.AddListener(StartButton);

        RectTransform settingButton = buttonSystem.Find("SettingButton") as RectTransform;
        settingButton.anchoredPosition = Vector3.left * 600f + Vector3.down * 50;
        settingButton.GetComponent<Button>().onClick.AddListener(SettingButton);

        RectTransform returnButton = buttonSystem.Find("ReturnButton") as RectTransform;
        returnButton.anchoredPosition = Vector3.left * 600f + Vector3.up * 100;
        returnButton.GetComponent<Button>().onClick.AddListener(ReturnButton);
    }

    private void LevelPreset()
    {
        levelSystem.anchoredPosition = Vector3.down * 1200f;
        content = levelSystem.Find("Scroll View").Find("Viewport").Find("Content") as RectTransform;
        for(int i = 0; i < 6; ++i)
        {
            levelSlotPrefab.Setup(i);
            Instantiate(levelSlotPrefab, content);
        }
    }

    private void TitlePreset()
    {
        RectTransform title = textSystem.Find("Title") as RectTransform;
        title.anchoredPosition = Vector3.right * 400f + Vector3.up * 900f;
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
