using TMPro;

using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

using UnityEngine;
using UnityEngine.SceneManagement;

using Utilities;

public class NetworkBootstrapManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField serverAddressInputField;

    [Header("Utilities")]
    [SerializeField] private bool useConsole;
    [SerializeField] private GameObject uiConsoleLoggerPrefab;

    void Start()
    {
        if (useConsole && uiConsoleLoggerPrefab != null)
        {
            GameObject uiConsoleLogger = Instantiate(uiConsoleLoggerPrefab);
            DontDestroyOnLoad(uiConsoleLogger);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            TryStartClient();
        }
    }

    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
    }

    public void TryStartClient()
    {
        if (string.IsNullOrEmpty(serverAddressInputField.text) || !ItaNetworkUtilities.ValidateIPAddress(serverAddressInputField.text))
        {
            Debug.LogError("server address is empty");
            return;
        }

        string serverAddress = serverAddressInputField.text;
        ushort port = 7777;

        NetworkManager networkManager = NetworkManager.Singleton.GetComponent<NetworkManager>();
        UnityTransport unityTransport = networkManager.GetComponent<UnityTransport>();
        unityTransport.ConnectionData.Address = serverAddress;
        unityTransport.ConnectionData.Port = port;

        Debug.Log($"Set UnityTransport connection data to {serverAddress}:{port}");

        NetworkManager.Singleton.StartClient();
    }

    #region Events

    public void OnPlatformDetected(bool isVR)
    {
        if (isVR)
        {
            // TODO: remove this, old method
            // StartHost();
        }
    }

    #endregion
}
