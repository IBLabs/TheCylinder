using UnityEngine;

public class NetworkBootstrapManager : MonoBehaviour
{
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
}
