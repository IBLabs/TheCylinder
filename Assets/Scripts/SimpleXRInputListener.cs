using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;

public class SimpleXRInputListener : MonoBehaviour
{
    public InputActionProperty triggerAction;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable()
    {
        triggerAction.action.performed += OnTriggerAction;
    }

    private void OnDisable()
    {
        triggerAction.action.performed -= OnTriggerAction;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerAction(InputAction.CallbackContext context)
    {
        Debug.Log("[TEST]: received trigger action!");   
    }
}
