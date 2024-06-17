using Unity.Netcode;

using UnityEngine;
using UnityEngine.InputSystem;

public class SimpleAnimatedCharacterController : NetworkBehaviour
{
    public float moveSpeed = 5f;
    public float runSpeed = 10f;
    public float jumpForce = 5f;
    public InputActionProperty moveAction;
    public InputActionProperty jumpAction;
    public InputActionProperty runAction; // New run action

    private Rigidbody rb;
    private Animator animator;

    // private bool isGrounded;

    private Vector3 m_inputVector;
    private bool m_runPressed;
    private float m_speed;

    private Camera _targetCamera;

    // private OwnerNetworkAnimator _networkAnimator;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        moveAction.action.Enable();
        jumpAction.action.Enable();
        runAction.action.Enable();

        var targetCamera = GameObject.FindWithTag("DesktopCamera");
        if (targetCamera != null && targetCamera.TryGetComponent(out Camera targetCameraComponent))
        {
            _targetCamera = targetCameraComponent;
        }
    }

    void Update()
    {
        if (!IsOwner) return;

        Vector2 rawInputVector = moveAction.action.ReadValue<Vector2>();
        Vector3 inputVector = new Vector3(rawInputVector.x, 0, rawInputVector.y);

        bool runPressed = runAction.action.ReadValue<float>() > 0.5f;
        float speed = runPressed ? runSpeed : moveSpeed;

        bool jumpPressed = jumpAction.action.triggered;

        Vector3 cameraForward = _targetCamera.transform.forward;
        cameraForward.y = 0f;
        cameraForward.Normalize();

        Vector3 cameraRight = _targetCamera.transform.right;
        cameraRight.y = 0f;
        cameraRight.Normalize();

        m_inputVector = cameraForward * inputVector.z + cameraRight * inputVector.x;

        m_runPressed = runPressed;
        m_speed = speed;

        UpdateAnimator(inputVector);
    }

    void FixedUpdate()
    {
        Move(m_inputVector, m_speed);
    }

    void Move(Vector3 inputVector, float speed)
    {
        Vector3 movement = new Vector3(inputVector.x, 0.0f, inputVector.z) * speed * Time.deltaTime;
        rb.MovePosition(transform.position + movement);

        if (movement != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(movement);
        }
    }

    void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    void UpdateAnimator(Vector3 inputVector)
    {
        bool isWalking = inputVector.magnitude > 0;
        bool isRunning = runAction.action.ReadValue<float>() > 0.5f;

        animator.SetBool("isWalking", isWalking);
        animator.SetBool("isRunning", isRunning);
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            // isGrounded = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            // isGrounded = false;
        }
    }
}
