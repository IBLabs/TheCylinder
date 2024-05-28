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

using UnityEngine;
using UnityEngine.Events;

public class TestRelay : MonoBehaviour
{
    [SerializeField] private TMP_InputField joinCodeInputField;

    public UnityEvent<string> OnRelayJoinCodeReceived;

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
        JoinRelay(joinCode);
    }

    private async void JoinRelay(string joinCode)
    {
        try
        {
            Debug.Log("joining relay with " + joinCode);

            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

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
}