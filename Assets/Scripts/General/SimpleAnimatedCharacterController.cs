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
    private bool isGrounded;

    private Vector2 m_inputVector;
    private bool m_runPressed;
    private float m_speed;

    private OwnerNetworkAnimator _networkAnimator;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        // Enable the input actions
        moveAction.action.Enable();
        jumpAction.action.Enable();
        runAction.action.Enable(); // Enable the run action
    }

    void Update()
    {
        if (!IsOwner) return;

        Vector2 inputVector = moveAction.action.ReadValue<Vector2>();
        bool runPressed = runAction.action.ReadValue<float>() > 0.5f;
        float speed = runPressed ? runSpeed : moveSpeed;

        bool jumpPressed = jumpAction.action.triggered;

        m_inputVector = inputVector;
        m_runPressed = runPressed;
        m_speed = speed;

        UpdateAnimator(inputVector);
    }

    void FixedUpdate()
    {
        Move(m_inputVector, m_speed);
    }

    void Move(Vector2 inputVector, float speed)
    {
        Vector3 movement = new Vector3(inputVector.x, 0.0f, inputVector.y) * speed * Time.deltaTime;
        rb.MovePosition(transform.position + movement);

        if (movement != Vector3.zero) // If the character is moving
        {
            transform.rotation = Quaternion.LookRotation(movement); // Rotate the character to face the direction of movement
        }
    }

    void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    void UpdateAnimator(Vector2 inputVector)
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
            isGrounded = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}
