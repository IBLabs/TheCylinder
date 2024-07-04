using System.Collections;
using System.Collections.Generic;

using UnityEngine;

[RequireComponent(typeof(Animator))]
public class XRPlayerGunVisualizer : MonoBehaviour
{
    [SerializeField] private Renderer[] ammoRenderers;

    private const string ANIMATOR_TRIGGER_SHOOT = "Shoot";

    private Animator _animator;

    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    public void VisualizeShoot()
    {
        _animator.SetTrigger(ANIMATOR_TRIGGER_SHOOT);
    }
}
