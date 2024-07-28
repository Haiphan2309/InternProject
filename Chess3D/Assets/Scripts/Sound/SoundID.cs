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

        //MUSIC
        _____MAIN_MENU_____ = 100, //ko goi cai nay
        MUSIC_MAIN_MENU,

        _____GAMEPLAY______ = 150, //ko goi cai nay
        GAMEPLAY_1,
        GAMEPLAY_2,
        GAMEPLAY_3,
        GAMEPLAY_4,
        GAMEPLAY_5,
        
        //SOUND EFFECT
        _____GAMEPLAY________ = 400, //ko goi cai nay
        SFX_CLICK_CHESSMAN,
        SFX_CLICK_TILE,
        SFX_DISAPPEAR,
        SFX_WATER_SPLASH,
        SFX_WIN,
        SFX_LOSE,
        SFX_PHONG,

        _____UI_____________= 500, //ko goi cai nay
        SFX_BUTTON_CLICK,
        SFX_UI_SHOW,
        SFX_UI_LOCK,
        SFX_TRANSITION_IN,
        SFX_TRANSITION_OUT,
        SFX_STAR,
        SFX_PURCHASE,
        SFX_OPEN_CHEST,
    }
}