using UnityEngine;

using Unity.Netcode;

public class NetworkMeadowGameManager : NetworkBehaviour
{
    private const int TARGET_POINTS = 5;

    private int pointCount = 0;

    public void OnPlayerPickedupPickupable(NetworkPickupable pickupable, ulong clientId)
    {
        if (NetworkManager.Singleton != null)
        {
            var targetPlayer = FindPlayerNetworkObjectByCliendId(clientId);
            if (targetPlayer != null)
            {
                AttemptParentPickupableToPlayerServerRpc(pickupable.NetworkObject, targetPlayer);
            }
        }
        else
        {
            var playerController = FindAnyObjectByType<NetworkPlayerController>();
            if (playerController != null)
            {
                pickupable.transform.SetParent(playerController.transform);
            }
            else
            {
                Destroy(pickupable.gameObject);
            }
        }
    }

    public void OnPlayerDroppedPickupables(ulong clientId)
    {
        if (NetworkManager.Singleton != null)
        {
            var targetPlayer = FindPlayerNetworkObjectByCliendId(clientId);
            if (targetPlayer != null)
            {
                var pickupables = targetPlayer.GetComponentsInChildren<NetworkPickupable>();
                foreach (var pickupable in pickupables)
                {
                    DestroyNetworkObjectServerRpc(pickupable.NetworkObject);
                }
            }
        }
        else
        {
            // TODO: handle offline scenario
        }
    }

    public void OnPlayerHitByBat(ulong clientId)
    {
        if (NetworkManager.Singleton != null)
        {
            var targetPlayer = FindPlayerNetworkObjectByCliendId(clientId);
            if (targetPlayer == null) return;

            var playerPickupables = targetPlayer.gameObject.GetComponentsInChildren<NetworkPickupable>();
            foreach (var playerPickupable in playerPickupables)
            {
                playerPickupable.transform.SetParent(null);
            }

            DestroyNetworkObjectServerRpc(targetPlayer);
        }
        else
        {
            // TODO: handle offline scenario
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void AttemptParentPickupableToPlayerServerRpc(NetworkObjectReference pickupableNetworkObjectRef, NetworkObjectReference playerNetworkObjectRef)
    {
        pickupableNetworkObjectRef.TryGet(out NetworkObject pickupableNetworkObject);
        playerNetworkObjectRef.TryGet(out NetworkObject playerNetworkObject);

        if (pickupableNetworkObject != null && playerNetworkObject != null)
        {
            pickupableNetworkObject.transform.SetParent(playerNetworkObject.transform);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void DestroyNetworkObjectServerRpc(NetworkObjectReference networkObjectRef)
    {
        networkObjectRef.TryGet(out NetworkObject networkObject);
        if (networkObject != null)
        {
            networkObject.Despawn(true);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void AddPointServerRpc(ulong clientId)
    {
        pointCount++;

        Debug.Log("points scored by player: " + clientId + ", total points: " + pointCount);

        if (CheckWinCondition())
        {
            FinishGameClientRpc(WinnerType.Desktop);
        }
    }

    private bool CheckWinCondition()
    {
        return pointCount >= TARGET_POINTS;
    }

    [ClientRpc]
    private void FinishGameClientRpc(WinnerType winner)
    {
        Debug.Log($"Game finished, winner: {winner}");
    }

    private NetworkObject FindPlayerNetworkObjectByCliendId(ulong clientId)
    {
        if (NetworkManager.Singleton != null)
        {
            var players = GameObject.FindGameObjectsWithTag("Player");
            foreach (var player in players)
            {
                var playerNetworkObject = player.GetComponent<NetworkObject>();
                if (playerNetworkObject != null && playerNetworkObject.OwnerClientId == clientId)
                {
                    return playerNetworkObject;
                }
            }
        }

        return null;
    }
}
