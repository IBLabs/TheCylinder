using System.Collections;
using System.Collections.Generic;

using Unity.Netcode;

using UnityEngine;

public class NetworkWallObstacleController : NetworkBehaviour
{
    private const string TRIGGER_SHOW = "Show";
    private const string TRIGGER_HIDE = "Hide";

    [SerializeField] private Animator animator;

    void Start()
    {
        ConfigureAnimator();
    }

    public void SetVisible(bool isVisible)
    {
        var hasNetworkAccess = NetworkManager.Singleton != null;
        if (hasNetworkAccess)
        {
            if (!IsServer)
            {
                Debug.LogWarning("Only the server can change the wall's visibility");
                return;
            }

            SetVisibleServerRpc(isVisible);
        }
        else
        {
            animator.SetTrigger(isVisible ? TRIGGER_SHOW : TRIGGER_HIDE);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetVisibleServerRpc(bool isVisible)
    {
        SetVisibleClientRpc(isVisible);
    }

    [ClientRpc]
    private void SetVisibleClientRpc(bool isVisible)
    {
        animator.SetTrigger(isVisible ? TRIGGER_SHOW : TRIGGER_HIDE);
    }

    private void ConfigureAnimator()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogError("No animator found on " + gameObject.name);
                return;
            }
        }
    }
}
