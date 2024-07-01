using Unity.Netcode;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class XRPlayerShooter : MonoBehaviour
{
    [SerializeField] private float cooldownTime = 1f;
    public bool cooldownEnabled = true;

    [SerializeField] private InputActionProperty triggerAction;

    public UnityEvent OnPlayerKilled;
    public UnityEvent<GameObject> OnEnemyHit;

    public UnityEvent<Vector3, Vector3> DidShoot;
    public UnityEvent<Vector3, Vector3> DidHit;
    public UnityEvent<Vector3, Vector3> DidHitPositive;

    public float normalizedCooldownTimer => Mathf.Clamp01(cooldownTimer / cooldownTime);

    private XRBaseControllerInteractor _attachedInteractor;

    private float cooldownTimer = 0f;

    private bool isGunReady => cooldownTimer <= 0f;

    void Start()
    {
        _attachedInteractor = GetComponent<XRBaseControllerInteractor>();
    }

    void Update()
    {
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
        }
    }

    void OnEnable()
    {
        triggerAction.action.performed += OnTriggerActionPerformed;
    }

    void OnDisable()
    {
        triggerAction.action.performed -= OnTriggerActionPerformed;
    }

    private void HandleHitPlayer(GameObject hitObject, Vector3 hitPoint)
    {
        Debug.Log("hit player object: " + hitObject.name);

        Vector3 hitVisualizationPosition = new Vector3(hitObject.transform.position.x, hitPoint.y, hitObject.transform.position.z);

        DidHitPositive?.Invoke(hitVisualizationPosition, Vector3.up);

        if (!HandleNetworkHit(hitObject))
        {
            Destroy(hitObject);
            OnPlayerKilled?.Invoke();
        }
    }

    private void HandleHitEnemy(GameObject hitObject, Vector3 hitPoint)
    {
        if (hitObject.TryGetComponent(out NetworkEnemyHitVisualizer visualizer))
        {
            visualizer.RequestVisualizeHit(() =>
            {
                OnEnemyHit?.Invoke(hitObject);
            });
        }
    }

    private void OnTriggerActionPerformed(InputAction.CallbackContext context)
    {
        if (cooldownEnabled && !isGunReady) return;

        cooldownTimer = cooldownTime;

        Debug.Log("attempting to shoot...");

        if (_attachedInteractor != null)
        {
            _attachedInteractor.SendHapticImpulse(0.5f, 0.1f);
        }

        DidShoot?.Invoke(transform.position, transform.forward);

        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, Mathf.Infinity))
        {
            DidHit?.Invoke(hit.point, hit.normal);

            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                HandleHitPlayer(hit.collider.gameObject, hit.point);
            }
            else if (hit.collider.CompareTag("Enemy"))
            {
                HandleHitEnemy(hit.collider.gameObject, hit.point);
            }
        }
    }

    private bool HandleNetworkHit(GameObject hitObject)
    {
        Debug.Log("[TEST]: handling network hit...");

        if (NetworkManager.Singleton == null)
        {
            Debug.Log("[TEST]: network manager is null, returning false...");
            return false;
        }

        NetworkObject hitNetworkObject = hitObject.GetComponent<NetworkObject>();
        if (hitNetworkObject == null)
        {
            Debug.Log("[TEST]: network object is null, returning false...");
            return false;
        }

        var localId = NetworkManager.Singleton.LocalClientId;
        var networkObjectOwnerId = hitNetworkObject.OwnerClientId;

        if (localId != networkObjectOwnerId)
        {
            Debug.Log("the object is owned by someone else, destroying it...");

            Destroy(hitObject);

            OnPlayerKilled?.Invoke();
        }


        return true;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(XRPlayerShooter))]
public class XRPlayerShooterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var shooter = (XRPlayerShooter)target;

        if (GUILayout.Button("Kill Random Player"))
        {
            var playerGameObject = GameObject.FindWithTag("Player");
            if (playerGameObject != null)
            {
                Destroy(playerGameObject);
                shooter.OnPlayerKilled?.Invoke();
            }
        }
    }
}
#endif