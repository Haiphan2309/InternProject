using DG.Tweening;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIChessManPanel : MonoBehaviour
{
    [SerializeField] float turnOnPos = -220f;
    [SerializeField] float turnOffPos = 180f;
    //
    public float duration = 1f;



    RectTransform rectTransform;

    [Button]
    public void Setup()
    {
        rectTransform = GetComponent<RectTransform>();
        // For testing only
        

        //
    }

    [Button]
    public void TurnOn()
    {
        //
 
        // Move Panel to Game View
        rectTransform.DOAnchorPosX(turnOnPos, duration).SetEase(Ease.OutBack);

    }

    [Button]
    public void TurnOff()
    {
        //
       
        // Move Panel out of Game View
        rectTransform.DOAnchorPosX(turnOffPos, duration).SetEase(Ease.InBack);

    }
}
