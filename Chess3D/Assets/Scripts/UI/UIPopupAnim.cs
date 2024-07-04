using DG.Tweening;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupAnim : MonoBehaviour
{
    [SerializeField] RectTransform panelRect;
    //[SerializeField] List<Image> images = new List<Image>();
    [SerializeField] List<Color> imageOriginColor, textOriginColor, tmpTextOriginColor;

    [Button]
    public void Show()
    {
        DOTween.Kill(panelRect);

        panelRect.localScale = Vector2.zero;

        Image[] images = panelRect.GetComponentsInChildren<Image>();
        if (images != null)
        {
            for (int i = 0; i < images.Length; i++)
            {
                DOTween.Kill(images[i]);

                Color colorClear = images[i].color;
                colorClear.a = 0;
                images[i].color = colorClear;
                images[i].DOFade(imageOriginColor[i].a, 0.5f);
            }
        }

        Text[] texts = panelRect.GetComponentsInChildren<Text>();
        if (texts != null)
        {
            for (int i =0;i<texts.Length;i++)
            {
                DOTween.Kill(texts[i]);

                Color colorClear = texts[i].color;
                colorClear.a = 0;
                texts[i].color = colorClear;
                texts[i].DOFade(textOriginColor[i].a, 0.5f);
            }
        }

        TMP_Text[] tmpTexts = panelRect.GetComponentsInChildren<TMP_Text>();
        if (tmpTexts != null)
        {
            for (int i = 0; i < tmpTexts.Length; i++)
            {
                DOTween.Kill(tmpTexts[i]);

                Color colorClear = tmpTexts[i].color;
                colorClear.a = 0;
                tmpTexts[i].color = colorClear;
                tmpTexts[i].DOFade(tmpTextOriginColor[i].a, 0.5f);
            }
        }

        panelRect.DOScale(1, 0.5f).SetEase(Ease.OutBack);
    }
    [Button]
    public void Hide()
    {
        UIManager.Instance.ShowAllButtons();
        DOTween.Kill(panelRect);

        Image[] images = panelRect.GetComponentsInChildren<Image>();
        if (images != null)
        {
            foreach (var image in images)
            {
                DOTween.Kill(image);
                image.DOFade(0, 0.5f);
            }
        }

        Text[] texts = panelRect.GetComponentsInChildren<Text>();
        if (texts != null)
        {
            foreach (var text in texts)
            {
                DOTween.Kill(text);
                text.DOFade(0, 0.5f);
            }
        }

        TMP_Text[] tmpTexts = panelRect.GetComponentsInChildren<TMP_Text>();
        if (tmpTexts != null)
        {
            foreach (var text in tmpTexts)
            {
                DOTween.Kill(text);
                text.DOFade(0, 0.5f);
            }
        }
        panelRect.DOScale(0, 0.5f).SetEase(Ease.InBack);
    }

    [Button]
    public void ReloadOriginColor()
    {
        if (imageOriginColor == null) imageOriginColor = new List<Color>();
        imageOriginColor.Clear();
        Image[] images = panelRect.GetComponentsInChildren<Image>();
        foreach (var image in images)
        {
            imageOriginColor.Add(image.color);
        }

        if (textOriginColor == null) textOriginColor = new List<Color>();
        textOriginColor.Clear();
        Text[] texts = panelRect.GetComponentsInChildren<Text>();
        foreach (var text in texts)
        {
            textOriginColor.Add(text.color);
        }
        
        if (tmpTextOriginColor == null) tmpTextOriginColor = new List<Color>();
        tmpTextOriginColor.Clear();
        TMP_Text[] tmpTexts = panelRect.GetComponentsInChildren<TMP_Text>();
        foreach (var text in texts)
        {
            tmpTextOriginColor.Add(text.color);
        }
    }
    //void KillThisDotween()
    //{
    //    DOTween.Kill(panelRect);
    //    foreach (Image image in images)
    //    {
    //        DOTween.Kill(image);
    //    }
    //}
}
