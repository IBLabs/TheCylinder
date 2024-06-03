using System.Collections;
using System.Collections.Generic;

using Unity.Netcode;
using Unity.VisualScripting;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class XRPlayerShooter : MonoBehaviour
{
    [SerializeField]
    private InputActionProperty triggerAction;

    private XRPlayerShooterVisualizer _visualizer;

    void Start()
    {
        _visualizer = GetComponent<XRPlayerShooterVisualizer>();
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

        _visualizer.VisualizeShot(transform.position, transform.forward);

        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Player")))
        {
            Debug.Log("hit player object: " + hit.collider.gameObject.name);

            NetworkObject hitNetworkObject = hit.collider.gameObject.GetComponent<NetworkObject>();
            if (hitNetworkObject != null)
            {
                var localId = NetworkManager.Singleton.LocalClientId;
                var networkObjectOwnerId = hitNetworkObject.OwnerClientId;

                if (localId == networkObjectOwnerId)
                {
                    Debug.Log("the object is owned by hitter");
                    return;
                }
                else
                {
                    Debug.Log("the object is owned by someone else, destroying it...");
                    Destroy(hit.collider.gameObject);
                }
            }
        }
    }
}
