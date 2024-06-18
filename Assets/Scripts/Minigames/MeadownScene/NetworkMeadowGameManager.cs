using UnityEngine;

using Unity.Netcode;
using UnityEngine.Events;
using System.Collections;
using Unity.Netcode.Components;
using Unity.VisualScripting;

public class NetworkMeadowGameManager : NetworkBehaviour
{
    public static NetworkMeadowGameManager Instance { get; private set; }

    public NetworkVariable<float> GameTimeLeft = new NetworkVariable<float>(0.0f);
    public NetworkVariable<int> VRPointCount = new NetworkVariable<int>(0);
    public NetworkVariable<int> DesktopPointCount = new NetworkVariable<int>(0);

    [Header("Dependencies")]
    [SerializeField] private NetworkPickupableSpawner pickupableSpawner;
    [SerializeField] private MeadowBankController bankController;
    [SerializeField] private SceneLoader sceneLoader;

    [SerializeField] private MeadowGameEndController xrGameEndController;

    [Header("Configuration")]
    [SerializeField] private float gameDuration = 120.0f;
    [SerializeField] private bool autoStartGameOnStart = false;

    [Header("Events")]
    public UnityEvent<float> OnGameDidStart;
    public UnityEvent OnGameDidFinish;

    private NetworkVariable<bool> _isGameRunning = new NetworkVariable<bool>(false);

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (NetworkManager.Singleton == null && autoStartGameOnStart)
        {
            StartGame();
        }
    }

    public override void OnNetworkSpawn()
    {
        if (NetworkManager.Singleton != null && autoStartGameOnStart)
        {
            StartGame();
        }

        base.OnNetworkSpawn();
    }

    public void StartGame()
    {
        if (NetworkManager != null)
        {
            if (!IsServer) return;
        }

        _isGameRunning.Value = true;

        GameTimeLeft.Value = gameDuration;
        StartCoroutine(GameTimerCoroutine());

        pickupableSpawner.SpawnPickupableAtRandomSpawnPoint();

        Debug.Log($"{GetType().Name} starting game...");
    }

    private IEnumerator GameTimerCoroutine()
    {
        while (GameTimeLeft.Value > 0)
        {
            GameTimeLeft.Value -= Time.deltaTime;
            yield return null;
        }

        _isGameRunning.Value = false;

        FinishGameServerRpc();
    }

    public void OnPickupableDidDie(NetworkPickupable pickupable)
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkHandlePickupableDidDie(pickupable);
        }
        else
        {
            Destroy(pickupable.gameObject);
        }
    }

    public void OnPlayerDroppedPickupables(ulong clientId)
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkHandlePlayerDroppedPickupables(clientId);
        }
        else
        {
            // TODO: handle offline scenario
        }
    }

    private void NetworkHandlePlayerDroppedPickupables(ulong clientId)
    {
        if (!_isGameRunning.Value) return;

        var targetPlayer = FindPlayerNetworkObjectByCliendId(clientId);
        if (targetPlayer != null)
        {
            FinishPickupableRoundServerRpc(WinnerType.Desktop);

            var pickupables = targetPlayer.GetComponentsInChildren<NetworkPickupable>();
            foreach (var pickupable in pickupables)
            {
                DestroyNetworkObjectServerRpc(pickupable.NetworkObject);
            }

            SpawnNewPickupableServerRpc();

            RelocateDrophouseServerRpc();
        }
    }

    private void NetworkHandlePickupableDidDie(NetworkPickupable pickupable)
    {
        if (!IsServer)
        {
            Debug.LogWarning("OnPickupableDidDie was called from a client, this shouldn't happen");
            return;
        }

        if (!_isGameRunning.Value) return;

        FinishPickupableRoundServerRpc(WinnerType.VR);

        DestroyNetworkObjectServerRpc(pickupable.NetworkObject);

        SpawnNewPickupableServerRpc();

        RelocateDrophouseServerRpc();
    }

    private void LocalHandlePlayerPickedupPickupable(NetworkPickupable pickupable)
    {
        var playerController = FindAnyObjectByType<NetworkPlayerController>();
        if (playerController != null)
        {
            pickupable.transform.SetParent(playerController.transform);
        }
        else
        {
            Destroy(pickupable.gameObject);
        }
    }

    private NetworkObject FindPlayerNetworkObjectByCliendId(ulong clientId)
    {
        if (NetworkManager.Singleton == null) return null;

        var players = GameObject.FindGameObjectsWithTag("Player");
        foreach (var player in players)
        {
            var playerNetworkObject = player.GetComponent<NetworkObject>();
            if (playerNetworkObject != null && playerNetworkObject.OwnerClientId == clientId)
            {
                return playerNetworkObject;
            }
        }

        return null;
    }

    [ServerRpc(RequireOwnership = false)]
    private void DestroyNetworkObjectServerRpc(NetworkObjectReference networkObjectRef)
    {
        networkObjectRef.TryGet(out NetworkObject networkObject);
        if (networkObject != null)
        {
            networkObject.Despawn(true);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnNewPickupableServerRpc()
    {
        pickupableSpawner.SpawnPickupableAtRandomSpawnPoint();
    }

    [ServerRpc(RequireOwnership = false)]
    private void RelocateDrophouseServerRpc()
    {
        Transform newTargetTransform = bankController.PerformRandomRelocate();
        RelocateDrophouseClientRpc(newTargetTransform.position);
    }

    [ServerRpc(RequireOwnership = false)]
    private void FinishPickupableRoundServerRpc(WinnerType roundWinner)
    {
        if (!_isGameRunning.Value) return;

        switch (roundWinner)
        {
            case WinnerType.Desktop:
                DesktopPointCount.Value++;
                break;
            case WinnerType.VR:
                VRPointCount.Value++;
                break;
            default:
                break;
        }
    }

    [ServerRpc]
    private void FinishGameServerRpc()
    {
        WinnerType winner = WinnerType.Unset;
        if (VRPointCount.Value > DesktopPointCount.Value)
        {
            NetworkScoreKeeper.Instance.AddXrScore();
            winner = WinnerType.VR;
        }
        else if (VRPointCount.Value < DesktopPointCount.Value)
        {
            NetworkScoreKeeper.Instance.AddDesktopScore();
            winner = WinnerType.Desktop;
        }

        ShowEndGameUIClientRpc(winner);
    }

    [ClientRpc]
    private void RelocateDrophouseClientRpc(Vector3 position)
    {
        if (IsServer) return;

        bankController.PerformRelocate(position);
    }

    [ClientRpc]
    private void ShowEndGameUIClientRpc(WinnerType winner)
    {
        xrGameEndController.ShowGameEndScreen(winner);

        OnGameDidFinish?.Invoke();
    }
}
