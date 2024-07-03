using System.Collections;
using System.Collections.Generic;
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
    public int levelIndex = 0;

    private int maxStarCount = 3;
    private bool isAvailable = true;
    private LevelData levelData; 
    public void Setup(int index)
    {
        levelIndex = index;
        assetPath = "ScriptableObjects/LevelData" + "/Level_" + levelIndex;
        defaultPath = "UI/DefaultAsset/LoadingScreenGradient";
        levelData = Resources.Load<LevelData>(assetPath);
        if (levelData == null)
        {
            isAvailable = false;
            transform.GetComponent<Image>().color = Color.black;
        }
        else
        {
            isAvailable = true;
            transform.GetComponent<Image>().color = Color.white;
        }
        SpriteSetup();
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

    public void ButtonSetup()
    {
        if (isAvailable)
        {
            LoadLevel(levelIndex);
        }
        else
        {

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

    public void LoadLevel(int levelIndex)
    {
        GDC.Managers.GameManager.Instance.LoadSceneManually(
            GDC.Enums.SceneType.GAMEPLAY,
            GDC.Enums.TransitionType.IN,
            SoundType.NONE,
            cb: () =>
            {
                GDC.Managers.GameManager.Instance.SetInitData(levelIndex);
            },
            true);
    }

    private void StarSetup()
    {
        int starCount = Random.Range(0,4); //GDC.Managers.SaveLoadManager.Instance.GameData.playerLevelDatas[0].star;
        for(int i = 0; i < maxStarCount; ++i)
        {
            if (i < starCount)
                levelStar.GetChild(i).GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/DefaultAsset/Star_0");
            else
                levelStar.GetChild(i).GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/DefaultAsset/Star_1");
        }
    }
}