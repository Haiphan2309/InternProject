using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GDC.Managers;

public class SoundSetting : MonoBehaviour
{
    bool alreadySet;
    [SerializeField] Slider SFXSlider, MusicSlider;
    void Start()
    {
        StartCoroutine(Cor_WaitUntilLoad());
    }
    IEnumerator Cor_WaitUntilLoad()
    {
        yield return new WaitUntil(() => SoundManager.Instance.LoadSave());
        SFXSlider.value = SoundManager.Instance.GetSFXVolume() * 10f;
        MusicSlider.value = SoundManager.Instance.GetMusicVolume() * 10f;

        alreadySet = true;
    }
    public void SettingSFXVolume()
    {
        SoundManager.Instance.SetSFXVolume(SFXSlider.value / 10f);
        if (alreadySet) SoundManager.Instance.PlaySound(AudioPlayer.SoundID.BUTTON_CLICK);
    }
    public void SettingMusicVolume()
    {
        var musicVolume = MusicSlider.value / 10f;
        SoundManager.Instance.SetMusicVolume(musicVolume);
        if (alreadySet) SoundManager.Instance.PlaySoundHelper(AudioPlayer.SoundID.BUTTON_CLICK, musicVolume);
    }
    public void MuteSFX()
    {
        if (SoundManager.Instance.GetSFXVolume() == 0)
        {
            SoundManager.Instance.SetSFXVolume(5);
            SFXSlider.value = 5;
            SoundManager.Instance.PlaySound(AudioPlayer.SoundID.BUTTON_CLICK, 0.5f);
        }
        else 
        {
            SoundManager.Instance.SetSFXVolume(0f);
            SFXSlider.value = 0;
        }
    }
    public void MuteMusic()
    {
        if (SoundManager.Instance.GetMusicVolume() == 0)
        {
            SoundManager.Instance.SetMusicVolume(5);
            MusicSlider.value = 5;
            SoundManager.Instance.PlaySoundHelper(AudioPlayer.SoundID.BUTTON_CLICK, 0.5f);
        }
        else 
        {
            SoundManager.Instance.SetMusicVolume(0f);
            MusicSlider.value = 0;
        }
    }
}
