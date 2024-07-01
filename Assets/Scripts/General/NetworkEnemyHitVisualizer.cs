using System.Collections;
using System.Collections.Generic;

using DG.Tweening;

using Unity.Netcode;

using UnityEngine;

public class NetworkEnemyHitVisualizer : NetworkBehaviour
{
    public delegate void OnHitVisualizationCompleted();

    private Renderer _renderer;

    private OnHitVisualizationCompleted _delegate;

    void Start()
    {
        _renderer = GetComponentInChildren<Renderer>();
    }

    public void RequestVisualizeHit(OnHitVisualizationCompleted onComplete)
    {
        var hasNetworkAccess = NetworkManager.Singleton != null;

        if (!hasNetworkAccess)
        {
            if (onComplete != null) _delegate += onComplete;
            LocalVisualizeHit();
            return;
        }

        if (!IsServer) return;

        if (onComplete != null) _delegate += onComplete;
        VisualizeHitServerRpc();
    }

    [ServerRpc]
    private void VisualizeHitServerRpc()
    {
        VisualizeHitClientRpc();
    }

    [ClientRpc]
    private void VisualizeHitClientRpc()
    {
        LocalVisualizeHit();
    }

    private void LocalVisualizeHit()
    {
        StartCoroutine(LocalVisualizeHitCoroutine());
    }

    private IEnumerator LocalVisualizeHitCoroutine()
    {
        if (_renderer == null) yield break;

        _renderer.material.DOColor(Color.black, "_EmissionColor", 0.4f).From(Color.white);

        // TODO: show thinking animation

        yield return new WaitForSeconds(.5f);

        yield return _renderer.material.DOColor(Color.black, "_EmissionColor", 0.1f).From(Color.red).SetLoops(2).WaitForCompletion();

        if (_delegate != null) _delegate.Invoke();
    }
}
