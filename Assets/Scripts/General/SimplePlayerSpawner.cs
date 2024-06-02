using System.Collections;
using System.Collections.Generic;

using Unity.Netcode;
using Unity.VisualScripting;

using UnityEngine;
using UnityEngine.Rendering;

public class SimplePlayerSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject playerPrefab;

    private bool _didSpawn;

    void Start()
    {
        if (NetworkManager.Singleton == null)
        {
            SpawnPlayer();
        }
    }

    public void OnClientConnected(ulong clientId)
    {
        if (!IsServer)
        {
            SpawnPlayerServerRpc(clientId);
        }
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
        {
            SpawnPlayerServerRpc(NetworkManager.Singleton.LocalClientId);
        }

        base.OnNetworkSpawn();
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnPlayerServerRpc(ulong clientId)
    {
        if (_didSpawn) return;

        var newPlayer = SpawnPlayer();

        NetworkObject networkObject = newPlayer.GetComponent<NetworkObject>();
        networkObject.SpawnAsPlayerObject(clientId, true);

        _didSpawn = true;
    }

    private GameObject SpawnPlayer()
    {
        return Instantiate(playerPrefab, transform.position, transform.rotation);
    }
}
