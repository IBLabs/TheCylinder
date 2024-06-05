using System.Threading.Tasks;

using TMPro;

using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class TestRelay : MonoBehaviour
{
    [SerializeField] private TMP_InputField joinCodeInputField;
    [SerializeField] private Button joinButton;

    public UnityEvent<string> OnRelayJoinCodeReceived;
    public UnityEvent<ulong> DidConnectToHost;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            JoinRelay();
        }
    }

    public async void CreateRelay()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(3);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            OnRelayJoinCodeReceived?.Invoke(joinCode);

            Debug.Log("received join code: " + joinCode);

            RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            NetworkManager.Singleton.StartHost();
        }
        catch (RelayServiceException e)
        {
            Debug.Log("failed to start relay and host: " + e);
        }

    }

    public void JoinRelay()
    {
        string joinCode = joinCodeInputField.text;

        if (IsJoinCodeValid(joinCode))
        {
            JoinRelayWithCode(joinCode);
        }
        else
        {
            Debug.Log("invalid join code");
        }
    }

    private bool IsJoinCodeValid(string joinCode)
    {
        return !string.IsNullOrEmpty(joinCode) && System.Text.RegularExpressions.Regex.IsMatch(joinCode, @"^[a-zA-Z0-9]{6}$");
    }

    private async void JoinRelayWithCode(string joinCode)
    {
        try
        {
            Debug.Log("joining relay with " + joinCode);

            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            RegisterNetworkManagerListeners();

            NetworkManager.Singleton.StartClient();
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
        }
    }

    private async Task PerformInitialization()
    {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("signed in " + AuthenticationService.Instance.PlayerId);
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public async void OnPlatformDetected(bool isVR)
    {
        await PerformInitialization();

        if (isVR) CreateRelay();
    }

    private void RegisterNetworkManagerListeners()
    {
        NetworkManager.Singleton.OnConnectionEvent += HandleConnectionEvent;
    }

    private void RemoveNetworkManagerListeners()
    {
        NetworkManager.Singleton.OnConnectionEvent -= HandleConnectionEvent;
    }

    private void HandleConnectionEvent(NetworkManager manager, ConnectionEventData eventData)
    {
        Debug.Log("connection event: " + eventData.EventType);

        if (eventData.EventType == ConnectionEvent.ClientConnected)
        {
            DidConnectToHost?.Invoke(eventData.ClientId);
        }
        else if (eventData.EventType == ConnectionEvent.ClientDisconnected)
        {
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(TestRelay))]
public class MyComponentEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TestRelay myComponent = (TestRelay)target;
        if (GUILayout.Button("Trigger OnClientConnected"))
        {
            myComponent.DidConnectToHost.Invoke(0);
        }
    }
}
#endif