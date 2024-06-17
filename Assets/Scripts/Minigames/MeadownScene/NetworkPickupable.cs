using UnityEngine;

using Unity.Netcode;
using System;
using Unity.Netcode.Components;

public class NetworkPickupable : NetworkBehaviour
{
    [SerializeField] private float timeToLive = 30.0f;

    private bool _isTimerActive = true;
    private bool _isPickedUp = false;

    void Update()
    {
        bool hasNetworkManager = NetworkManager.Singleton != null;

        if ((hasNetworkManager && IsServer) || !hasNetworkManager)
        {
            if (_isTimerActive)
            {
                timeToLive -= Time.deltaTime;

                if (timeToLive <= 0)
                {
                    _isTimerActive = false;

                    NetworkMeadowGameManager.Instance.OnPickupableDidDie(this);
                }
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            bool hasNetworkAccess = NetworkManager.Singleton != null;

            if (hasNetworkAccess)
            {
                if (!other.TryGetComponent(out NetworkObject playerNetworkObject))
                {
                    Debug.LogWarning("Player object does not have NetworkObject component");
                    return;
                }

                if (!playerNetworkObject.IsOwner)
                {
                    Debug.Log("Current client does not own the collided player object, aborting");
                    return;
                }

                AttemptPickupServerRpc(playerNetworkObject);

                StopTimerServerRpc();
            }
            else
            {
                // TODO: handle offline scenario
            }

            // TODO: should not be used
            // NetworkMeadowGameManager.Instance.OnPlayerPickedupPickupable(this, pickedUpBy);
        }
    }

    public override void OnNetworkObjectParentChanged(NetworkObject parentNetworkObject)
    {
        if (!IsServer) return;

        if (parentNetworkObject == null)
        {
            _isPickedUp = false;

            if (TryGetComponent(out NetworkTransform networkTransform))
            {
                networkTransform.InLocalSpace = false;
            }

            ResumeTimer();
        }

        base.OnNetworkObjectParentChanged(parentNetworkObject);
    }

    public void ResumeTimer()
    {
        ResumeTimerServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void ResumeTimerServerRpc()
    {
        _isTimerActive = true;
    }

    [ServerRpc(RequireOwnership = false)]
    private void StopTimerServerRpc()
    {
        _isTimerActive = false;

        Debug.Log($"{GetType().Name} server RPC stopping timer for pickupable");
    }

    [ServerRpc(RequireOwnership = false)]
    private void AttemptPickupServerRpc(NetworkObjectReference playerRef)
    {
        if (_isPickedUp)
        {
            Debug.Log("Already picked up, aborting");
            return;
        }

        if (!playerRef.TryGet(out NetworkObject playerNetworkObject))
        {
            Debug.LogWarning("Couldn't get referenced player network object");
            return;
        }

        if (NetworkObject.TrySetParent(playerNetworkObject))
        {
            NetworkSoundManager.Instance.PlaySoundServerRpc("MeadowPickup1", transform.position);

            _isPickedUp = true;

            if (TryGetComponent(out NetworkTransform networkTransform))
            {
                networkTransform.InLocalSpace = true;
                transform.localPosition = Vector3.up * 0.4f;
            }
        }
        else
        {
            Debug.LogWarning("Failed to parent pickupable to player");
        }
    }

    private ulong GetPlayerClientId(Collider other)
    {
        if (NetworkManager.Singleton == null) return 0;

        NetworkObject playerNetworkObject = other.GetComponent<NetworkObject>();

        if (!playerNetworkObject.IsOwner) return 0;

        return playerNetworkObject.OwnerClientId;
    }
}
