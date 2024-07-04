using UnityEngine;
using DG.Tweening;
using Unity.Netcode;

[RequireComponent(typeof(XRPlayerShooter))]
public class NetworkXRPlayerShooterVisualizer : NetworkBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private ParticleSystem hitParticleSystem;
    [SerializeField] private ParticleSystem positiveHitParticleSystem;
    [SerializeField] private float lineDuration = 0.2f;

    private XRPlayerShooter _shooter;
    private Tweener _gunPunchTweener;

    void Start()
    {
        _shooter = GetComponent<XRPlayerShooter>();

        _shooter.DidShoot.AddListener(OnDidShoot);
        _shooter.DidHit.AddListener(OnDidHit);
        _shooter.DidHitPositive.AddListener(OnDidHitPositive);
    }

    public override void OnDestroy()
    {
        _shooter.DidShoot.RemoveListener(OnDidShoot);
        _shooter.DidHit.RemoveListener(OnDidHit);
        _shooter.DidHitPositive.RemoveListener(OnDidHitPositive);

        base.OnDestroy();
    }

    private void OnDidShoot(Vector3 origin, Vector3 direction)
    {
        VisualizeShot(origin, direction);
    }

    private void OnDidHit(Vector3 position, Vector3 faceDirection)
    {
        VisualizeHit(position, faceDirection);
    }

    private void OnDidHitPositive(Vector3 position, Vector3 faceDirection)
    {
        VisualizePossitiveHit(position, faceDirection);
    }

    void Update()
    {
        if (lineRenderer.startWidth > 0)
        {
            lineRenderer.startWidth = Mathf.Max(0, lineRenderer.startWidth - lineDuration * Time.deltaTime);
        }
        else if (lineRenderer.enabled) lineRenderer.enabled = false;
    }

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

    public void VisualizeHit(Vector3 position, Vector3 faceDirection)
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkVisualizeHit(position, faceDirection);
        }
        else
        {
            PerformVisualizeHit(position, faceDirection);
        }
    }

    public void VisualizePossitiveHit(Vector3 position, Vector3 faceDirection)
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkVisualizePositiveHit(position, faceDirection);
        }
        else
        {
            PerformVisualizePositiveHit(position, faceDirection);
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

    private void NetworkVisualizeHit(Vector3 position, Vector3 faceDirection)
    {
        if (!IsServer)
        {
            Debug.Log("only server is allowed to visualize hits");
            return;
        }

        Debug.Log($"{GetType().Name} server visualizing hit...");

        PerformVisualizeHit(position, faceDirection);

        VisualizeHitClientRpc(position, faceDirection);
    }

    private void NetworkVisualizePositiveHit(Vector3 position, Vector3 faceDirection)
    {
        if (!IsServer)
        {
            Debug.Log("only server is allowed to visualize positive hits");
            return;
        }

        Debug.Log($"{GetType().Name} server visualizing positive hit...");

        PerformVisualizePositiveHit(position, faceDirection);

        VisualizePositiveHitClientRpc(position, faceDirection);
    }

    [ClientRpc]
    private void VisualizeShotClientRpc(Vector3 origin, Vector3 direction)
    {
        if (IsServer) return;

        Debug.Log($"{GetType().Name} client visualizing shot...");

        PerformVisualizeShot(origin, direction);
    }

    [ClientRpc]
    private void VisualizeHitClientRpc(Vector3 position, Vector3 faceDirection)
    {
        if (IsServer) return;

        Debug.Log($"{GetType().Name} client visualizing hit...");

        PerformVisualizeHit(position, faceDirection);
    }

    [ClientRpc]
    private void VisualizePositiveHitClientRpc(Vector3 position, Vector3 faceDirection)
    {
        if (IsServer) return;

        Debug.Log($"{GetType().Name} client visualizing positive hit...");

        PerformVisualizePositiveHit(position, faceDirection);
    }

    private void PerformVisualizeShot(Vector3 origin, Vector3 direction)
    {
        lineRenderer.SetPosition(0, origin);
        lineRenderer.SetPosition(1, origin + direction * 100f);

        lineRenderer.enabled = true;
        lineRenderer.startWidth = 0.08f;
    }

    private void PerformVisualizeHit(Vector3 position, Vector3 faceDirection)
    {
        var newHitParticleSystem = Instantiate(hitParticleSystem);
        newHitParticleSystem.transform.position = position;
        newHitParticleSystem.transform.forward = faceDirection;

        Destroy(newHitParticleSystem.gameObject, 2f);
    }

    private void PerformVisualizePositiveHit(Vector3 position, Vector3 faceDirection)
    {
        var newPositiveHitParticleSystem = Instantiate(positiveHitParticleSystem);
        newPositiveHitParticleSystem.transform.position = position;
        newPositiveHitParticleSystem.transform.forward = faceDirection;

        Destroy(newPositiveHitParticleSystem.gameObject, 2f);
    }
}