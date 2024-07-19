using System.Collections;
using System.Collections.Generic;
using System.Linq;

using DG.Tweening;

using Unity.Netcode;
using Unity.VisualScripting;

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

        var targetMaterialsNames = new List<string> { "General_EnemyMat (Instance)", "General_EnemySecondaryMat (Instance)" };

        var targetMaterials = _renderer.materials.Where(m => targetMaterialsNames.Contains(m.name)).ToList();

        Debug.Log("found " + targetMaterials.Count + " materials");
        Debug.Log("materials in renderer materials list: " + string.Join(", ", _renderer.materials.Select(m => m.name).ToArray()));

        for (int i = 0; i < targetMaterials.Count; i++)
        {
            var material = targetMaterials[i];
            var tweener = material.DOColor(Color.black, "_EmissionColor", 0.33f).From(Color.white);
            if (i == targetMaterials.Count - 1)
            {
                yield return tweener.WaitForCompletion();
            }
        }

        if (NetworkSoundManager.Instance != null)
        {
            NetworkSoundManager.Instance.PlaySoundServerRpc("EnemyError1", transform.position);
        }

        // TODO: show thinking animation

        for (int i = 0; i < targetMaterials.Count; i++)
        {
            var material = targetMaterials[i];
            var tweener = material.DOColor(Color.black, "_EmissionColor", 0.1f).From(Color.red).SetLoops(2);
            if (i == targetMaterials.Count - 1)
            {
                yield return tweener.WaitForCompletion();
            }
        }

        if (_delegate != null) _delegate.Invoke();
    }
}
