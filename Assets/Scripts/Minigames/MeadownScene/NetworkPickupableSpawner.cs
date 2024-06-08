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
        if (IsServer && autoSpawnOnStart)
        {
            StartCoroutine(AutoSpawnCoroutine());
        }
    }

    private IEnumerator AutoSpawnCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(1, 4));
            SpawnPickupableAtRandomSpawnPoint();
        }
    }

    public UnityEvent<NetworkPickupable, ulong> PlayerDidPickupPickupable;

    public void SpawnPickupableAtRandomSpawnPoint()
    {
        Transform spawnTransform = (spawnPoints != null) && (spawnPoints.Length > 0) ? spawnPoints[Random.Range(0, spawnPoints.Length)] : transform;
        SpawnPickupable(spawnTransform.position, spawnTransform.rotation);
    }

    public void SpawnPickupable(Vector3 spawnPosition, Quaternion rotation)
    {
        if (NetworkManager.Singleton != null)
        {
            HandleNetworkPickupableSpawn(spawnPosition, rotation);
        }
        else
        {
            HandleLocalPickupableSpawn(spawnPosition, rotation);
        }
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
        Destroy(pickupable.gameObject);
    }
}
