using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoldButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Image progressBar;
    public float holdTime = 2f;

    public UnityEvent OnAction;

    private float _timer;
    private bool _isHeld;
    private bool _didFireAction;

    void Start()
    {
        progressBar.fillAmount = 0f;
    }

    void Update()
    {
        if (!_didFireAction)
        {
            if (_isHeld)
            {
                _timer += Time.deltaTime;

                if (_timer >= holdTime)
                {
                    ButtonAction();
                }
            }
            else if (_timer > 0f)
            {
                _timer -= Time.deltaTime;
            }
        }

        if (!_didFireAction && _timer > 0f) progressBar.fillAmount = _timer / holdTime;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _isHeld = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _isHeld = false;
    }

    private void ButtonAction()
    {
        _didFireAction = true;

        Debug.Log("Button held long enough!");

        OnAction?.Invoke();
    }
}