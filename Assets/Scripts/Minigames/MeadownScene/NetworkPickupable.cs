using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Netcode;
using UnityEngine.Events;

public class NetworkPickupable : NetworkBehaviour
{
    public UnityEvent<NetworkPickupable, ulong> OnPickup;

    public override void OnNetworkSpawn()
    {
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ulong pickedUpBy = 0;

            if (NetworkManager.Singleton != null)
            {
                NetworkObject networkObject = other.GetComponent<NetworkObject>();
                pickedUpBy = networkObject.OwnerClientId;
            }

            NetworkPickupableSpawner.Instance.PlayerDidPickupPickupable.Invoke(this, pickedUpBy);
        }
    }
}