using UnityEngine;

public class NetworkBootstrapManager : MonoBehaviour
{
    [Header("Utilities")]
    [SerializeField] private bool useConsoleOnDesktop;
    [SerializeField] private bool useConsoleOnVR;
    [SerializeField] private GameObject uiConsoleLoggerPrefab;

    void Start()
    {

    }

    public void OnPlatformDetected(bool isVR)
    {
        if (isVR && useConsoleOnVR)
        {
            SetupConsole();
        }
        else if (!isVR && useConsoleOnDesktop)
        {
            SetupConsole();
        }
    }

    private void SetupConsole()
    {
        if (useConsoleOnDesktop && uiConsoleLoggerPrefab != null)
        {
            GameObject uiConsoleLogger = Instantiate(uiConsoleLoggerPrefab);
            DontDestroyOnLoad(uiConsoleLogger);
        }
    }
}
