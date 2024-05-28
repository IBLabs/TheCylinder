using Unity.Netcode;

using UnityEngine;

public class NetworkClientConnectPrefabSpawner : MonoBehaviour
{
    public GameObject objectToActivate;
    private bool hasActivated = false;

    void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
    }

    void OnClientConnected(ulong connection)
    {
        if (connection != NetworkManager.Singleton.LocalClientId && !hasActivated)
        {
            objectToActivate.SetActive(true);
            hasActivated = true;
        }

        Debug.Log("Client connected: " + connection);
    }
}
