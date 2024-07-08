using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TutorialData
{
    public int chapterIndex, levelIndex;
    public Sprite tutorialSprite;
    public string tutorialText;
}
[CreateAssetMenu(menuName = "Config/UI/TutorialConfig")]
public class TutorialConfig : ScriptableObject
{
    public List<TutorialData> tutorialDatas;
}
