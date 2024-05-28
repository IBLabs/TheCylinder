using System;
using System.Collections;
using System.Collections.Generic;

using DG.Tweening;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(MeshRenderer))]
public class TransitionSphereController : MonoBehaviour
{
    private const float BLACK_VALUE = 0f;
    private const float WORDL_VALUE = 1f;

    public float transitionDuration = 1.0f;

    public InputActionProperty toggleTransitionAction;

    [Header("Events")]
    public UnityEvent OnFadeToBlackCompleted;
    public UnityEvent OnFadeToWorldCompleted;

    private MeshRenderer _meshRenderer;

    private bool _isBlack;

    private void Start()
    {
        _meshRenderer = GetComponent<MeshRenderer>();

        _meshRenderer.material.SetFloat("_Progress", WORDL_VALUE);

        FadeToWorld();
    }

    private void OnEnable()
    {
        toggleTransitionAction.action.performed += ToggleTransition;
    }

    private void OnDisable()
    {
        toggleTransitionAction.action.performed -= ToggleTransition;
    }

    public void FadeToBlack()
    {
        _meshRenderer.material.DOFloat(BLACK_VALUE, "_Offset", transitionDuration).From(-1.0f).SetEase(Ease.InCubic).OnComplete(() => OnFadeToBlackCompleted?.Invoke());
        _isBlack = true;
    }

    public IEnumerator FadeToBlackAsync()
    {
        _isBlack = true;
        yield return _meshRenderer.material.DOFloat(BLACK_VALUE, "_Offset", transitionDuration).From(-1.0f).SetEase(Ease.InCubic).WaitForCompletion();
        OnFadeToBlackCompleted?.Invoke();
    }

    public IEnumerator FadeToWorldAsync()
    {
        _isBlack = false;
        yield return _meshRenderer.material.DOFloat(WORDL_VALUE, "_Offset", transitionDuration).From(0f).SetEase(Ease.InCubic).WaitForCompletion();
        OnFadeToWorldCompleted?.Invoke();
    }

    public void FadeToWorld()
    {
        _meshRenderer.material.DOFloat(WORDL_VALUE, "_Offset", transitionDuration).From(0f).SetEase(Ease.InCubic).OnComplete(() => OnFadeToWorldCompleted?.Invoke());
        _isBlack = false;
    }

    #region Private Implementation Details
    // 
    private void ToggleTransition(InputAction.CallbackContext context)
    {
        if (_isBlack)
            FadeToWorld();
        else
            FadeToBlack();
    }

    #endregion
}
