using GDC.Managers;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISetting : MonoBehaviour
{
    [SerializeField] UIPopupAnim uiPopupAnim;
    [SerializeField] Button menuBtn, replayBtn, hideButton;
    Coroutine hideCor;

    [Button]
    void Show()
    {
        gameObject.SetActive(true);
        menuBtn.onClick.AddListener(OnMenu);
        replayBtn.onClick.AddListener(OnReplay);
        hideButton.onClick.AddListener(Hide);

        if (hideCor != null)
        {
            StopCoroutine(hideCor);
        }

        uiPopupAnim.Show();
    }
    void Hide()
    {
        uiPopupAnim.Hide();
        hideCor = StartCoroutine(Cor_Hide());
    }
    IEnumerator Cor_Hide()
    {
        yield return new WaitForSeconds(1f);
        gameObject.SetActive(false);
    }
    void OnMenu()
    {
        GameManager.Instance.LoadSceneManually(
            GDC.Enums.SceneType.MAINMENU,
            GDC.Enums.TransitionType.IN,
            SoundType.NONE,
            cb: () =>
            {
                //    //GDC.Managers.GameManager.Instance.SetInitData(levelIndex);
            },
            true);
    }
    void OnReplay()
    {
        int currentLevelIndex = GameplayManager.Instance.levelIndex;
        GameManager.Instance.LoadSceneManually(
            GDC.Enums.SceneType.GAMEPLAY,
            GDC.Enums.TransitionType.IN,
            SoundType.NONE,
            cb: () =>
            {
                GDC.Managers.GameManager.Instance.SetInitData(currentLevelIndex);
            },
            true);
    }
}
