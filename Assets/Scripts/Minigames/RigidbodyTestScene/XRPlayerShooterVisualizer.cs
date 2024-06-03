using UnityEngine;
using DG.Tweening;
using Unity.Netcode;

public class XRPlayerShooterVisualizer : NetworkBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;

    public void VisualizeShot(Vector3 origin, Vector3 direction)
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkVisualizeShot(origin, direction);
        }
        else
        {
            PerformVisualizeShot(origin, direction);
        }
    }

    private void NetworkVisualizeShot(Vector3 origin, Vector3 direction)
    {
        if (!IsServer)
        {
            Debug.Log("only server is allowed to visualize shots");
            return;
        }

        Debug.Log($"{GetType().Name} server visualizing shot...");

        PerformVisualizeShot(origin, direction);

        VisualizeShotClientRpc(origin, direction);
    }

    [ClientRpc]
    private void VisualizeShotClientRpc(Vector3 origin, Vector3 direction)
    {
        Debug.Log($"{GetType().Name} client visualizing shot...");

        PerformVisualizeShot(origin, direction);
    }

    private void PerformVisualizeShot(Vector3 origin, Vector3 direction)
    {
        lineRenderer.SetPosition(0, origin);
        lineRenderer.SetPosition(1, origin + direction * 100f);

        lineRenderer.enabled = true;

        DOTween.To(() => lineRenderer.startWidth, x => lineRenderer.startWidth = x, 0f, 0.2f).SetEase(Ease.OutCubic).onComplete += () =>
        {
            lineRenderer.enabled = false;
            lineRenderer.startWidth = 0.08f;
        };
    }
}