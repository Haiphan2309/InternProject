using System.Collections.Generic;
using UnityEngine;
using System;
using GDC.Configuration;
using GDC.Enums;
using GDC.Constants;

namespace GDC.Managers
{

    [Serializable]
    public struct GameData
    {
       
        public void SetupData() //load
        {
            
        }
        public GameDataOrigin ConvertToGameDataOrigin() //save
        {
            GameDataOrigin gameDataOrigin = new GameDataOrigin();
            return gameDataOrigin;
        }

    }

    [Serializable]
    public struct GameDataOrigin
    {
       
    }

    [Serializable]
    public struct CacheData
    {

    }
}
