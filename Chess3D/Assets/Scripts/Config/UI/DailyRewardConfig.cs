using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RewardData
{
    public string rewardName;
    public int undoAmount, solveAmount;
}
[Serializable]
[CreateAssetMenu(menuName = "Config/DailyRewardConfig")]
public class DailyRewardConfig : ScriptableObject
{
    public List<RewardData> rewards;
}
