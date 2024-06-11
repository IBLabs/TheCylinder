using System.Collections;
using System.Collections.Generic;

using Unity.Netcode;

using UnityEngine;
using UnityEngine.InputSystem;

public class OnButtonSpawnRequester : NetworkBehaviour
{
    [SerializeField] private InputActionProperty spawnAction;

    void OnEnable()
    {
        spawnAction.action.performed += OnSpawn;
    }

    void OnDisable()
    {
        spawnAction.action.performed -= OnSpawn;
    }

    private void OnSpawn(InputAction.CallbackContext context)
    {
        var spawner = GetComponent<NetworkSimplePlayerSpawner>();

        ulong clientId = (NetworkManager.Singleton != null) ? NetworkManager.Singleton.LocalClientId : 0;
        spawner.RequestSpawnFromServer(clientId);
    }
}
