using DG.Tweening;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMainMenuManager : MonoBehaviour
{
    [SerializeField] RectTransform topSlider;
    [SerializeField] RectTransform bottomSlider;

    [Button]
    private void Anim()
    {
        topSlider.anchoredPosition = Vector2.up * 600;
        bottomSlider.anchoredPosition = Vector2.down * 600;
        topSlider.DOAnchorPosY(0, 1f);
        bottomSlider.DOAnchorPosY(0, 1f);
    }
}
