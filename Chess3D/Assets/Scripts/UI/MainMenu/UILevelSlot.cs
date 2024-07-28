using GDC.Managers;
using System.Collections;
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
    // public string defaultPath;
    public int chapterIndex = 0;
    public int levelIndex = 0;

    private int maxStarCount = 3;
    private int currentStarCount;
    private bool isAvailable = true;
    private LevelData levelData;

    public void LevelSetup(int chapterIndex, int levelIndex)
    {
        this.chapterIndex = chapterIndex;
        this.levelIndex = levelIndex;
        StartCoroutine(Cor_LevelSetup());
    }

    IEnumerator Cor_LevelSetup()
    {
        yield return new WaitUntil(() => SaveLoadManager.Instance != null);
        this.currentStarCount = SaveLoadManager.Instance.GameData.GetLevelStar(chapterIndex, levelIndex);
        levelData = GameUtils.GetLevelData(chapterIndex, levelIndex);
        GameData gameData = SaveLoadManager.Instance.GameData;

        if (levelIndex > gameData.currentLevelOfChapters[chapterIndex])
        {
            this.isAvailable = false;
            transform.GetComponent<Image>().color = Color.gray;
            levelImage.color = Color.gray;
        }
        else
        {
            this.isAvailable = true;
            transform.GetComponent<Image>().color = Color.white;
        }

        // EASY ACCESS PURPORSE
#if UNITY_EDITOR
        isAvailable = true;
#endif

        SpriteSetup();
        ButtonSetup();
        TextSetup();
        StarSetup();
    }

    private void SpriteSetup()
    {
        Sprite sprite;
        //if (isAvailable)
        //{
        //    sprite = levelData.thumbnail;
        //}
        //else
        //{
        //    sprite = Resources.Load<Sprite>("UI/DefaultAsset/LoadingScreenGradient.png");
        //}
        sprite = levelData.thumbnail;
        levelImage.sprite = sprite;
        // Debug.Log("Level " + levelIndex + " is available " + isAvailable);
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
            levelText.text = $"Level {chapterIndex + 1}-{levelIndex + 1}";
        }
        else
        {
            levelText.text = "Unavailable";
        }
    }

    private void LoadLevel(int chapterIndex, int levelIndex)
    {
        if (UIManager.Instance.isClick) return;
        UIManager.Instance.isClick = true;
        SoundManager.Instance.PlaySound(AudioPlayer.SoundID.SFX_BUTTON_CLICK);
        GDC.Managers.GameManager.Instance.LoadSceneManually(
            GDC.Enums.SceneType.GAMEPLAY,
            GDC.Enums.TransitionType.IN,
            SoundType.NONE,
            cb: () =>
            {
                GameManager.Instance.SetInitData(chapterIndex, levelIndex);
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