using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEngine;

public class ClientNetworkDebugCanvasController : MonoBehaviour
{
    [SerializeField] private TMP_InputField codeInputField;

    public void Join()
    {
        var relayNetworkManager = FindFirstObjectByType<NetworkRelayManager>();

        if (relayNetworkManager == null)
        {
            Debug.LogError("NetworkRelayManager not found in the scene.");
            return;
        }

        relayNetworkManager.JoinRelay(codeInputField.text);
    }
}
