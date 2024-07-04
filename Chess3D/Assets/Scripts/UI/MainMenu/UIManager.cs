using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
/*    public RectTransform holderSystem;
    public RectTransform sliderSystem;
    public RectTransform textSystem;
    public RectTransform buttonSystem;
*/

    public RectTransform title;

    public RectTransform topSlider;
    public RectTransform topChessHolder;

    public RectTransform bottomSlider;
    public RectTransform bottomChessHolder;

    public Transform startButton;
    public Transform settingButton;
    public Transform returnButton;
    public Transform creditButton;

    public RectTransform levelHolder;
    public Transform content;

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
        creditButton.GetComponent<Button>().onClick.AddListener(() => Debug.Log("Fuck you"));

        // POSITION SETTTER
        startButton.GetComponent<RectTransform>().anchoredPosition = Vector3.right * 600f + Vector3.up * 25;
        settingButton.GetComponent<RectTransform>().anchoredPosition = Vector3.left * 600f + Vector3.down * 50;
        returnButton.GetComponent<RectTransform>().anchoredPosition = Vector3.left * 600f + Vector3.up * 100;
    }

    private void LevelPreset()
    {
        levelHolder.anchoredPosition = Vector3.down * 1200f;
        for (int i = 0; i < 20; ++i)
        {
            levelSlotPrefab.Setup(i);
            Instantiate(levelSlotPrefab, content);
        }
    }

    private void TextPreset()
    {
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
