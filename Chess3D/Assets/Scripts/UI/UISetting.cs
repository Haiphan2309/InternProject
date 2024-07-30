using GDC.Enums;
using GDC.Managers;
using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISetting : MonoBehaviour
{
    [SerializeField] private UIPopupAnim uiPopupAnim;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] RectTransform bottomGroupRect;
    [SerializeField] private Button menuBtn, replayBtn, hideButton, unlockButton;
    [SerializeField] private Slider musicSlider, soundSlider, cameraSpeedSlider;
    [SerializeField] private TMP_Dropdown languageDropdown;
    private Coroutine hideCor;
    [SerializeField] private int maxVolume = 10;
    private bool isAreadySetup;

    [Button]
    public void Show(bool isGameplay = false)
    {
        gameObject.SetActive(true);
        if (isGameplay)
        {
            bottomGroupRect.gameObject.SetActive(true);
            levelText.gameObject.SetActive(true);
            uiPopupAnim.ReloadOriginImageColor();
        }
        else
        {
            bottomGroupRect.gameObject.SetActive(false);
        }

        if (isAreadySetup == false)
        {
            isAreadySetup = true;
            Setup();
        }

        if (hideCor != null)
        {
            StopCoroutine(hideCor);
        }

        uiPopupAnim.Show();
    }
    private void Setup()
    {
        menuBtn.onClick.AddListener(OnMenu);
        replayBtn.onClick.AddListener(OnReplay);
        hideButton.onClick.AddListener(Hide);
        unlockButton.onClick.AddListener(UnlockAll);

        musicSlider.onValueChanged.AddListener(delegate { OnChangeMusicVolume(); });
        soundSlider.onValueChanged.AddListener(delegate { OnChangeSoundVolume(); });
        cameraSpeedSlider.onValueChanged.AddListener(delegate { OnChangeCameraSpeed(); });
        languageDropdown.onValueChanged.AddListener(delegate { OnChangeLanguage(); });

        musicSlider.maxValue = maxVolume;
        soundSlider.maxValue = maxVolume;
        cameraSpeedSlider.maxValue = maxVolume;
        musicSlider.value = SoundManager.Instance.GetMusicVolume() * maxVolume;
        soundSlider.value = SoundManager.Instance.GetSFXVolume() * maxVolume;
        cameraSpeedSlider.value = PlayerPrefs.GetInt("CameraTargetSpeed", 8);

        if (levelText.gameObject.activeSelf)
        {
            levelText.text = "Level " + (GameplayManager.Instance.chapterData.id + 1).ToString() + "-" + (GameplayManager.Instance.levelData.id + 1).ToString();
        }

        languageDropdown.ClearOptions();
        List<string> optionDatas = new List<string>();
        foreach (var enumValue in Enum.GetValues(typeof(Language)).Cast<Language>())
        {
            optionDatas.Add(enumValue.ToString());
        }
        languageDropdown.AddOptions(optionDatas);
        languageDropdown.value = (int)SaveLoadManager.Instance.GameData.language;
        //Debug.Log("SETUP " + languageDropdown.value + SaveLoadManager.Instance.GameData.language);
    }
    public void Hide()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowAllButtons();
        }
        menuBtn.onClick.RemoveAllListeners();
        replayBtn.onClick.RemoveAllListeners();
        hideButton.onClick.RemoveAllListeners();
        unlockButton.onClick.RemoveAllListeners();

        musicSlider.onValueChanged.RemoveAllListeners();
        soundSlider.onValueChanged.RemoveAllListeners();
        cameraSpeedSlider.onValueChanged.RemoveAllListeners();
        languageDropdown.onValueChanged.RemoveAllListeners();
        uiPopupAnim.Hide();
        PopupManager.Instance.HideBlackBg();
        //hideCor = StartCoroutine(Cor_Hide());
    }
    //private IEnumerator Cor_Hide()
    //{
    //    yield return new WaitForSeconds(1f);
        
    //}
    public void OnMenu()
    {
        int curChapterIndex = GameplayManager.Instance.chapterData.id;
        GameManager.Instance.LoadSceneManually(
            GDC.Enums.SceneType.MAINMENU,
            GDC.Enums.TransitionType.IN,
            SoundType.NONE,
            cb: () =>
            {
                //    //GDC.Managers.GameManager.Instance.SetInitData(levelIndex);
                GameManager.Instance.LoadMenuLevel(curChapterIndex);
            },
            true);
        Hide();
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
        Hide();
    }

    private void OnChangeMusicVolume()
    {
        //SoundManager.Instance.PlayMusic(AudioPlayer.SoundID.GAMEPLAY_1, (float)musicSlider.value / maxVolume);
        SoundManager.Instance.SetMusicVolume((float)musicSlider.value/maxVolume);
        SoundManager.Instance.PlaySound(AudioPlayer.SoundID.SFX_CLICK_TILE);
    }
    private void OnChangeSoundVolume()
    {
        SoundManager.Instance.SetSFXVolume((float)soundSlider.value / maxVolume);
        SoundManager.Instance.PlaySound(AudioPlayer.SoundID.SFX_CLICK_TILE);
    }
    private void OnChangeLanguage()
    {
        //Debug.Log("new value " + languageDropdown.value);
        SoundManager.Instance.PlaySound(AudioPlayer.SoundID.SFX_BUTTON_CLICK);
        SaveLoadManager.Instance.GameData.language = (Language)(languageDropdown.value);
        SaveLoadManager.Instance.Save();
    }

    private void OnChangeCameraSpeed()
    {
        SoundManager.Instance.PlaySound(AudioPlayer.SoundID.SFX_CLICK_TILE);
        if (GameplayManager.Instance != null)
        {
            GameplayManager.Instance.camController.ChangeTargetSpeedValue((int)cameraSpeedSlider.value);
        }
        PlayerPrefs.SetInt("CameraTargetSpeed", (int)cameraSpeedSlider.value);
    }
    private void UnlockAll()
    {
        if (UIManager.Instance) UIManager.Instance.UIReset();
    }    
}
