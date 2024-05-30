using Unity.Netcode;

using UnityEngine;

public class NetworkPlayerController : NetworkBehaviour
{
    private CharacterController characterController; // CharacterController reference

    // Start is called before the first frame update
    void Start()
    {
        // Get the CharacterController component if it exists
        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        // if (!IsOwner) return;

        float moveSpeed = 5f; // Adjust the movement speed as needed
        float gravity = 9.8f; // Adjust the gravity value as needed

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 movementDirection = new Vector3(horizontalInput, 0f, verticalInput);
        movementDirection.Normalize();

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
}
