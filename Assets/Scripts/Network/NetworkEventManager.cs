using System;
using UnityEngine;
using UnityEngine.Events;
using Unity.Netcode;

[System.Serializable]
public class Vector3Event : UnityEvent<Vector3> { }

public class NetworkEventManager : NetworkBehaviour
{
    public Vector3Event onSpawnRequestReceived;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void RequestSpawnObject()
    {
        Vector3 spawnPosition = transform.position;
        RequestSpawnObjectServerRpc(spawnPosition);
    }
    
    public void RequestSpawnObject(Vector3 position)
    {
        RequestSpawnObjectServerRpc(position);
    }

    [ServerRpc(RequireOwnership = false)]
    void RequestSpawnObjectServerRpc(Vector3 position)
    {
        onSpawnRequestReceived.Invoke(position);
    }
}