using GDC.Managers;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISetting : MonoBehaviour
{
    [SerializeField] UIPopupAnim uiPopupAnim;
    [SerializeField] TMP_Text levelText;
    [SerializeField] Button menuBtn, replayBtn, hideButton;
    [SerializeField] Slider musicSlider, soundSlider;
    Coroutine hideCor;
    [SerializeField] int maxVolume = 10;

    [Button]
    public void Show()
    {
        gameObject.SetActive(true);
        menuBtn.onClick.AddListener(OnMenu);
        replayBtn.onClick.AddListener(OnReplay);
        hideButton.onClick.AddListener(Hide);

        musicSlider.onValueChanged.AddListener(delegate { OnChangeMusicVolume(); });
        soundSlider.onValueChanged.AddListener(delegate { OnChangeSoundVolume(); });
        musicSlider.maxValue = maxVolume;
        soundSlider.maxValue = maxVolume;
        musicSlider.value = SoundManager.Instance.GetMusicVolume() * maxVolume;
        soundSlider.value = SoundManager.Instance.GetSFXVolume() * maxVolume;

        if (levelText.gameObject.activeSelf)
        {
            levelText.text = "Level " + (GameplayManager.Instance.chapterData.id + 1).ToString() + "-" + (GameplayManager.Instance.levelData.id + 1).ToString();
        }

        if (hideCor != null)
        {
            StopCoroutine(hideCor);
        }

        uiPopupAnim.Show();
    }
    public void Hide()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowAllButtons();
        }
        uiPopupAnim.Hide();
        hideCor = StartCoroutine(Cor_Hide());
    }
    IEnumerator Cor_Hide()
    {
        yield return new WaitForSeconds(1f);
        gameObject.SetActive(false);
    }
    public void OnMenu()
    {
        GameManager.Instance.LoadSceneManually(
            GDC.Enums.SceneType.MAINMENU,
            GDC.Enums.TransitionType.IN,
            SoundType.NONE,
            cb: () =>
            {
                //    //GDC.Managers.GameManager.Instance.SetInitData(levelIndex);
            },
            true);
    }
    public void OnReplay()
    {
        int currentChapterIndex = GameplayManager.Instance.chapterData.id;
        int currentLevelIndex = GameplayManager.Instance.levelData.id;
        GameManager.Instance.LoadSceneManually(
            GDC.Enums.SceneType.GAMEPLAY,
            GDC.Enums.TransitionType.IN,
            SoundType.NONE,
            cb: () =>
            {
                GDC.Managers.GameManager.Instance.SetInitData(currentChapterIndex, currentLevelIndex);
            },
            true);
    }

    void OnChangeMusicVolume()
    {
        SoundManager.Instance.SetMusicVolume((float)musicSlider.value/maxVolume);
        SoundManager.Instance.PlaySound(AudioPlayer.SoundID.SFX_CLICK_TILE);
    }
    void OnChangeSoundVolume()
    {
        SoundManager.Instance.SetSFXVolume((float)soundSlider.value / maxVolume);
        SoundManager.Instance.PlaySound(AudioPlayer.SoundID.SFX_CLICK_TILE);
    }
}
