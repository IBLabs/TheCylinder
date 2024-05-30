using Unity.Netcode;

using UnityEngine;

public class ClickSpawner : NetworkBehaviour
{
    [SerializeField]
    private LayerMask targetLayer;

    [SerializeField]
    private Camera mainCamera;

    [SerializeField]
    private NetworkRigidbodySpawner spawner;

    [SerializeField]
    private NetworkObject targetPlayerPrefab;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, targetLayer))
            {
                Debug.Log("requesting spawn at " + hit.point);
                PerformSpawnServerRpc(hit.point);
            }

            Debug.DrawRay(ray.origin, ray.direction * 100, Color.red, 2);
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            PerformSpawnPlayerServerRpc(NetworkManager.Singleton.LocalClientId);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void PerformSpawnServerRpc(Vector3 location)
    {
        Debug.Log("spawning on server at " + location);
        spawner.SpawnTargetObject(location);
    }

    [ServerRpc(RequireOwnership = false)]
    private void PerformSpawnPlayerServerRpc(ulong clientId)
    {
        var newPlayer = Instantiate(targetPlayerPrefab, transform.position, transform.rotation);

        NetworkObject networkObject = newPlayer.GetComponent<NetworkObject>();
        networkObject.SpawnAsPlayerObject(clientId);
    }
}