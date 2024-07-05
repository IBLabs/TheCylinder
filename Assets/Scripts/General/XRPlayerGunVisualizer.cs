using System.Collections;
using System.Collections.Generic;

using UnityEngine;

[RequireComponent(typeof(Animator))]
public class XRPlayerGunVisualizer : MonoBehaviour
{
    [SerializeField] private XRPlayerShooter attachedShooter;
    [SerializeField] private Renderer[] ammoRenderers;

    [ColorUsage(true, true)]
    [SerializeField] private Color loadedColor = Color.red;

    private const string ANIMATOR_TRIGGER_SHOOT = "Shoot";

    private Animator _animator;

    void Start()
    {
        _animator = GetComponent<Animator>();

        UpdateAmmoVisuals();
    }

    void Update()
    {
        if (attachedShooter.Ammo < ammoRenderers.Length)
        {
            SetMeshRendererProgress(attachedShooter.ReloadProgress, ammoRenderers[attachedShooter.Ammo]);
        }
    }

    public void VisualizeShoot()
    {
        _animator.SetTrigger(ANIMATOR_TRIGGER_SHOOT);
    }

    public void OnAmmoChanged(int ammo)
    {
        for (int i = 0; i < ammoRenderers.Length; i++)
        {
            SetMeshRendererActive(i < ammo, ammoRenderers[i]);
        }
    }

    private void UpdateAmmoVisuals()
    {
        OnAmmoChanged(attachedShooter.Ammo);
    }

    private void SetMeshRendererActive(bool isActive, Renderer renderer)
    {
        if (isActive)
        {
            renderer.material.SetColor("_EmissionColor", loadedColor);
        }
        else
        {
            renderer.material.SetColor("_EmissionColor", Color.black);
        }
    }

    private void SetMeshRendererProgress(float progress, Renderer renderer)
    {
        renderer.material.SetColor("_EmissionColor", Color.Lerp(Color.black, loadedColor, progress));
    }
}
