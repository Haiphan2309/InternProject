using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using GDC.Enums;

public class AIManager : MonoBehaviour
{
    public static AIManager Instance { get; private set; }
    private List<PlayerArmy> playerArmies;
    private List<EnemyArmy> enemyArmies;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        playerArmies = GameplayManager.Instance.levelData.GetPlayerArmies();
        enemyArmies = GameplayManager.Instance.levelData.GetEnemyArmies();
    }
}
