using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Netcode;
using UnityEngine.Rendering;
using UnityEngine.Events;

public class NetworkPickupableSpawner : NetworkBehaviour
{
    public static NetworkPickupableSpawner Instance { get; private set; }

    [SerializeField] private GameObject pickupablePrefab;

    [SerializeField] private Transform[] spawnPoints;

    [SerializeField] private bool autoSpawnOnStart = false;
    [SerializeField] private int spawnCount = -1;

    private Transform _lastUsedTransformPoint;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("multiple NetworkPickupableSpawner instances detected, destroying this one");
            Destroy(this);
        }
    }

    void Start()
    {
        if (autoSpawnOnStart)
        {
            StartCoroutine(AutoSpawnCoroutine());
        }
    }

    private IEnumerator AutoSpawnCoroutine()
    {
        int spawnCounter = 0;
        while ((spawnCount == -1) || (spawnCount > 0 && spawnCounter < spawnCount))
        {
            yield return new WaitForSeconds(Random.Range(1, 4));
            SpawnPickupableAtRandomSpawnPoint();
            spawnCounter++;
        }
    }

    public UnityEvent<NetworkPickupable, ulong> PlayerDidPickupPickupable;

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

        NetworkObject networkObject = LocalSpawnPickupable(spawnPosition, rotation).GetComponent<NetworkObject>();
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

    public void OnPickupablePickedUp(NetworkPickupable pickupable, ulong pickedUpBy)
    {
        Debug.Log("pickupable picked up by " + pickedUpBy);

        PlayerDidPickupPickupable?.Invoke(pickupable, pickedUpBy);
    }
}
