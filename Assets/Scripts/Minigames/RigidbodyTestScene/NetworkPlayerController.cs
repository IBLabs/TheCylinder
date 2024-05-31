using DG.Tweening;

using Unity.Netcode;

using UnityEngine;

public class NetworkPlayerController : NetworkBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float gravity = 9.8f;

    private CharacterController characterController;
    private Camera playerCamera;
    private Transform cameraPivotTransform;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        cameraPivotTransform = GameObject.FindGameObjectWithTag("CameraPivot").transform;
        playerCamera = cameraPivotTransform.GetComponentInChildren<Camera>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftBracket))
        {
            RotateCamera(false);
        }
        else if (Input.GetKeyDown(KeyCode.RightBracket))
        {
            RotateCamera(true);
        }

        // if (!IsOwner) return;

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 movementDirection = new Vector3(horizontalInput, 0f, verticalInput);
        movementDirection.Normalize();

        Vector3 cameraForward = playerCamera.transform.forward;
        cameraForward.y = 0;
        cameraForward.Normalize();

        Vector3 cameraRight = playerCamera.transform.right;
        cameraRight.y = 0;
        cameraRight.Normalize();

        movementDirection = cameraForward * movementDirection.z + cameraRight * movementDirection.x;

        if (characterController != null)
        {
            characterController.Move(movementDirection * moveSpeed * Time.deltaTime);

            if (!characterController.isGrounded)
            {
                characterController.Move(Vector3.down * gravity * Time.deltaTime);
            }
        }
        else
        {
            transform.Translate(movementDirection * moveSpeed * Time.deltaTime);
        }
    }

    private void RotateCamera(bool clockwise)
    {
        cameraPivotTransform.DORotate(new Vector3(0, clockwise ? 90f : -90f, 0), 0.5f).SetRelative(true).SetEase(Ease.OutQuad);
    }
}
