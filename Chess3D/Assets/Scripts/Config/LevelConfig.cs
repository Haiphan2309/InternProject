using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GDC.Configuration
{
    [CreateAssetMenu(fileName = "Level Config", menuName = "Scriptable Objects/Level Config")]
    public class LevelConfig : ScriptableObject
    {
        [SerializeField] List<int> expToNextLevel = new List<int>();

        public int ExpToNextLevel(int currentLevel)
        {
            if (currentLevel >= expToNextLevel.Count || currentLevel < 1)
            {
                return -1;
            }
            return expToNextLevel[currentLevel - 1];
        }
    }
}