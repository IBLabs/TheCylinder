using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;

public class SimpleXRInputListener : MonoBehaviour
{
    public InputActionProperty triggerAction;

    public Transform rightController;
    public GameObject explosionPrefab;
    
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
        Ray ray = new Ray(rightController.position, rightController.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject.CompareTag("Enemy"))
            {
                GameObject newExplosion = Instantiate(explosionPrefab, hit.point, Quaternion.identity);
                Destroy(newExplosion, 2f);
                
                Collider[] colliders = Physics.OverlapSphere(hit.point, 5f);
                foreach (var collider in colliders)
                {
                    if (collider.gameObject.CompareTag("Enemy"))
                    {
                        Rigidbody rb = collider.GetComponent<Rigidbody>();
                        if (rb != null)
                        {
                            rb.AddExplosionForce(1000f, hit.point, 5f);
                        }
                    }
                }

                Destroy(hit.collider.gameObject);
            }
        }
    }
}
