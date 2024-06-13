using UnityEngine;

using Unity.Netcode;
using UnityEngine.Events;
using System.Collections;
using Unity.Netcode.Components;

public class NetworkMeadowGameManager : NetworkBehaviour
{
    public static NetworkMeadowGameManager Instance { get; private set; }

    public NetworkVariable<float> GameTimeLeft = new NetworkVariable<float>(0.0f);

    [SerializeField] private NetworkPickupableSpawner pickupableSpawner;
    [SerializeField] private MeadowBankController bankController;

    [Header("Configuration")]
    [SerializeField] private bool autoStartGameOnStart = false;

    [Header("Events")]
    public UnityEvent<float> OnGameDidStart;

    private const int TARGET_POINTS = 5;

    private int pointCount = 0;

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

        GameTimeLeft.Value = 120.0f;
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

        // TODO: check win condition and end game
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

    public void OnPlayerPickedupPickupable(NetworkPickupable pickupable, ulong clientId)
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkHandlePlayerPickedupPickupable(pickupable, clientId);
        }
        else
        {
            LocalHandlePlayerPickedupPickupable(pickupable);
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

    public void OnPlayerHitByBat(ulong clientId)
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkHandlePlayerHitByBat(clientId);
        }
        else
        {
            // TODO: handle offline scenario
        }
    }

    private void NetworkHandlePlayerPickedupPickupable(NetworkPickupable pickupable, ulong clientId)
    {
        var targetPlayer = FindPlayerNetworkObjectByCliendId(clientId);
        if (targetPlayer != null)
        {
            AttemptParentPickupableToPlayerServerRpc(pickupable.NetworkObject, targetPlayer);
        }
    }

    private void NetworkHandlePlayerDroppedPickupables(ulong clientId)
    {
        var targetPlayer = FindPlayerNetworkObjectByCliendId(clientId);
        if (targetPlayer != null)
        {
            var pickupables = targetPlayer.GetComponentsInChildren<NetworkPickupable>();
            foreach (var pickupable in pickupables)
            {
                DestroyNetworkObjectServerRpc(pickupable.NetworkObject);
            }

            SpawnNewPickupableServerRpc();

            RelocateDrophouseServerRpc();
        }
    }

    private void NetworkHandlePlayerHitByBat(ulong clientId)
    {
        var targetPlayer = FindPlayerNetworkObjectByCliendId(clientId);
        if (targetPlayer == null) return;

        var playerPickupables = targetPlayer.gameObject.GetComponentsInChildren<NetworkPickupable>();
        foreach (var playerPickupable in playerPickupables)
        {
            playerPickupable.transform.SetParent(null);
            playerPickupable.ResumeTimer();
        }

        DestroyNetworkObjectServerRpc(targetPlayer);
    }

    private void NetworkHandlePickupableDidDie(NetworkPickupable pickupable)
    {
        if (!IsServer)
        {
            Debug.LogWarning("OnPickupableDidDie was called from a client, this shouldn't happen");
            return;
        }

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

    private bool CheckWinCondition()
    {
        return pointCount >= TARGET_POINTS;
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
    private void AttemptParentPickupableToPlayerServerRpc(NetworkObjectReference pickupableRef, NetworkObjectReference playerRef)
    {
        // try get both the pickupable object and the player
        pickupableRef.TryGet(out NetworkObject pickupable);
        playerRef.TryGet(out NetworkObject player);

        pickupable.GetComponent<NetworkTransform>().InLocalSpace = true;

        // if both objects are valid, parent the pickupable to the player
        if (pickupable != null && player != null)
        {
            pickupable.transform.SetParent(player.transform);
            pickupable.transform.localPosition = Vector3.up * 0.27f;
        }
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
    private void AddPointServerRpc(ulong clientId)
    {
        pointCount++;

        Debug.Log("points scored by player: " + clientId + ", total points: " + pointCount);

        if (CheckWinCondition())
        {
            FinishGameClientRpc(WinnerType.Desktop);
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

    [ClientRpc]
    private void RelocateDrophouseClientRpc(Vector3 position)
    {
        if (IsServer) return;

        bankController.PerformRelocate(position);
    }

    [ClientRpc]
    private void FinishGameClientRpc(WinnerType winner)
    {
        Debug.Log($"Game finished, winner: {winner}");
    }
}
