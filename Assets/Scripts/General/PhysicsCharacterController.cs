using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;

public class PhysicsCharacterController : MonoBehaviour
{
    [Header("Floating")]
    [SerializeField] private float rideHeight = .5f;
    [SerializeField] private float rideSpringStrength = 20.0f;
    [SerializeField] private float rideDamperStrength = 2.0f;

    [Header("Movement")]
    [SerializeField] private float maxSpeed = 10.0f;
    [SerializeField] private float acceleration = 10.0f;
    [SerializeField] private float maxAccelerationForce = 10.0f;
    [SerializeField] private AnimationCurve accelerationFactorCurve;

    [Header("Actions")]
    [SerializeField] private InputActionProperty moveAction;

    private Rigidbody _rigidbody;

    private Vector3 _goalVelocity = Vector3.zero;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        HandleFloatingInAir();
        HandleSomethingElse();
    }

    private void HandleSomethingElse()
    {
        Vector3 moveInput = moveAction.action.ReadValue<Vector2>();

        // TODO: adjust based on camera angle

        Vector3 moveDirection = new Vector3(moveInput.x, 0, moveInput.y);
        Vector3 targetVelocity = moveDirection * maxSpeed;

        float velocityDifference = Vector3.Dot(_rigidbody.velocity.normalized, targetVelocity.normalized);
        float accelerationFactor = accelerationFactorCurve.Evaluate(velocityDifference);
        float modifiedAcceleration = acceleration * accelerationFactor;

        _goalVelocity = Vector3.MoveTowards(_goalVelocity, targetVelocity, modifiedAcceleration * Time.fixedDeltaTime);

        Vector3 neededAcceleration = (_goalVelocity - _rigidbody.velocity) / Time.fixedDeltaTime;

        neededAcceleration = Vector3.ClampMagnitude(neededAcceleration, maxAccelerationForce);

        _rigidbody.AddForce(neededAcceleration);
    }

    private void HandleFloatingInAir()
    {
        RaycastHit hit;
        bool didHit = Physics.Raycast(transform.position, Vector3.down, out hit, 1.0f);
        if (didHit)
        {
            Vector3 currentVelocity = _rigidbody.velocity;

            Vector3 hitObjectVelocity = Vector3.zero;
            if (hit.rigidbody != null)
            {
                hitObjectVelocity = hit.rigidbody.velocity;
            }

            Vector3 localRayDirection = transform.TransformDirection(Vector3.down);

            float vel1 = Vector3.Dot(localRayDirection, currentVelocity);
            float vel2 = Vector3.Dot(localRayDirection, hitObjectVelocity);

            float relativeVelocity = vel1 - vel2;
            float x = hit.distance - rideHeight;

            float springForce = (x * rideSpringStrength) - (relativeVelocity * rideDamperStrength);

            Vector3 springForceVector = localRayDirection * springForce;

            _rigidbody.AddForce(springForceVector);
        }
    }
}
