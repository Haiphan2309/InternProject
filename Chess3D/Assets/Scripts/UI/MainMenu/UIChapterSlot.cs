using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIChapterSlot : MonoBehaviour
{
    // public Image chapterImage;
    public TMP_Text chapterText;

    public int chapterIndex = 0;
    private bool isAvailable = true;
    private ChapterData chapterData;

    public void ChapterSetup(int chapterIndex)
    {
        this.chapterIndex = chapterIndex;
        //SpriteSetup();
        ButtonSetup();
        //TextSetup();
    }

    private void SpriteSetup()
    {
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
        UIManager.Instance.LevelPreset(chapterIndex);
    }

    private void TextSetup()
    {
        if (isAvailable)
        {
            chapterText.text = "Chapter " + (chapterIndex + 1).ToString();
        }
        else
        {
            chapterText.text = "Unavailable";
        }
    }
}
