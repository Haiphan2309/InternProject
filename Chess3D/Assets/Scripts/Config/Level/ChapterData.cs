using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Data/ChapterData")]
public class ChapterData : ScriptableObject
{
    public int id;
    List<LevelData> levelDatas;
    Sprite background;
    int starRequire;
}
