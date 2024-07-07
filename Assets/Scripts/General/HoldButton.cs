using System.Collections;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using DG.Tweening;

public class HoldButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Image progressBar;
    public AudioSource sfxAudioSource;
    public AudioClip holdSound;
    public AudioClip hoverSound;
    public float holdTime = 2f;
    public Color normalColor = Color.white;
    public Color hoverColor = Color.gray;

    [SerializeField] private RectTransform outlineTransform;

    public UnityEvent OnAction;

    private float _timer;
    private bool _isButtonHeld;
    private bool _didInvokeAction;

    void Start()
    {
        progressBar.fillAmount = 0f;

        SetHover(false);
    }

    void Update()
    {
        if (!_didInvokeAction)
        {
            HandleButton();
        }

        UpdateProgressBar();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _isButtonHeld = true;

        sfxAudioSource.clip = holdSound;
        sfxAudioSource.Play();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _isButtonHeld = false;

        PitchDownSFX();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        sfxAudioSource.PlayOneShot(hoverSound);

        SetHover(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetHover(false);
    }

    private void SetHover(bool isHover)
    {
        float animationDuration = .2f;

        float targetZ = isHover ? -.25f : 0f;
        float targetAlpha = isHover ? 1f : 0f;

        outlineTransform.DOLocalMoveZ(targetZ, animationDuration).SetEase(Ease.OutBack);

        if (outlineTransform != null && outlineTransform.TryGetComponent<UIOutline>(out var outline))
        {
            outline.DOColor(isHover ? hoverColor : normalColor, animationDuration);
        }
    }

    private void HandleButton()
    {
        if (_isButtonHeld)
        {
            _timer += Time.deltaTime;

            if (_timer >= holdTime)
            {
                _timer = holdTime;

                PitchDownSFX();
                InvokeAction();
            }
        }
        else if (_timer > 0f)
        {
            _timer -= Time.deltaTime;
            _timer = Mathf.Max(_timer, 0f);
        }
    }

    private void UpdateProgressBar()
    {
        if (_didInvokeAction) return;

        progressBar.fillAmount = _timer / holdTime;
    }

    private void InvokeAction()
    {
        _didInvokeAction = true;

        Debug.Log("Button held long enough!");

        OnAction?.Invoke();
    }

    private void PitchDownSFX()
    {
        sfxAudioSource.DOPitch(.1f, 0.2f).onComplete = () =>
        {
            sfxAudioSource.Stop();
            sfxAudioSource.pitch = 1f;
        };
    }
}