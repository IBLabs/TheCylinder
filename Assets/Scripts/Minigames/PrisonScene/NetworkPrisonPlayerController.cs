using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;

public class NetworkPrisonPlayerController : MonoBehaviour
{
    [SerializeField] private InputActionProperty activateAction;

    void Start()
    {
        activateAction.action.performed += OnActivatePerformed;
    }

    void OnDestroy()
    {
        activateAction.action.performed -= OnActivatePerformed;
    }

    private void OnActivatePerformed(InputAction.CallbackContext context)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 0.12f);
        foreach (var collider in colliders)
        {
            var actionableObject = collider.GetComponent<IActionableObject>();
            if (actionableObject != null)
            {
                actionableObject.PerformAction();
            }
        }
    }
}
