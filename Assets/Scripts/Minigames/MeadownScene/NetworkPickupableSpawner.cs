using UnityEngine;

using Unity.Netcode;
using Unity.VisualScripting;

public class NetworkPickupableSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject pickupablePrefab;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private float spawnHeight = .2f;

    private Transform _lastUsedTransformPoint;

    public void SpawnPickupableAtRandomSpawnPoint()
    {
        Transform spawnTransform;

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            spawnTransform = transform;
        }
        else if (spawnPoints.Length < 2)
        {
            spawnTransform = spawnPoints[0];
        }
        else
        {
            do
            {
                spawnTransform = spawnPoints[Random.Range(0, spawnPoints.Length)];
            } while (spawnTransform == _lastUsedTransformPoint);
        }

        SpawnPickupable(spawnTransform);
    }

    public void SpawnPickupable(Transform spawnTransform)
    {
        if (NetworkManager.Singleton != null)
        {
            if (!IsServer) return;

            HandleNetworkPickupableSpawn(spawnTransform.position, spawnTransform.rotation);
        }
        else
        {
            HandleLocalPickupableSpawn(spawnTransform.position, spawnTransform.rotation);
        }

        _lastUsedTransformPoint = spawnTransform;
    }

    private void HandleNetworkPickupableSpawn(Vector3 spawnPosition, Quaternion rotation)
    {
        if (!IsServer)
        {
            Debug.LogWarning("client cannot spawn pickupables");
            return;
        }

        var targetSpawnPosition = spawnPosition + new Vector3(0, spawnHeight, 0);

        NetworkObject networkObject = LocalSpawnPickupable(targetSpawnPosition, rotation).GetComponent<NetworkObject>();
        networkObject.Spawn();
    }

    private void HandleLocalPickupableSpawn(Vector3 spawnPosition, Quaternion rotation)
    {
        LocalSpawnPickupable(spawnPosition, rotation);
    }

    private GameObject LocalSpawnPickupable(Vector3 spawnPosition, Quaternion rotation)
    {

        GameObject newPickupable = Instantiate(pickupablePrefab, spawnPosition, rotation);

        return newPickupable;
    }
}
