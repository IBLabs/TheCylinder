using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PrisonSettingsController : MonoBehaviour
{
    [SerializeField] private XRPlayerShooter _leftShooter;
    [SerializeField] private Canvas _leftShooterCanvas;
    [SerializeField] private XRPlayerShooter _rightShooter;
    [SerializeField] private Canvas _rightShooterCanvas;
    [SerializeField] private AgentDuplicator _agentDuplicator;

    public void ToggleCooldownEnabled()
    {
        _leftShooter.cooldownEnabled = !_leftShooter.cooldownEnabled;
        _rightShooter.cooldownEnabled = !_rightShooter.cooldownEnabled;

        _leftShooterCanvas.enabled = _leftShooter.cooldownEnabled;
        _rightShooterCanvas.enabled = _rightShooter.cooldownEnabled;
    }

    public void ToggleDuplicateAgents()
    {
        _agentDuplicator.duplicatorEnabled = !_agentDuplicator.duplicatorEnabled;
    }
}
