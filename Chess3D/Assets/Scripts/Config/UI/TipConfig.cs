using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TipData
{
    public LanguageDictionary tipDict;
}
[Serializable]
public class TipChapterData
{
    //public int chapterIndex; //Chapter index se duoc xac dinh dua vao index trong tipChapterDatas.
    public List<TipData> tipDatas;
}
[Serializable]
[CreateAssetMenu(menuName = "Config/UI/TipConfig")]
public class TipConfig : ScriptableObject
{
    public List<TipData> commonTipDatas;
    public List<TipChapterData> tipChapterDatas;
}
