using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

using TMPro;

using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

using UnityEngine;

using Utilities;

public class NetworkManagerSetup : MonoBehaviour
{
    public bool autoStartAsHost;

    private const int port = 7777;

    void Start()
    {
        NetworkManagerSetup[] networkManagerSetups = FindObjectsByType<NetworkManagerSetup>(FindObjectsSortMode.None);
        if (networkManagerSetups.Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        if (autoStartAsHost)
        {
            StartHost();
        }
    }

    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
    }

    public void StartClient(string address)
    {
        SetUnityTransportConnectionData(address);
        NetworkManager.Singleton.StartClient();
    }

    private void SetUnityTransportConnectionData(string address)
    {
        NetworkManager networkManager = GetComponent<NetworkManager>();
        UnityTransport unityTransport = networkManager.GetComponent<UnityTransport>();
        unityTransport.ConnectionData.Address = address;
        unityTransport.ConnectionData.Port = port;

        Debug.Log($"Set UnityTransport connection data to {address}:{port}");
    }
}