using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class UILevelSlot : MonoBehaviour
{
    public Image levelImage;
    public RectTransform levelStar;
    public TMP_Text levelText;

    public string levelPath;
    public string assetPath;
    public string defaultPath;
    public int chapterIndex = 0;
    public int levelIndex = 0;

    private int maxStarCount = 3;
    private int currentStarCount;
    private bool isAvailable = true;
    private LevelData levelData;

    public void LevelSetup(int chapterIndex, int levelIndex, int currentStarCount)
    {
        this.chapterIndex = chapterIndex;
        this.levelIndex = levelIndex;
        this.currentStarCount = currentStarCount;

        assetPath = "ScriptableObjects/LevelData" + "/Level_" + levelIndex;
        defaultPath = "UI/DefaultAsset/LoadingScreenGradient";
        levelData = Resources.Load<LevelData>(assetPath);
        if (levelData == null)
        {
            this.isAvailable = false;
            transform.GetComponent<Image>().color = Color.black;
        }
        else
        {
            this.isAvailable = true;
            transform.GetComponent<Image>().color = Color.white;
        }

        SpriteSetup();
        ButtonSetup();
        TextSetup();
        StarSetup();
    }

    private void SpriteSetup()
    {
        Sprite sprite;
        if (isAvailable)
        {
            sprite = levelData.thumbnail;
            isAvailable = true;
            transform.GetComponent<Image>().color = Color.white;
        }
        else
        {
            sprite = Resources.Load<Sprite>(defaultPath);
            isAvailable = false;
            transform.GetComponent<Image>().color = Color.black;
        }
        levelImage.sprite = sprite;
        Debug.Log("Level " + levelIndex + " is available " + isAvailable);
    }

    private void ButtonSetup()
    {
        if (isAvailable)
        {
            transform.GetComponent<Button>().interactable = true;
            transform.GetComponent<Button>().onClick.AddListener(delegate { LoadLevel(chapterIndex, levelIndex); });
        }
        else
        {
            transform.GetComponent<Button>().interactable = false;
        }
    }

    private void TextSetup()
    {
        if (isAvailable)
        {
            levelText.text = "Level " + (levelIndex + 1).ToString();
        }
        else
        {
            levelText.text = "Unavailable";
        }
    }

    private void LoadLevel(int chapterIndex, int levelIndex)
    {
        GDC.Managers.GameManager.Instance.LoadSceneManually(
            GDC.Enums.SceneType.GAMEPLAY,
            GDC.Enums.TransitionType.IN,
            SoundType.NONE,
            cb: () =>
            {
                GDC.Managers.GameManager.Instance.SetInitData(chapterIndex, levelIndex);
            },
            true);
    }

    private void StarSetup()
    {
        for(int i = 0; i < maxStarCount; ++i)
        {
            if (i < currentStarCount)
                levelStar.GetChild(i).GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/DefaultAsset/Star_0");
            else
                levelStar.GetChild(i).GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/DefaultAsset/Star_1");
        }
    }
}