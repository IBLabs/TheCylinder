using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BGMController : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;

    [SerializeField] private float delay = 5f;

    private void OnEnable()
    {
        StartCoroutine(OnEnableCoroutine());
    }

    private IEnumerator OnEnableCoroutine()
    {
        yield return new WaitForSeconds(delay);

        audioSource.Play();
        audioSource.DOFade(1f, 1f).From(0f).SetEase(Ease.Linear);
    }
}
