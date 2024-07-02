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
        TextPreset();
    }

    private void SliderPreset()
    {
        Transform topSlider = backgroundSystem.GetChild(1);
        topSlider.GetComponent<RectTransform>().anchoredPosition = Vector3.up * 600f;

        Transform bottomSlider = backgroundSystem.GetChild(2);
        bottomSlider.GetComponent<RectTransform>().anchoredPosition = Vector3.down * 600f;
    }

    private void ButtonPreset()
    {
        Transform startButton = buttonSystem.GetChild(0);
        startButton.GetComponent<RectTransform>().anchoredPosition = Vector3.right * 600f + Vector3.up * 25;
        startButton.GetComponent<Button>().onClick.AddListener(StartButton);

        Transform settingButton = buttonSystem.GetChild(1);
        settingButton.GetComponent<RectTransform>().anchoredPosition = Vector3.left * 600f + Vector3.down * 50;
        settingButton.GetComponent<Button>().onClick.AddListener(SettingButton);

        Transform returnButton = buttonSystem.GetChild(2);
        returnButton.GetComponent<RectTransform>().anchoredPosition = Vector3.left * 600f + Vector3.up * 100;
        returnButton.GetComponent<Button>().onClick.AddListener(ReturnButton);
    }

    private void LevelPreset()
    {
        levelSystem.anchoredPosition = Vector3.down * 1200f;
        Transform content = levelSystem.GetChild(0).GetChild(0).GetChild(0);
        for(int i = 0; i < 6; ++i)
        {
            levelSlotPrefab.Setup(i);
            Instantiate(levelSlotPrefab, content);
        }
    }

    private void TextPreset()
    {
        Transform title = textSystem.GetChild(0);
        title.GetComponent<RectTransform>().anchoredPosition = Vector3.right * 400f + Vector3.up * 300f;
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
