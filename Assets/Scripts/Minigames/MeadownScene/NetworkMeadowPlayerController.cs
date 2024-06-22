using System.Collections;
using System.Collections.Generic;

using DG.Tweening;

using Unity.Netcode;
using Unity.VisualScripting;

using UnityEngine;
using UnityEngine.InputSystem;

public class NetworkMeadowPlayerController : NetworkBehaviour
{
    [Header("Configuration")]
    [SerializeField] private float overlapSphereRadius = .2f;
    [SerializeField] private InputActionProperty activateAction;

    private MeshRenderer[] _renderers;

    void Start()
    {
        InitializeRenderers();

        activateAction.action.performed += OnActivateActionPerformed;
    }

    #region Networking

    [ServerRpc(RequireOwnership = false)]
    private void VisualizeActivateServerRpc()
    {
        VisualizeActivateClientRpc();
    }

    [ClientRpc]
    private void VisualizeActivateClientRpc()
    {
        if (IsLocalPlayer) return;

        LocalVisualizeActivate();
    }

    #endregion

    #region Events

    private void OnActivateActionPerformed(InputAction.CallbackContext context)
    {
        Debug.Log("[TEST]: player attempted to activate");

        var hasNetworkAccess = NetworkManager.Singleton != null;
        if (hasNetworkAccess)
        {
            if (!IsLocalPlayer) return;
            ScanForHiddenGems();
            LocalVisualizeActivate();
            VisualizeActivateServerRpc();
        }
        else
        {
            ScanForHiddenGems();
            LocalVisualizeActivate();
        }
    }

    #endregion

    #region Private Implementation Details

    private void InitializeRenderers()
    {
        _renderers = GetComponentsInChildren<MeshRenderer>();

        Debug.Log($"[TEST]: found {_renderers.Length} renderers");
    }

    private void LocalVisualizeActivate()
    {
        foreach (var renderer in _renderers)
        {
            renderer.material.DOColor(Color.black, "_EmissionColor", 0.5f).From(Color.white);
        }
    }

    private void ScanForHiddenGems()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, overlapSphereRadius, LayerMask.GetMask("HiddenObjects"));

        if (hitColliders.Length > 0)
        {
            foreach (var hitCollider in hitColliders)
            {
                var hiddenGem = hitCollider.GetComponent<NetworkMeadowHiddenGemController>();
                if (hiddenGem != null)
                {
                    hiddenGem.ExposeSelf();

                    NetworkMeadowGameManager.Instance.OnPlayerDidFindHiddenGem(hiddenGem.NetworkObject);
                }
            }
        }
        else
        {
            Debug.Log("[TEST]: no objects found within sphere");
        }
    }

    #endregion
}
