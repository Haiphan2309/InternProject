using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UITutorial : MonoBehaviour
{
    [SerializeField] UIPopupAnim uiPopupAnim;
    [SerializeField] Image tutorialImage;
    [SerializeField] TMP_Text tutorialText;
    Queue<TutorialData> tutorialDataQueue;

    public void Show()
    {
        if (tutorialDataQueue == null || tutorialDataQueue.Count == 0) return;
        //Debug.Log("Tutorial data queue count: " + tutorialDataQueue.Count);
        gameObject.SetActive(true);

        TutorialData tutorialData = tutorialDataQueue.Dequeue();
        tutorialImage.sprite = tutorialData.tutorialSprite;
        tutorialText.text = tutorialData.tutorialText;

        uiPopupAnim.Show();  
    }
    public void Hide()
    {
        uiPopupAnim.Hide();
        if (tutorialDataQueue.Count > 0 )
        {
            StartCoroutine(Cor_Show());
        }
    }
    IEnumerator Cor_Show()
    {
        yield return new WaitForSeconds(0.5f);
        DOTween.KillAll();
        Show();
    }
    public void EnqueueTutorial(TutorialData tutorialData)
    {
        if (tutorialDataQueue == null)
        {
            tutorialDataQueue = new Queue<TutorialData>();
        }
        tutorialDataQueue.Enqueue(tutorialData);
    }    
}
