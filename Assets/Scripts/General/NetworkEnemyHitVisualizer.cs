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

        yield return _renderer.material.DOColor(Color.black, "_EmissionColor", 0.33f).From(Color.white).WaitForCompletion();

        if (NetworkSoundManager.Instance != null)
        {
            NetworkSoundManager.Instance.PlaySoundServerRpc("EnemyError1", transform.position);
        }

        // TODO: show thinking animation

        yield return _renderer.material.DOColor(Color.black, "_EmissionColor", 0.1f).From(Color.red).SetLoops(2).WaitForCompletion();

        if (_delegate != null) _delegate.Invoke();
    }
}
