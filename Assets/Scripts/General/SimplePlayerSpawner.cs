using System.Collections;
using System.Collections.Generic;

using Unity.Netcode;
using Unity.VisualScripting;

using UnityEngine;
using UnityEngine.InputSystem;
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

    public void OnSpawnRequested(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            if (NetworkManager.Singleton != null)
            {
                var players = FindObjectsByType<NetworkPlayerController>(FindObjectsSortMode.None);

                foreach (var player in players)
                {
                    if (player.IsOwner)
                    {
                        Debug.Log("player already spawned");
                        return;
                    }
                }

                SpawnPlayerServerRpc(NetworkManager.Singleton.LocalClientId, true);
            }
            else
            {
                var player = FindAnyObjectByType<NetworkPlayerController>();

                if (player != null)
                {
                    Debug.Log("player already spawned");
                    return;
                }

                SpawnPlayer();
            }
        }
    }

    public void OnClientConnected(ulong clientId)
    {
        if (!IsServer)
        {
            SpawnPlayerServerRpc(clientId, false);
        }
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
        {
            SpawnPlayerServerRpc(NetworkManager.Singleton.LocalClientId, false);
        }

        base.OnNetworkSpawn();
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnPlayerServerRpc(ulong clientId, bool isRespawn)
    {
        if (!isRespawn && _didSpawn) return;

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
