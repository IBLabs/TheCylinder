using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class NetworkTransitionCoordinator : NetworkBehaviour
{
    public ITransitionController transitionController;

    public void OnTransitionWillStart(float duration)
    {
        if (IsServer)
        {
            StartOutTransitionClientRpc(duration);
        }
    }

    [ClientRpc]
    private void StartOutTransitionClientRpc(float duration)
    {
        if (IsServer) return;

        transitionController.FadeToBlack();
    }
}