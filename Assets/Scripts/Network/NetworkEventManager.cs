using System;
using UnityEngine;
using UnityEngine.Events;
using Unity.Netcode;
using UnityEngine.SceneManagement;

[System.Serializable]
public class Vector3Event : UnityEvent<Vector3> { }

public class NetworkEventManager : NetworkBehaviour
{
    public Vector3Event onSpawnRequestReceived;

    private void Start()
    {
        NetworkEventManager[] networkEventManagers = FindObjectsByType<NetworkEventManager>(FindObjectsSortMode.None);
        if (networkEventManagers.Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    public void RequestSpawnObject()
    {
        Debug.Log("[TEST]: requesting spawn object with default position");
        Vector3 spawnPosition = transform.position;
        RequestSpawnObjectServerRpc(spawnPosition);
    }

    public void RequestSpawnObject(Vector3 position)
    {
        Debug.Log("[TEST]: requesting spawn object with position: " + position);
        RequestSpawnObjectServerRpc(position);
    }

    [ServerRpc(RequireOwnership = false)]
    void RequestSpawnObjectServerRpc(Vector3 position)
    {
        Debug.Log("[TEST] received spawn request at position: " + position);
        onSpawnRequestReceived.Invoke(position);
    }
}