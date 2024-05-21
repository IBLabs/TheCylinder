using UnityEngine;

namespace UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets
{
    public class CollisionForceAmplifier : MonoBehaviour
    {
        public float forceMultiplier = 10.0f;

        private void OnCollisionEnter(Collision collision)
        {
            Rigidbody rb = collision.collider.attachedRigidbody;

            if (rb != null && !rb.isKinematic)
            {
                Vector3 forceDirection = collision.contacts[0].point - transform.position;
                forceDirection = -forceDirection.normalized; // Reverse direction towards the other object

                rb.AddForce(forceDirection * forceMultiplier, ForceMode.Impulse);
            }
        }
    }
}