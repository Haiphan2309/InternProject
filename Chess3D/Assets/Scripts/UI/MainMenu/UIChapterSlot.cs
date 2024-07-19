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
        StartCoroutine(Cor_ChapterSetup());
    }

    IEnumerator Cor_ChapterSetup()
    {
        yield return new WaitUntil(() => SaveLoadManager.Instance != null);
        chapterData = GameUtils.GetChapterData(this.chapterIndex);
        GameData gameData = SaveLoadManager.Instance.GameData;

        if (chapterIndex > gameData.currentChapter || chapterData.starRequire > gameData.GetAllStar())
        {
            transform.GetComponent<Image>().color = Color.gray;
            isAvailable = false;
            chapterImage.color = Color.gray;
        }
        else
        {
            transform.GetComponent<Image>().color = Color.white;
            isAvailable = true;
        }

        // EASY ACCESS PURPORSE
        // isAvailable = true;

        SpriteSetup();
        ButtonSetup();
        TextSetup();
    }

    private void SpriteSetup()
    {
        Sprite sprite = chapterData.thumbnail;
        //if (isAvailable)
        //{
        //    sprite = chapterData.thumbnail;
        //}
        //else
        //{
        //    sprite = Resources.Load<Sprite>("UI/DefaultAsset/LoadingScreenGradient.png");
        //}
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
        SoundManager.Instance.PlaySound(AudioPlayer.SoundID.SFX_BUTTON_CLICK);
        SoundManager.Instance.PlaySound(AudioPlayer.SoundID.SFX_TRANSITION_IN);
        UIManager.Instance.LevelPreset(chapterIndex, chapterData.levelDatas.Count);
    }

    private void TextSetup()
    {
        int currentPlayerStats = SaveLoadManager.Instance.GameData.GetAllStar();
        if (isAvailable)
        {
            chapterText.text = $"Chapter {chapterIndex + 1}";
            requirementsText.text = "";
        }
        else
        {
            chapterText.text = "???";
            requirementsText.text = $"Require: {currentPlayerStats}/{this.chapterData.starRequire} stars";
        }
    }
}
