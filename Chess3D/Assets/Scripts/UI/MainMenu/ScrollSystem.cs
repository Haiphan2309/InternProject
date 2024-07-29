using UnityEngine;
using UnityEngine.UI;

public class ScrollSystem : MonoBehaviour
{
    private ScrollRect _scrollRect;

    [SerializeField] private ScrollButton _upButton;
    [SerializeField] private ScrollButton _downButton;

    private readonly float _scrollSpeed = 0.075f;

    // Start is called before the first frame update
    void Start()
    {
        _scrollRect = GetComponent<ScrollRect>();
    }

    void Update()
    {
        if (_upButton != null && _upButton.isDown)
        {
            ScrollUp();
        }
        if (_downButton != null && _downButton.isDown)
        {
            ScrollDown();
        }
    }

    public void ScrollUp()
    {
        if (_scrollRect != null)
        {
            if (_scrollRect.verticalNormalizedPosition <= 1f)
            {
                DisableButtons();
                _scrollRect.verticalNormalizedPosition += _scrollSpeed;
                EnableButtons();
            }
        }
    }

    public void ScrollDown()
    {
        if (_scrollRect != null)
        {
            if (_scrollRect.verticalNormalizedPosition >= 0f)
            {
                DisableButtons();
                _scrollRect.verticalNormalizedPosition -= _scrollSpeed;
                EnableButtons();
            }
        }
    }

    private void DisableButtons()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.HideAllButtons();
            _upButton.GetComponent<Button>().enabled = false;
            _downButton.GetComponent<Button>().enabled = false;
        }
    }

    private void EnableButtons()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowAllButtons();
            _upButton.GetComponent<Button>().enabled = true;
            _downButton.GetComponent<Button>().enabled = true;
        }
    }
}
