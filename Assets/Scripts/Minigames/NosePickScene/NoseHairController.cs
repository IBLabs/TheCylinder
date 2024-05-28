using System.Collections;
using System.Collections.Generic;

using DG.Tweening;

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class NoseHairController : MonoBehaviour
{
    [SerializeField] private Rigidbody targetRigidbody;

    private Tweener _hairMoveTweener;

    private float _moveDuration = .2f;
    private float _moveDurationRandomOffset = .5f;

    void Start()
    {
        StartHairMovement();
    }

    private void OnDestroy()
    {
        if (_hairMoveTweener != null)
        {
            _hairMoveTweener.Kill();
        }
    }

    public void OnSelectEntered(SelectEnterEventArgs args)
    {
        if (_hairMoveTweener != null)
        {
            _hairMoveTweener.Kill();
        }
    }

    public void OnSelectExit(SelectExitEventArgs args)
    {
        targetRigidbody.isKinematic = false;
    }

    #region Private Implementation Details

    private void StartHairMovement()
    {
        float targetDuration = Random.Range(_moveDuration, _moveDuration + _moveDurationRandomOffset);
        _hairMoveTweener = transform.DOMoveY(0.1f * transform.localScale.y, targetDuration).SetRelative(true).SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }

    #endregion
}