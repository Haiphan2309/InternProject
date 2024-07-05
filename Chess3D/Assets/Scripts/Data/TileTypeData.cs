using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GDC.Enums;
using System;

[Serializable]
public class TileTypeDictionary
{
    public TileType tileType;
    public List<int> idList;
}

[CreateAssetMenu(menuName = "Data/TileTypeData")]
public class TileTypeData : ScriptableObject
{
    public List<TileTypeDictionary> tileTypeDatas;
}
