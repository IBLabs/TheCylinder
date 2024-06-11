using System.Collections;
using System.Collections.Generic;

using Unity.Netcode;

using UnityEngine;

[RequireComponent(typeof(NetworkSimplePlayerSpawner))]
public class OnNetworkSpawnSpawnRequester : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        if (!enabled) return;

        var spawner = GetComponent<NetworkSimplePlayerSpawner>();
        spawner.RequestSpawnFromServer(NetworkManager.Singleton.LocalClientId);

        base.OnNetworkSpawn();
    }
}
