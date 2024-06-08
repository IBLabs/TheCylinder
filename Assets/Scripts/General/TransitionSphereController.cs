using System;
using System.Collections;
using System.Collections.Generic;

using DG.Tweening;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(MeshRenderer))]
public class TransitionSphereController : ITransitionController
{
    private const float BLACK_VALUE = 0f;
    private const float WORDL_VALUE = 1f;

    public float transitionDuration = 1.0f;
    public bool transitionOnStart = true;

    [Header("Events")]
    public UnityEvent OnFadeToBlackCompleted;
    public UnityEvent OnFadeToWorldCompleted;

    private MeshRenderer _meshRenderer;


    private void Start()
    {
        _meshRenderer = GetComponent<MeshRenderer>();

        _meshRenderer.material.SetFloat("_Progress", WORDL_VALUE);

        if (transitionOnStart) FadeToScene();
    }

    public override void FadeToBlack()
    {
        _meshRenderer.material.DOFloat(BLACK_VALUE, "_Offset", transitionDuration).From(-1.0f).SetEase(Ease.InCubic).OnComplete(() => OnFadeToBlackCompleted?.Invoke());
    }

    public IEnumerator FadeToBlackAsync()
    {
        yield return _meshRenderer.material.DOFloat(BLACK_VALUE, "_Offset", transitionDuration).From(-1.0f).SetEase(Ease.InCubic).WaitForCompletion();
        OnFadeToBlackCompleted?.Invoke();
    }

    public IEnumerator FadeToWorldAsync()
    {
        yield return _meshRenderer.material.DOFloat(WORDL_VALUE, "_Offset", transitionDuration).From(0f).SetEase(Ease.InCubic).WaitForCompletion();
        OnFadeToWorldCompleted?.Invoke();
    }

    public override void FadeToScene()
    {
        _meshRenderer.material.DOFloat(WORDL_VALUE, "_Offset", transitionDuration).From(0f).SetEase(Ease.InCubic).OnComplete(() => OnFadeToWorldCompleted?.Invoke());
    }

}
