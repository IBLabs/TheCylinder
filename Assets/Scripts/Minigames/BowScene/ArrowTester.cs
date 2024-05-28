using Unity.Mathematics;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

class ArrowTester : MonoBehaviour
{
    [SerializeField] private XRInteractionManager interactionManager;
    [SerializeField] private XRDirectInteractor rHandDirectInteractor;
    [SerializeField] private XRDirectInteractor lHandDirectInteractor;
    [SerializeField] private XRGrabInteractable bowInteractable;
    [SerializeField] private XRGrabInteractable grabberInteractable;

    [SerializeField] private XRSocketInteractor bowSocketInteractor;

    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Transform arrowSpawnTransform;

    [Header("Configuration")]
    [SerializeField] private float arrowSpeed = 100f;

    [Header("Actions")]
    public InputActionProperty rSelectAction;
    public InputActionProperty lSelectAction;

    private Transform _activeArrowTransform;

    private void OnEnable()
    {
        rSelectAction.action.performed += OnRHandSelectPerformed;
        lSelectAction.action.performed += OnLHandSelectPerformed;
    }

    private void OnDisable()
    {
        rSelectAction.action.performed -= OnRHandSelectPerformed;
        lSelectAction.action.performed -= OnLHandSelectPerformed;
    }

    private void OnRHandSelectPerformed(InputAction.CallbackContext context)
    {
        if (bowInteractable is IXRSelectInteractable bowSelectInteractable)
        {
            interactionManager.SelectEnter(rHandDirectInteractor, bowSelectInteractable);
        }
    }

    private void OnLHandSelectPerformed(InputAction.CallbackContext context)
    {
        if (grabberInteractable is IXRSelectInteractable selectInteractable)
        {
            interactionManager.SelectEnter(lHandDirectInteractor, selectInteractable);
        }
    }

    public void OnSelectEnter(SelectEnterEventArgs args)
    {
        GameObject newArrow = Instantiate(arrowPrefab, arrowSpawnTransform.position, Quaternion.identity, arrowSpawnTransform);

        Quaternion rotDelta = Quaternion.FromToRotation(Vector3.up, arrowSpawnTransform.forward);
        newArrow.transform.rotation *= rotDelta;

        Rigidbody rb = newArrow.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }

        _activeArrowTransform = newArrow.transform;
    }

    public void OnSelectExit(SelectExitEventArgs args)
    {
        if (_activeArrowTransform != null)
        {
            Rigidbody rb = _activeArrowTransform.GetComponent<Rigidbody>();
            if (rb != null)
            {
                _activeArrowTransform.parent = null;

                rb.isKinematic = false;

                Vector3 velocity = arrowSpawnTransform.forward * arrowSpeed;
                rb.velocity = velocity;
            }

            _activeArrowTransform = null;
        }
    }
}