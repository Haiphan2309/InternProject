using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ShopSlotData
{
    public string slotName;
    public int undoAmount, solveAmount, turnAmount;
    public float costUSD, costVND;
    public Sprite icon, bg;
}
[Serializable]
[CreateAssetMenu(menuName = "Config/ShopConfig")]
public class ShopConfig : ScriptableObject
{
    public int maxDailyAdsLimit;
    public List<ShopSlotData> shopSlotDatas = new List<ShopSlotData>();
}
