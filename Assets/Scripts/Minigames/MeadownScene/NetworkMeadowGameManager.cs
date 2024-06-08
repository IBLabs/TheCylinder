using UnityEngine;

using Unity.Netcode;

public class NetworkMeadowGameManager : NetworkBehaviour
{
    private const int TARGET_POINTS = 5;

    private int pointCount = 0;

    public void OnPlayerPickedupPickupable(NetworkPickupable pickupable, ulong clientId)
    {
        DestroyPickupableServerRpc(pickupable.NetworkObject);
        AddPointServerRpc(clientId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void DestroyPickupableServerRpc(NetworkObjectReference pickupableNetworkObjectRef)
    {
        pickupableNetworkObjectRef.TryGet(out NetworkObject pickupableNetworkObject);
        if (pickupableNetworkObject != null)
        {
            pickupableNetworkObject.Despawn(true);
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
}
