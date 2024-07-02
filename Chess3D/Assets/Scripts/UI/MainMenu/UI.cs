using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour
{
    public RectTransform title;

    public RectTransform topSlider;
    public RectTransform topChessHolder;
    public RectTransform topChessContainer;

    public RectTransform bottomSlider;
    public RectTransform bottomChessHolder;
    public RectTransform bottomChessContainer;

    public RectTransform startButton;
    public RectTransform settingButton;
    public RectTransform returnButton;

    public float _timer = 1f;

    public virtual void Preset()
    {
        title = UIManager.Instance.backgroundSystem.Find("Title") as RectTransform;

        topSlider = UIManager.Instance.backgroundSystem.Find("TopSlider") as RectTransform;
        topChessHolder = topSlider.Find("ChessHolder") as RectTransform;
        topChessContainer = topChessHolder.Find("ChessContainer") as RectTransform;

        bottomSlider = UIManager.Instance.backgroundSystem.Find("BottomSlider") as RectTransform;
        bottomChessHolder = bottomSlider.Find("ChessHolder") as RectTransform;
        bottomChessContainer = bottomChessHolder.Find("ChessContainer") as RectTransform;

        startButton = UIManager.Instance.buttonSystem.Find("StartButton") as RectTransform;
        settingButton = UIManager.Instance.buttonSystem.Find("SettingButton") as RectTransform;
        returnButton = UIManager.Instance.buttonSystem.Find("ReturnButton") as RectTransform;
    }

    public virtual void Anim()
    {

    }
}
