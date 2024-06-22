using System.Collections;
using System.Collections.Generic;

using Unity.Netcode;

using UnityEngine;

public class NetworkHiddenGemSpawner : NetworkBehaviour
{
    [Header("Configuration")]
    [SerializeField] private GameObject hiddenGemPrefab;
    [SerializeField] private bool autoSpawnOnStart;
    [SerializeField] private Transform[] spawnPoints;

    void Start()
    {
        var hasNetworkAccess = NetworkManager.Singleton != null;
        if (!hasNetworkAccess)
        {
            if (autoSpawnOnStart)
            {
                SpawnHiddenGemAtRandomPosition();
            }
        }
        else
        {
            if (IsServer)
            {
                if (autoSpawnOnStart)
                {
                    SpawnHiddenGemAtRandomPosition();
                }
            }
        }
    }

    public void SpawnHiddenGemAtRandomPosition()
    {
        var randomIndex = Random.Range(0, spawnPoints.Length);
        var randomSpawnPoint = spawnPoints[randomIndex];

        SpawnHiddenGem(randomSpawnPoint.position);
    }

    public void SpawnHiddenGem(Vector3 position)
    {
        var newHiddenGem = Instantiate(hiddenGemPrefab, position, Quaternion.identity);

        var hasNetworkAccess = NetworkManager.Singleton != null;
        if (hasNetworkAccess)
        {
            var hiddenGemNetworkObject = newHiddenGem.GetComponent<NetworkObject>();
            hiddenGemNetworkObject.Spawn(destroyWithScene: true);
        }
    }
}
