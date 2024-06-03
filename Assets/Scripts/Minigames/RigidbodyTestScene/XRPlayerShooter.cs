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
    [SerializeField] private InputActionProperty triggerAction;

    public UnityEvent OnPlayerKilled;

    private XRPlayerShooterVisualizer _visualizer;
    private XRBaseControllerInteractor _attachedInteractor;

    void Start()
    {
        _visualizer = GetComponent<XRPlayerShooterVisualizer>();
        _attachedInteractor = GetComponent<XRBaseControllerInteractor>();
    }

    void OnEnable()
    {
        triggerAction.action.performed += OnTriggerActionPerformed;
    }

    void OnDisable()
    {
        triggerAction.action.performed -= OnTriggerActionPerformed;
    }

    private void OnTriggerActionPerformed(InputAction.CallbackContext context)
    {
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
                Debug.Log("hit player object: " + hit.collider.gameObject.name);

                Vector3 hitVisualizationPosition = new Vector3(hit.collider.transform.position.x, hit.point.y, hit.collider.transform.position.z);
                _visualizer.VisualizePossitiveHit(hitVisualizationPosition, Vector3.up);

                if (NetworkManager.Singleton != null)
                {
                    NetworkObject hitNetworkObject = hit.collider.gameObject.GetComponent<NetworkObject>();
                    if (hitNetworkObject != null)
                    {
                        var localId = NetworkManager.Singleton.LocalClientId;
                        var networkObjectOwnerId = hitNetworkObject.OwnerClientId;

                        if (localId != networkObjectOwnerId)
                        {
                            Debug.Log("the object is owned by someone else, destroying it...");

                            Destroy(hit.collider.gameObject);

                            OnPlayerKilled?.Invoke();
                        }
                    }
                }
                else
                {
                    Destroy(hit.collider.gameObject);

                    OnPlayerKilled?.Invoke();
                }
            }
        }
    }
}
