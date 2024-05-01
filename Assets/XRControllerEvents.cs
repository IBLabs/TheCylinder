using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class XRControllerEvents : MonoBehaviour
{
    [SerializeField] private ActionBasedController controller;

    public UnityEvent onTriggerShoot;
    
    void Start()
    {
        controller.activateAction.action.performed += OnActivateAction;
    }

    private void OnDestroy()
    {
        controller.activateAction.action.performed -= OnActivateAction;
    }

    private void OnActivateAction(InputAction.CallbackContext context)
    {
        onTriggerShoot?.Invoke();
    }
}
