using DG.Tweening;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UIGameplaySlider : MonoBehaviour
{
 
    [SerializeField] RectTransform star1;
    [SerializeField] RectTransform star2;
    [SerializeField] RectTransform star3;



    public Sprite starOffSprite;
    public Sprite starOnSprite;

    Slider slider;

    RectTransform[] stars;
    bool[] starStatus;
    float maxSliderBar; // Kich thuoc toi da cua bar -> dung de chinh position cho star
    int currentStars;

    [Button]
    public void Setup()
    {
        // setup stat
        currentStars = 3;
        maxSliderBar = 500;
        //Setup Slider
        slider = gameObject.GetComponent<Slider>();
      
        // Setup star
        stars = new RectTransform[3] {star1, star2, star3};
        starStatus = new bool[3] {true, true, true};
        
        SetStarStatus(starStatus);
    }
    
    void SetStarStatus(int number,bool status)
    {
        starStatus[number] = status;
        if (status == true)
        {
            stars[number].gameObject.GetComponent<Image>().sprite = starOnSprite;
        }
        else
        {
            stars[number].gameObject.GetComponent<Image>().sprite = starOffSprite;
        }
    }
    void SetStarStatus(bool[] status)
    {
        
        if (status.Length > 3 || status.Length < 0)
        {
            Debug.LogError("Error in SetStarStatus: Length of array must in range [1,3]");
        }
        for(int i = 0; i < status.Length; i++)
        {
            SetStarStatus(i, status[i]);
        }
    }

    bool GetStarStatus(int number)
    {
        return starStatus[number];
    }

    void SetStarPosition(int index, float pos)
    {

    }
    

    [Button]
    public void OnValueChanged()
    {
        
        for(int i = 0; i < stars.Length; i++)
        {
            bool status = stars[i].anchoredPosition.x < slider.value * maxSliderBar;
            if (status != GetStarStatus(i))
            { 
                SetStarStatus(i, status);
            }
            
        }
    }

    public void ChangeValueFromTurn(int turn, int maxTurn)
    {
        
        slider.value = (float)turn / maxTurn;
        
    }
}
