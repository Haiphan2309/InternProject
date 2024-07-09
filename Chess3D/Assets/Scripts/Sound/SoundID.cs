using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace AudioPlayer
{
    [Serializable]
    public enum SoundID
    {
        NONE = -1,
        _____COMMON_____ = 0,
        TRANSITION_IN = 3,
        TRANSITION_OUT = 4,

        //MUSIC
        TEST_MUSIC = 99,
        _____MAIN_MENU_____ = 100,
        MUSIC_MAIN_MENU,
        
        //SOUND EFFECT
        _____GAMEPLAY________ = 400,
        SFX_TOUCH_CHESSMAN,

        _____UI_____________= 500,
        SFX_BUTTON_CLICK
    }
}