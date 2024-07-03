using DG.Tweening;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIWinPanel : MonoBehaviour
{
    [SerializeField] UIPopupAnim uiPopupAnim;
    [SerializeField] List<Image> stars, halos;
    [SerializeField] Slider turnSlider;
    [SerializeField] TMP_Text turnText;

    [Button]
    void Show()
    {
        uiPopupAnim.Show();
        //Debug.Log(GameplayManager.Instance.levelData.maxTurn);
        turnSlider.maxValue = GameplayManager.Instance.levelData.maxTurn;
        turnText.text = "Turn: " + GameplayManager.Instance.remainTurn.ToString();
        turnSlider.value = 0;
        foreach (var item in stars)
        {
            DOTween.Kill(item);
            Color fadeBlack = Color.black;
            fadeBlack.a = 0.5f;
            item.color = fadeBlack;
            item.rectTransform.localScale = Vector2.one * 0.9f;
        }
        foreach (var item in halos)
        {
            DOTween.Kill(item);
            item.rectTransform.localScale = Vector2.zero;
        }
        StartCoroutine(Cor_Show());
    }

    IEnumerator Cor_Show()
    {
        yield return new WaitForSeconds(0.5f);
        turnSlider.DOValue(GameplayManager.Instance.remainTurn, 3);
        for (int i=0; i<3; i++)
        {
            stars[i].DOColor(Color.white,0.2f);
            stars[i].rectTransform.DOScale(1, 0.3f).SetEase(Ease.OutBack);
            halos[i].rectTransform.DOScale(1, 0.5f).SetEase(Ease.OutBack);
            yield return new WaitForSeconds(1);
        }
    }
}
