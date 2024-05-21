using Unity.Netcode;
using UnityEngine;

public class NetworkManagerSetup : MonoBehaviour
{
    public bool autoStartAsHost;
    
    void Start()
    {
        if (autoStartAsHost)
        {
            StartHost();
        }
    }

    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
    }

    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
    }
}