using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AudioPlayer;
using GDC.Managers;
using NaughtyAttributes;

public class PlayBackgroundMusic : MonoBehaviour
{
    [SerializeField] bool introMusic;
    [SerializeField, ShowIf("introMusic")] SoundID backgroundIntroMusic;
    [SerializeField] SoundID backgroundMusic;
    void Start()
    {
        var sound = SoundManager.Instance;
        if (introMusic)
        {
            sound.PlayMusicWithIntro(backgroundIntroMusic, backgroundMusic);
        }
        else 
        {
            var audioSource = sound.PlayMusic(backgroundMusic);
            if (audioSource != null) audioSource.loop = true;
        }
    }
}
