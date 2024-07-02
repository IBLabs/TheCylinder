using System.Collections;
using System.Collections.Generic;

using Unity.Netcode;
using Unity.VisualScripting;

using UnityEngine;

public class NetworkWallSwitchController : NetworkBehaviour
{
    private const string TRIGGER_ENABLE = "Enable";
    private const string TRIGGER_DISABLE = "Disable";

    [SerializeField] private NetworkWallObstacleController attachedWall;
    [SerializeField] private Animator animator;

    [SerializeField] private float cooldown = 5f;


    private readonly NetworkVariable<bool> _isEnabled = new NetworkVariable<bool>(true);

    [ServerRpc(RequireOwnership = false)]
    private void RequestActivateServerRpc()
    {
        if (!IsServer)
        {
            Debug.LogWarning("Only the server can activate the wall switch");
            return;
        }

        StartCoroutine(ActivateCoroutine());
    }

    private IEnumerator ActivateCoroutine()
    {
        var hasNetworkAccess = NetworkManager.Singleton != null;

        _isEnabled.Value = false;

        attachedWall.SetVisible(true);

        if (hasNetworkAccess) SetSwitchEnableClientRpc(false);
        else LocalSetSwitchEnabled(false);

        yield return new WaitForSeconds(cooldown);

        attachedWall.SetVisible(false);

        if (hasNetworkAccess) SetSwitchEnableClientRpc(true);
        else LocalSetSwitchEnabled(true);

        _isEnabled.Value = true;
    }

    [ClientRpc]
    private void SetSwitchEnableClientRpc(bool isEnabled)
    {
        LocalSetSwitchEnabled(isEnabled);
    }

    private void LocalSetSwitchEnabled(bool isEnabled)
    {
        animator.SetTrigger(isEnabled ? TRIGGER_ENABLE : TRIGGER_DISABLE);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (_isEnabled.Value)
            {
                var hasNetworkAccess = NetworkManager.Singleton != null;
                if (hasNetworkAccess)
                {
                    RequestActivateServerRpc();
                }
                else
                {
                    StartCoroutine(ActivateCoroutine());
                }
            }
        }
    }
}
