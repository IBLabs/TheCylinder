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

    public UnityEvent OnLightOn;

    public void PerformAction()
    {
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
        lightObject.GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
        lightObject.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.white);
    }
}
