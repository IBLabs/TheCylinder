using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Events;

public class MeadowBatController : NetworkBehaviour
{
    public UnityEvent<ulong> OnPlayerHitByBat;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            ulong hitPlayer = 0;

            if (NetworkManager.Singleton != null)
            {
                NetworkObject playerNetworkObject = other.GetComponent<NetworkObject>();
                hitPlayer = playerNetworkObject.OwnerClientId;
            }

            OnPlayerHitByBat?.Invoke(hitPlayer);
        }
    }
}
