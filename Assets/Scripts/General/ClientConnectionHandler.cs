using Unity.Netcode;
using UnityEngine;

public class ClientConnectionHandler : MonoBehaviour
{
    public float spectatorHeight = 2f;
    public GameObject spectatorPrefab;
    
    private void OnEnable()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        }
    }

    private void OnDisable()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        if (NetworkManager.Singleton.IsHost && clientId != NetworkManager.Singleton.LocalClientId)
        {
            Debug.Log($"Client connected with ID: {clientId}");

            Vector3 spawnPosition = new Vector3(transform.position.x, spectatorHeight, transform.position.z); 
            Instantiate(spectatorPrefab, spawnPosition, Quaternion.identity, transform);
        }
    }
}