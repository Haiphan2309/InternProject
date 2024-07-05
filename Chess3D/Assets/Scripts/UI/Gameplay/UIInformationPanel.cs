using GDC.Managers;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIInformationPanel : MonoBehaviour
{
    public UIGameplaySlider uIGameplaySlider;
    [SerializeField] TMP_Text turnText;
    [SerializeField] TMP_Text highScoreText;


    public void Setup()
    {
        Debug.Log("Setup info");
        int chapterId = GameplayManager.Instance.chapterData.id;
        int levelId = GameplayManager.Instance.levelData.id;
        uIGameplaySlider.Setup();
        SetUITurn(GameplayManager.Instance.levelData.maxTurn);
        SetUIHighScore(SaveLoadManager.Instance.GameData.GetLevelHighScore(chapterId, levelId));
    }

   
    public void SetUITurn(int turn)
    {
        turnText.text = "Turn: " + turn.ToString();
        uIGameplaySlider.ChangeValueFromTurn(turn, GameplayManager.Instance.levelData.maxTurn);
    }

    public void SetUIHighScore(int score)
    {
        highScoreText.text = "High Score: " + score.ToString();
    }

}
