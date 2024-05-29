using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

using TMPro;

using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.VisualScripting;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TestRelay : MonoBehaviour
{
    [SerializeField] private TMP_InputField joinCodeInputField;
    [SerializeField] private Button joinButton;
    [SerializeField] private TextMeshProUGUI connectedText;

    public UnityEvent<string> OnRelayJoinCodeReceived;

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

            Debug.Log(joinCode);

            RelayServerData relayServerData = new RelayServerData(allocation, "dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartHost();

            OnRelayJoinCodeReceived?.Invoke(joinCode);
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
        }

    }

    public void JoinRelay()
    {
        string joinCode = joinCodeInputField.text;
        if (!string.IsNullOrEmpty(joinCode) && System.Text.RegularExpressions.Regex.IsMatch(joinCode, @"^[a-zA-Z0-9]{6}$"))
        {
            JoinRelay(joinCode);
        }
        else
        {
            Debug.Log("Invalid join code");
        }
    }

    private async void JoinRelay(string joinCode)
    {
        try
        {
            Debug.Log("joining relay with " + joinCode);

            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnect;
            NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnect;

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

        if (isVR)
        {
            CreateRelay();
        }
    }

    private void HandleClientConnect(ulong clientId)
    {
        connectedText.gameObject.SetActive(true);

        RemoveNetworkManagerListeners();
    }

    private void HandleClientDisconnect(ulong clientId)
    {
        RemoveNetworkManagerListeners();
    }

    private void RemoveNetworkManagerListeners()
    {
        NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnect;
        NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnect;
    }
}