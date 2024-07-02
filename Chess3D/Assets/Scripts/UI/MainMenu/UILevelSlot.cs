using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UILevelSlot : MonoBehaviour
{
    public Image levelImage;
    public Button levelButton;
    public RectTransform levelStar;
    public string levelPath;
    public string assetPath;
    public TMP_Text levelText;

    private int maxStarCount = 3;
    private int levelIndex = 0;
    public void Setup(int index)
    {
        levelIndex = index % 3;
        assetPath = "ScriptableObjects/LevelData" + "/Level_" + levelIndex;
        ButtonSetup();
        SpriteSetup();
        TextSetup();
        StarSetup();
    }

    private void SpriteSetup()
    {
        levelImage.sprite = Resources.Load<LevelData>(assetPath).thumbnail;
    }

    private void ButtonSetup()
    {
        levelButton = GetComponent<Button>();
        levelButton.onClick.AddListener(LevelOnClick);
    }

    private void TextSetup()
    {
        levelText.text = "Level " + (levelIndex + 1).ToString();
    }

    private void LevelOnClick()
    {
        Debug.Log("Clicked");
    }

    private void StarSetup()
    {
        int starCount = Random.Range(0,4); //GDC.Managers.SaveLoadManager.Instance.GameData.playerLevelDatas[0].star;
        for(int i = 0; i < maxStarCount; ++i)
        {
            if (i < starCount)
                levelStar.GetChild(i).GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/Star_0");
            else
                levelStar.GetChild(i).GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/Star_1");
        }
    }
}