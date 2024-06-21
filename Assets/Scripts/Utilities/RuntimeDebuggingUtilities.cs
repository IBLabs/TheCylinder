using System.Collections;
using System.Collections.Generic;

using Unity.Netcode;

using UnityEditor;

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class RuntimeDebuggingUtilities
{
    private static NetworkManagerDebugConfig networkManagerConfig;

    [RuntimeInitializeOnLoadMethod]
    static void OnRuntimeInitialize()
    {
#if UNITY_EDITOR
        networkManagerConfig = AssetDatabase.LoadAssetAtPath<NetworkManagerDebugConfig>("Assets/Scripts/Utilities/DefaultNetworkManagerDebugConfig.asset");

        if (networkManagerConfig == null)
        {
            Debug.LogError("NetworkManagerDebugConfig not found. Please create a NetworkManagerDebugConfig asset in the Utilities folder.");
            return;
        }

        InitializDebugNetworkFramework(networkManagerConfig);
#endif
    }

    private static void InitializDebugNetworkFramework(NetworkManagerDebugConfig networkManagerConfig)
    {
        if (!networkManagerConfig.loadDebugManagers)
        {
            return;
        }

        if (Object.FindFirstObjectByType<NetworkManager>() == null)
        {
            if (networkManagerConfig.networkManagerPrefab != null)
            {
                GameObject.Instantiate(networkManagerConfig.networkManagerPrefab, Vector3.zero, Quaternion.identity);
                Debug.Log("Network Manager instantiated for debugging.");
            }
            else
            {
                Debug.LogError("Network Manager prefab is not assigned in the configuration.");
            }

            if (networkManagerConfig.networkRelayManagerPrefab != null)
            {
                var networkRelayManager = GameObject.Instantiate(networkManagerConfig.networkRelayManagerPrefab, Vector3.zero, Quaternion.identity);
                Debug.Log("Network Relay Manager instantiated for debugging.");

#if UNITY_EDITOR
                StartRelay(networkRelayManager.GetComponent<NetworkRelayManager>());
#endif
            }
            else
            {
                Debug.LogError("Network Relay Manager prefab is not assigned in the configuration.");

            }
        }
        else
        {
            Debug.Log("Network Manager already exists in the scene.");
        }
    }

    private static async void StartRelay(NetworkRelayManager networkRelayManager)
    {
        await networkRelayManager.PerformInitialization();
        networkRelayManager.CreateRelay();
    }
}
