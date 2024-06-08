using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;
using Unity.Netcode;

public interface IActionableObject
{
    void PerformAction();

}

public class PrisonSwitchController : NetworkBehaviour, IActionableObject
{
    [SerializeField] private GameObject lightObject;

    [ColorUsage(true, true)]
    [SerializeField] private Color lightUpColor;

    public UnityEvent OnLightOn;

    private bool _isLightOn;

    public void PerformAction()
    {
        if (_isLightOn)
        {
            Debug.Log($"{GetType().Name} already on, aborting action");
            return;
        }

        Debug.Log($"{GetType().Name} performing action...");

        if (NetworkManager.Singleton != null)
        {
            NetworkTurnOn();
        }
        else
        {
            PerformTurnOn();
            OnLightOn.Invoke();
        }
    }

    private void NetworkTurnOn()
    {
        if (IsServer)
        {
            Debug.Log($"{GetType().Name} is server, aborting");
            return;
        }

        TurnOnServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void TurnOnServerRpc()
    {
        Debug.Log($"{GetType().Name} server turning on light");

        PerformTurnOn();
        OnLightOn.Invoke();

        TurnOnClientRpc();
    }

    [ClientRpc]
    private void TurnOnClientRpc()
    {
        if (IsServer) return;

        Debug.Log($"{GetType().Name} client turning on light");

        PerformTurnOn();
        OnLightOn.Invoke();
    }

    private void PerformTurnOn()
    {
        _isLightOn = true;

        lightObject.GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
        lightObject.GetComponent<Renderer>().material.SetColor("_EmissionColor", lightUpColor);

        if (NetworkSoundManager.Instance != null)
        {
            NetworkSoundManager.Instance.PlaySoundServerRpc("BuzzerSound1", transform.position);
        }
    }
}
