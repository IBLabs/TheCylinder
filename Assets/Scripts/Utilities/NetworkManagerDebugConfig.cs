using UnityEngine;

[CreateAssetMenu(fileName = "NetworkManagerDebugConfig", menuName = "Settings/NetworkManagerDebugConfig")]
public class NetworkManagerDebugConfig : ScriptableObject
{
    public bool loadDebugManagers = true;
    public bool addUIConsoleLogger = true;
    public bool initializeInBuild = false;
    public GameObject networkManagerPrefab;
    public GameObject networkRelayManagerPrefab;
    public GameObject uiConsoleLogger;
}