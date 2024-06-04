using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class XRPlayerShooterCooldownVisualizer : MonoBehaviour
{
    [SerializeField] private Slider cooldownSlider;

    private XRPlayerShooter _shooter;

    void Start()
    {
        _shooter = GetComponent<XRPlayerShooter>();
    }

    void Update()
    {
        cooldownSlider.value = _shooter.normalizedCooldownTimer;
    }
}
