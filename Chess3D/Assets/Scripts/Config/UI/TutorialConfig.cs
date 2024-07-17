using GDC.Enums;
using RotaryHeart.Lib.SerializableDictionary;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LanguageDictionary : SerializableDictionaryBase<Language, string>
{
}
[Serializable]
public class TutorialData
{
    public int chapterIndex, levelIndex;
    public Sprite tutorialSprite;
    public string tutorialText;
    public LanguageDictionary tutorialDict;
}
[CreateAssetMenu(menuName = "Config/UI/TutorialConfig")]
public class TutorialConfig : ScriptableObject
{
    public List<TutorialData> tutorialDatas;
}
