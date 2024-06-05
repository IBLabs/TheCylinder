using Unity.Netcode;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class NetworkSimplePlayerSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject playerPrefab;

    public void OnSpawnRequested(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            if (NetworkManager.Singleton != null)
            {
                RequestSpawnFromServer(NetworkManager.Singleton.LocalClientId);
            }
            else
            {
                HandleLocalSpawn();
            }
        }
    }

    public void RequestSpawnFromServer(ulong clientId)
    {
        if (IsServer)
        {
            Debug.Log("the server can't request a spawn");
            return;
        }

        HandleNetworkSpawn(clientId);
    }

    private void HandleNetworkSpawn(ulong clientId)
    {
        if (IsPlayerAlreadySpawnedNetwork()) return;

        SpawnPlayerServerRpc(clientId);
    }

    private void HandleLocalSpawn()
    {
        if (IsPlayerAlreadySpawnedLocal()) return;

        SpawnPlayerLocal();
    }

    private bool IsPlayerAlreadySpawnedLocal()
    {
        var player = FindAnyObjectByType<NetworkPlayerController>();
        if (player != null)
        {
            Debug.Log("player already spawned");
            return true;
        }

        return false;
    }

    private bool IsPlayerAlreadySpawnedNetwork()
    {
        var players = FindObjectsByType<NetworkPlayerController>(FindObjectsSortMode.None);
        foreach (var player in players)
        {
            if (player.IsOwner)
            {
                Debug.Log("player already spawned");
                return true;
            }
        }

        return false;
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnPlayerServerRpc(ulong clientId)
    {
        var newPlayer = SpawnPlayerLocal();

        NetworkObject networkObject = newPlayer.GetComponent<NetworkObject>();
        networkObject.SpawnAsPlayerObject(clientId, true);
    }

    private GameObject SpawnPlayerLocal()
    {
        return Instantiate(playerPrefab, transform.position, transform.rotation);
    }
}
