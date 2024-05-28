using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Vector3 = UnityEngine.Vector3;

public class HandTransformer : MonoBehaviour
{
    [SerializeField] private Transform rootTransform;
    [SerializeField] private Transform targetTransform;

    [SerializeField] private Transform controllerTransform;
        
    public float moveSpeed = 5f;
    public float distanceFactor = 1f;

    [SerializeField] private InputActionProperty primaryButtonAction;

    private Transform _hmdTransform;

    private Vector3 _initialControllerPos;
    private Vector3 _initialTargetPos;

    private readonly float _handsLength = 19f;

    private void OnEnable()
    {
        primaryButtonAction.action.performed += OnPrimaryButtonPressed;
    }

    private void OnDisable()
    {
        primaryButtonAction.action.performed -= OnPrimaryButtonPressed;
    }

    private void OnPrimaryButtonPressed(InputAction.CallbackContext context)
    {
        float distance = Vector3.Distance(_hmdTransform.position, controllerTransform.position) * distanceFactor;
        
        Debug.Log("[TEST]: distance is: " + distance);
        
        moveSpeed = (_handsLength * transform.localScale.x) / distance;
        
        Debug.Log("[TEST]: moveSpeed is: " + moveSpeed);
    }

    private void Start()
    {
        _hmdTransform = Camera.main.transform;
        
        ResetControllerInitialPos();
        ResetTargetInitialPos();
    }

    private void ResetControllerInitialPos()
    {
        _initialControllerPos = controllerTransform.position;
    }

    private void ResetTargetInitialPos()
    {
        _initialTargetPos = targetTransform.position;
    }

    private void Update()
    {
        // TODO: use this
        // Vector3 moveDelta = (controllerTransform.position - _initialControllerPos) * moveSpeed;
        // targetTransform.position = _initialTargetPos + moveDelta;
        
        // get controller position relative to hmd
        Vector3 controllerPos = controllerTransform.position - _hmdTransform.position;
        Quaternion controllerRot = controllerTransform.rotation;
        
        // position the target relative to the root using controllerPos
        targetTransform.position = rootTransform.position + controllerPos * moveSpeed;
        targetTransform.rotation = controllerRot;
    }

    public void SetDistanceFactor(float newFactor)
    {
        distanceFactor = newFactor;
    }
}