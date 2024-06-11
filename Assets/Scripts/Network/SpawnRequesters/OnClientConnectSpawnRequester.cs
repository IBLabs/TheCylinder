using UnityEngine;

[RequireComponent(typeof(NetworkSimplePlayerSpawner))]
public class OnClientConnectSpawnRequester : MonoBehaviour
{
    private NetworkSimplePlayerSpawner _spawner;

    public void OnDidConnectToHost(ulong clientId)
    {
        if (!enabled) return;

        Debug.Log("requesting spawn for client: " + clientId);

        _spawner = GetComponent<NetworkSimplePlayerSpawner>();

        _spawner.RequestSpawnFromServer(clientId);
    }
}
