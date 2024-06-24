using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GDC.Configuration
{
    [CreateAssetMenu(menuName = "Scriptable Object/Patrol Config", fileName = "Patrol Config")]
    public class StreetPatrolConfig : ScriptableObject
    {
        public Vector2 MinPivot, MaxPivot;
        public float MinWaitingTime, MaxWaitingTime;
        public float MinMoveSpeed, MaxMoveSpeed;
    }
}
