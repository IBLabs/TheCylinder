using DG.Tweening;

using Unity.Netcode;
using Unity.VisualScripting;

using UnityEngine;
using UnityEngine.InputSystem;

public class NetworkPlayerController : NetworkBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float gravity = 9.8f;
    [SerializeField] private bool gravityEnabled = true;
    [SerializeField] private InputActionProperty moveAction;
    [SerializeField] private InputActionProperty activateAction;
    [SerializeField] private Camera playerCamera;

    private CharacterController characterController;
    private Vector3 _movementDirection;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();

        if (characterController != null && NetworkManager.Singleton != null)
        {
            characterController.enabled = false;
        }
    }

    void Start()
    {
        var cameraGameObject = GameObject.FindGameObjectWithTag("DesktopCamera");
        if (cameraGameObject != null)
        {
            playerCamera = cameraGameObject.GetComponent<Camera>();
        }

        activateAction.action.performed += OnActivatePerformed;
    }

    public override void OnDestroy()
    {
        activateAction.action.performed -= OnActivatePerformed;

        base.OnDestroy();
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            // var simplePlayerSpawner = FindAnyObjectByType<NetworkSimplePlayerSpawner>();
            // if (simplePlayerSpawner != null)
            // {
            //     transform.position = simplePlayerSpawner.transform.position;
            // }

            if (characterController != null)
            {
                characterController.enabled = true;
            }

            base.OnNetworkSpawn();
        }
    }

    void FixedUpdate()
    {
        if (characterController != null)
        {
            characterController.Move(_movementDirection * moveSpeed * Time.deltaTime);

            if (_movementDirection != Vector3.zero)
            {
                Quaternion toRotation = Quaternion.LookRotation(_movementDirection, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, Time.deltaTime * rotationSpeed);
            }

            if (!characterController.isGrounded && gravityEnabled)
            {
                characterController.Move(Vector3.down * gravity * Time.deltaTime);
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadPlus) || Input.GetKeyDown(KeyCode.Plus))
        {
            moveSpeed += .1f;
            Debug.Log("Move speed increased to: " + moveSpeed);
        }
        else if (Input.GetKeyDown(KeyCode.KeypadMinus) || Input.GetKeyDown(KeyCode.Minus))
        {
            moveSpeed -= .1f;
            Debug.Log("Move speed decreased to: " + moveSpeed);
        }

        if (!IsOwner && NetworkManager.Singleton != null) return;

        Vector2 moveInput = moveAction.action.ReadValue<Vector2>();

        float horizontalInput = moveInput.x;
        float verticalInput = moveInput.y;

        _movementDirection = new Vector3(horizontalInput, 0f, verticalInput);

        if (playerCamera != null)
        {
            Vector3 cameraForward = playerCamera.transform.forward;
            cameraForward.y = 0;
            cameraForward.Normalize();

            Vector3 cameraRight = playerCamera.transform.right;
            cameraRight.y = 0;
            cameraRight.Normalize();

            _movementDirection = cameraForward * _movementDirection.z + cameraRight * _movementDirection.x;
        }
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
