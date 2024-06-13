using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Netcode;
using UnityEngine.Events;

public class NetworkPickupable : NetworkBehaviour
{
    [SerializeField] private float timeToLive = 30.0f;

    private bool _isTimerActive = true;

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
            ulong pickedUpBy = GetPlayerClientId(other);

            StopTimerServerRpc();

            NetworkMeadowGameManager.Instance.OnPlayerPickedupPickupable(this, pickedUpBy);
        }
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

    private ulong GetPlayerClientId(Collider other)
    {
        if (NetworkManager.Singleton == null) return 0;

        NetworkObject playerNetworkObject = other.GetComponent<NetworkObject>();

        if (!playerNetworkObject.IsOwner) return 0;

        return playerNetworkObject.OwnerClientId;
    }
}
