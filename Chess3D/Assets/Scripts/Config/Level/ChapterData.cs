using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Data/ChapterData")]
[Serializable]
public class ChapterData : ScriptableObject
{
    public int id;
    //public string chapterName;
    public List<LevelData> levelDatas;
    public Sprite thumbnail;
    public Material skyBox;
    public int starRequire;
}
