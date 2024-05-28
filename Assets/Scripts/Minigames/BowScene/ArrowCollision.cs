using UnityEngine;

public class ArrowCollision : MonoBehaviour
{
    private Rigidbody _rigidbody;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        _rigidbody.velocity = Vector2.zero;
        _rigidbody.isKinematic = true;

        transform.parent = collision.transform;
        transform.position += transform.up * 0.5f;
    }
}