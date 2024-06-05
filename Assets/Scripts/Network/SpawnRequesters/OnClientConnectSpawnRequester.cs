using UnityEngine;

[RequireComponent(typeof(NetworkSimplePlayerSpawner))]
public class OnClientConnectSpawnRequester : MonoBehaviour
{
    private NetworkSimplePlayerSpawner _spawner;

    void Start()
    {
        _spawner = GetComponent<NetworkSimplePlayerSpawner>();
    }

    public void OnDidConnectToHost(ulong clientId)
    {
        Debug.Log("requesting spawn for client: " + clientId);

        _spawner.RequestSpawnFromServer(clientId);
    }
}
