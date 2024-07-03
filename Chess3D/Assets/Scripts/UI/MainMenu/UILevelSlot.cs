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
    public void Setup(int index)
    {
        levelIndex = index;
        assetPath = "ScriptableObjects/LevelData" + "/Level_" + levelIndex;
        defaultPath = "UI/DefaultAsset/LoadingScreenGradient";
        SpriteSetup();
        TextSetup();
        StarSetup();
    }

    private void SpriteSetup()
    {
        LevelData levelData = Resources.Load<LevelData>(assetPath);
        Sprite sprite;
        if (levelData == null)
        {
            sprite = Resources.Load<Sprite>(defaultPath);
            isAvailable = false;
            transform.GetComponent<Image>().color = Color.black;
        }
        else
        {
            sprite = levelData.thumbnail;
            isAvailable = true;
        }
        levelImage.sprite = sprite;
    }

    public void ButtonSetup()
    {
        // Debug.Log("Level " + levelIndex + " Add Button");
        if (isAvailable)
        {
            LoadLevel(levelIndex);
        }
    }

    private void TextSetup()
    {
        levelText.text = "Level " + (levelIndex + 1).ToString();
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