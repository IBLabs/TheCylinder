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
    public float holdTime = 2f;
    public Color normalColor = Color.white;
    public Color hoverColor = Color.gray;
    public float transitionDuration = 0.2f;
    public Graphic[] graphics;

    public UnityEvent OnAction;

    private float _timer;
    private bool _isHeld;
    private bool _didFireAction;

    void Start()
    {
        progressBar.fillAmount = 0f;
        SetGraphicsColor(normalColor);
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
                    sfxAudioSource.DOPitch(.1f, 0.2f).onComplete = () =>
                    {
                        sfxAudioSource.Stop();
                        sfxAudioSource.pitch = 1f;
                    };

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

        sfxAudioSource.clip = holdSound;
        sfxAudioSource.Play();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _isHeld = false;

        sfxAudioSource.DOPitch(.1f, 0.2f).onComplete = () =>
        {
            sfxAudioSource.Stop();
            sfxAudioSource.pitch = 1f;
        };
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SetGraphicsColor(hoverColor);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetGraphicsColor(normalColor);
    }

    private void ButtonAction()
    {
        _didFireAction = true;

        Debug.Log("Button held long enough!");

        OnAction?.Invoke();
    }

    private void SetGraphicsColor(Color targetColor)
    {
        StopAllCoroutines();

        foreach (var graphic in graphics)
        {
            StartCoroutine(FadeToColor(graphic, targetColor, transitionDuration));
        }
    }

    private IEnumerator FadeToColor(Graphic graphic, Color targetColor, float duration)
    {
        Color startColor = graphic.color;
        float time = 0;

        while (time < duration)
        {
            graphic.color = Color.Lerp(startColor, targetColor, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        graphic.color = targetColor;
    }
}