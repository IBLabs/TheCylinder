using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

namespace DefaultNamespace
{
    public class TargetTubesController : MonoBehaviour
    {
        public GameObject targetSpawnedObject;
        public float spawnForce = 10f;

        public InputActionProperty triggerAction;

        private void OnEnable()
        {
            triggerAction.action.performed += OnTriggerAction;
        }

        private void OnDisable()
        {
            triggerAction.action.performed -= OnTriggerAction;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SpawnSomething();
            }
        }

        private void OnTriggerAction(InputAction.CallbackContext context)
        {
            SpawnSomething();
        }

        private void SpawnSomething()
        {
            int randomIndex = Random.Range(0, transform.childCount);
            Transform randomChild = transform.GetChild(randomIndex);
            GameObject spawnedObject = Instantiate(targetSpawnedObject, randomChild);
            spawnedObject.transform.localPosition = Vector3.zero;
                
            Rigidbody spawnedObjectRb = spawnedObject.GetComponent<Rigidbody>();
            if (spawnedObjectRb != null)
            {
                spawnedObjectRb.isKinematic = false;
                spawnedObjectRb.AddForce(Vector3.up * spawnForce, ForceMode.Impulse);
                    
                float torqueX = Random.Range(-0.5f, 0.5f);
                float torqueY = Random.Range(-0.5f, 0.5f);
                float torqueZ = Random.Range(-0.5f, 0.5f);
                spawnedObjectRb.AddTorque(new Vector3(torqueX, torqueY, torqueZ), ForceMode.Impulse);
            }
        }
    }
}