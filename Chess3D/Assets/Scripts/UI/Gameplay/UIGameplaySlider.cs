using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UIGameplaySlider : MonoBehaviour
{

    public Sprite starOffSprite;
    public Sprite starOnSprite;

    Slider slider;
    
    GameObject[] stars;
    bool[] starStatus;
    float maxSliderBar; // Kich thuoc toi da cua bar -> dung de chinh position cho star
    int currentStars;

    [Button]
    public void Setup()
    {
        slider = gameObject.GetComponent<Slider>();
        //

        //
        stars = new GameObject[3];
        starStatus = new bool[3] {true, true, true};
        currentStars = 3;
        maxSliderBar = 500;
        SetStarStatus(starStatus);
    }
    
    void SetStarStatus(int number,bool status)
    {
        starStatus[number] = status;
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

    void SetStartPosition(int index, float pos)
    {

    }
    public void LoseStar()
    {
        SetStarStatus(currentStars-1, false);
        currentStars--;
    }
}
