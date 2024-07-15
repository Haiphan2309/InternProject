using DG.Tweening;
using GDC.Managers;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIInformationPanel : MonoBehaviour
{
    public UIGameplaySlider uIGameplaySlider;
    [SerializeField] TMP_Text turnRemainText;
    [SerializeField] TMP_Text highScoreText;
    [SerializeField] TMP_Text turnText;

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
        turnRemainText.text = "Turn: " + turn.ToString();
        uIGameplaySlider.ChangeValueFromTurn(turn, GameplayManager.Instance.levelData.maxTurn);
    }

    public void SetUIHighScore(int score)
    {
        highScoreText.text = "High Score: " + score.ToString();
    }

    public void ChangeToPlayerTurn()
    {
        turnText.text = "Player Turn!";
        turnText.color = Color.blue;
        PlayTextAnim();
    }

    public void ChangeToEnemyTurn()
    {
        turnText.text = "Enemy Turn!";
        turnText.color = Color.red;
        PlayTextAnim();    
    }

    [Button]
    private void PlayTextAnim()
    {
        DOTween.Kill(turnText);
        turnText.rectTransform.DOScale(1.3f, 0.2f)
            .SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                turnText.rectTransform.DOScale(1f, 0.2f)
                    .SetEase(Ease.OutBounce);
            });
           
        
    }

}
