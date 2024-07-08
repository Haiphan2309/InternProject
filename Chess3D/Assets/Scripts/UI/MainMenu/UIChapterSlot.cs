using GDC.Managers;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIChapterSlot : MonoBehaviour
{
    public Image chapterImage;
    public TMP_Text chapterText;
    public TMP_Text requirementsText;

    public int chapterIndex = 0;
    private bool isAvailable = true;
    private ChapterData chapterData;

    public void ChapterSetup(int chapterIndex)
    {
        this.chapterIndex = chapterIndex;
        this.chapterData = GameUtils.GetChapterData(this.chapterIndex);

        if (this.chapterData == null || this.chapterData.starRequire <= 10) //SaveLoadManager.Instance.GameData.GetAllStar())
        {
            isAvailable = true;
            transform.GetComponent<Image>().color = Color.white;
        }
        else
        {
            isAvailable = false;
            transform.GetComponent<Image>().color = Color.black;
        }

        SpriteSetup();
        ButtonSetup();
        TextSetup();
    }

    private void SpriteSetup()
    {
        Sprite sprite;
        if (isAvailable)
        {
            sprite = chapterData.thumbnail;
        }
        else
        {
            sprite = null;
        }
        chapterImage.sprite = sprite;
        Debug.Log("Chapter " + chapterIndex + " is available " + isAvailable);
    }

    private void ButtonSetup()
    {
        if (isAvailable)
        {
            transform.GetComponent<Button>().interactable = true;
            transform.GetComponent<Button>().onClick.AddListener(LoadLevelList);
        }
        else
        {
            transform.GetComponent<Button>().interactable = false;
        }
    }

    private void LoadLevelList()
    {
        UIManager.Instance.LevelPreset(chapterIndex, chapterData.levelDatas.Count);
    }

    private void TextSetup()
    {
        int currentPlayerStats = 10; //SaveLoadManager.Instance.GameData.GetAllStar();
        if (isAvailable)
        {
            chapterText.text = $"Chapter {chapterIndex + 1}";
            requirementsText.text = "OK";
        }
        else
        {
            chapterText.text = "???";
            requirementsText.text = $"Require: {currentPlayerStats}/{this.chapterData.starRequire} stars";
        }
    }
}
