using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Events;
using UnityEngine.SocialPlatforms;
using Unity.VisualScripting.FullSerializer;

public class MeadowBatController : NetworkBehaviour
{
    [SerializeField] private GameObject hitEffect;
    [SerializeField] private float hitCooldown = .35f;

    private bool _canHit = true;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            var hasNetworkAccess = NetworkManager.Singleton != null;

            if (hasNetworkAccess && !IsServer)
            {
                Debug.LogWarning("Only the server can process player hits");
                return;
            }

            if (!other.TryGetComponent(out NetworkObject playerNetworkObject))
            {
                Debug.LogWarning($"{GetType().Name} couldn't find target player's network object");
                return;
            }

            AttemptKillPlayerServerRpc(playerNetworkObject);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        var hitPoint = collision.GetContact(0).point;
        var hitNormal = collision.GetContact(0).normal;

        if (_canHit && hitEffect != null)
        {
            bool hasNetworkAccess = NetworkManager.Singleton != null;
            if (hasNetworkAccess && IsServer)
            {
                VisualizeHitClientRpc(hitPoint, hitNormal);
                NetworkSoundManager.Instance.PlaySoundServerRpc("HammerHit1", hitPoint);
            }
            else if (!hasNetworkAccess)
            {
                LocalVisualizeHit(hitPoint, hitNormal);
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void AttemptKillPlayerServerRpc(NetworkObjectReference playerToKillRef)
    {
        if (!playerToKillRef.TryGet(out NetworkObject playerToKill))
        {
            Debug.LogWarning($"{GetType().Name} couldn't find target player's network object");
            return;
        }

        var playerPickupables = playerToKill.GetComponentsInChildren<NetworkPickupable>();
        if (playerPickupables.Length > 0)
        {
            foreach (var pickupable in playerPickupables)
            {
                pickupable.NetworkObject.TryRemoveParent();
            }
        }

        Destroy(playerToKill.gameObject);
    }

    [ClientRpc]
    private void VisualizeHitClientRpc(Vector3 position, Vector3 normal)
    {
        LocalVisualizeHit(position, normal);
    }

    private void LocalVisualizeHit(Vector3 position, Vector3 normal)
    {
        var effect = Instantiate(hitEffect, position, Quaternion.LookRotation(normal));
        Destroy(effect, 2.0f);

        StartCoroutine(CooldownCoroutine());
    }

    private IEnumerator CooldownCoroutine()
    {
        _canHit = false;
        yield return new WaitForSeconds(hitCooldown);
        _canHit = true;
    }
}
