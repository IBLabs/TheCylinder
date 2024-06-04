using System.Collections;
using System.Collections.Generic;

using Unity.Netcode;
using Unity.VisualScripting;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR;
using UnityEngine.UIElements;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Events;

public class XRPlayerShooter : MonoBehaviour
{
    [SerializeField] private float cooldownTime = 1f;
    public bool cooldownEnabled = true;

    [SerializeField] private InputActionProperty triggerAction;

    public UnityEvent OnPlayerKilled;
    public UnityEvent<GameObject> OnEnemyHit;

    public float normalizedCooldownTimer => Mathf.Clamp01(cooldownTimer / cooldownTime);

    private XRPlayerShooterVisualizer _visualizer;
    private XRBaseControllerInteractor _attachedInteractor;

    private float cooldownTimer = 0f;

    private bool isGunReady => cooldownTimer <= 0f;

    void Start()
    {
        _visualizer = GetComponent<XRPlayerShooterVisualizer>();
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
        _visualizer.VisualizePossitiveHit(hitVisualizationPosition, Vector3.up);

        if (NetworkManager.Singleton != null)
        {
            NetworkObject hitNetworkObject = hitObject.GetComponent<NetworkObject>();
            if (hitNetworkObject != null)
            {
                var localId = NetworkManager.Singleton.LocalClientId;
                var networkObjectOwnerId = hitNetworkObject.OwnerClientId;

                if (localId != networkObjectOwnerId)
                {
                    Debug.Log("the object is owned by someone else, destroying it...");

                    Destroy(hitObject);

                    OnPlayerKilled?.Invoke();
                }
            }
        }
        else
        {
            Destroy(hitObject);

            OnPlayerKilled?.Invoke();
        }
    }

    private void HandleHitEnemy(GameObject hitObject, Vector3 hitPoint)
    {
        OnEnemyHit?.Invoke(hitObject);
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

        _visualizer.VisualizeShot(transform.position, transform.forward);

        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, Mathf.Infinity))
        {
            _visualizer.VisualizeHit(hit.point, hit.normal);

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
}
