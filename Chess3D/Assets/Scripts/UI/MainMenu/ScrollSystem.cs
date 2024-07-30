using UnityEngine;
using UnityEngine.UI;

public class ScrollSystem : MonoBehaviour
{
    private ScrollRect _scrollRect;

    [SerializeField] private ScrollButton _upButton;
    [SerializeField] private ScrollButton _downButton;
    [SerializeField] private ScrollButton _leftButton;
    [SerializeField] private ScrollButton _rightButton;

    private readonly float _scrollSpeed = 0.25f;

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
        if (_leftButton != null && _leftButton.isDown)
        {
            ScrollLeft();
        }
        if (_rightButton != null && _rightButton.isDown)
        {
            ScrollRight();
        }
    }

    public void ScrollUp()
    {
        if (_scrollRect == null) return;
        if (_scrollRect.verticalNormalizedPosition <= 1f)
        {
            DisableButtons();
            _scrollRect.verticalNormalizedPosition += _scrollSpeed;
            EnableButtons();
        }
    }

    public void ScrollDown()
    {
        if (_scrollRect == null) return;
        if (_scrollRect.verticalNormalizedPosition >= 0f)
        {
            DisableButtons();
            _scrollRect.verticalNormalizedPosition -= _scrollSpeed;
            EnableButtons();
        }
    }

    public void ScrollLeft()
    {
        if (_scrollRect == null) return;
        if (_scrollRect.horizontalNormalizedPosition >= 0f)
        {
            DisableButtons();
            _scrollRect.horizontalNormalizedPosition += _scrollSpeed;
            EnableButtons();
        }
    }

    public void ScrollRight()
    {
        if (_scrollRect == null) return;
        if (_scrollRect.horizontalNormalizedPosition <= 1f)
        {
            DisableButtons();
            _scrollRect.horizontalNormalizedPosition -= _scrollSpeed;
            EnableButtons();
        }
    }

    private void DisableButtons()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.HideAllButtons();
        }
        if (_upButton != null) 
            _upButton.GetComponent<Button>().enabled = false;
        if (_downButton != null)
            _downButton.GetComponent<Button>().enabled = false;
        if (_leftButton != null) 
            _leftButton.GetComponent<Button>().enabled = false;
        if (_rightButton != null)
            _rightButton.GetComponent<Button>().enabled = false;
    }

    private void EnableButtons()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowAllButtons();
        }
        if (_upButton != null) 
            _upButton.GetComponent<Button>().enabled = true;
        if (_downButton != null)
            _downButton.GetComponent<Button>().enabled = true;
        if (_leftButton != null) 
            _leftButton.GetComponent<Button>().enabled = true;
        if (_rightButton != null)
            _rightButton.GetComponent<Button>().enabled = true;
    }
}
