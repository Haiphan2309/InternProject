using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupManager : MonoBehaviour
{
    public static PopupManager Instance { get; private set; }

    [SerializeField] Transform canvasTrans;
    [SerializeField] UIAnnounce uiAnnouncePrefab;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public void ShowAnnounce(string text)
    {
        UIAnnounce uiAnnounce = Instantiate(uiAnnouncePrefab, canvasTrans);
        uiAnnounce.Show(text);
    }
}
