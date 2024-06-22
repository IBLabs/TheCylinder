using System.Collections;
using System.Collections.Generic;

using Unity.Netcode;

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class NetworkFollowGameManager : NetworkBehaviour
{
    public GameState CurrentGameState => _gameState.Value;

    public float GameTimeLeft => _gameTimeLeft.Value;

    [Header("Dependencies")]
    [SerializeField] private MeadowDesktopGameEndController desktopGameEndController;

    [Header("Configuration")]
    [SerializeField] private float gameDuration = 120f;

    private readonly NetworkVariable<GameState> _gameState = new NetworkVariable<GameState>(GameState.Idle);

    private readonly NetworkVariable<float> _gameTimeLeft = new NetworkVariable<float>(0f);

    private bool _isGameRunning = false;

    public void StartGame()
    {
        var hasNetworkAccess = NetworkManager.Singleton != null;
        if (hasNetworkAccess)
        {
            if (IsServer)
            {
                StartCoroutine(StartGameCoroutine());
            }
        }
        else
        {
            StartCoroutine(StartGameCoroutine());
        }
    }

    void Update()
    {
        if (_isGameRunning)
        {
            UpdateGameTime();
        }
    }

    #region Events

    public void OnPlayerKilled()
    {
        StartCoroutine(OnPlayerKilledCoroutine());
    }

    private IEnumerator OnPlayerKilledCoroutine()
    {
        yield return new WaitForEndOfFrame();

        if (GameObjectUtilities.CheckAllPlayersDead())
        {
            EndGame(WinnerType.VR);

            Debug.Log("[TEST]: VR player wins!");
        }
    }

    #endregion

    #region Networking

    [ClientRpc]
    private void ShowGameEndClientRpc(WinnerType winner)
    {
        LocalEndGame(winner);
    }

    #endregion

    private void EndGame(WinnerType winner)
    {
        if (_isGameRunning) { _isGameRunning = false; }

        bool hasNetworkAccess = NetworkManager.Singleton != null;
        if (hasNetworkAccess)
        {
            if (IsServer)
            {
                NetworkEndGame(winner);
            }
        }
        else
        {
            LocalEndGame(winner);
        }
    }

    private void NetworkEndGame(WinnerType winner)
    {
        ShowGameEndClientRpc(winner);
    }

    private void LocalEndGame(WinnerType winner)
    {
        if (desktopGameEndController && desktopGameEndController.isActiveAndEnabled)
        {
            desktopGameEndController.SetWinner(winner);
            desktopGameEndController.Show();
        }
    }

    private void StartGameTime()
    {
        _gameTimeLeft.Value = gameDuration;
        _isGameRunning = true;
    }

    private void UpdateGameTime()
    {
        _gameTimeLeft.Value -= Time.deltaTime;
        if (_gameTimeLeft.Value <= 0)
        {
            _gameTimeLeft.Value = 0;
            _isGameRunning = false;

            GameTimeDidEnd();
        }
    }

    private void GameTimeDidEnd()
    {
        EndGame(WinnerType.Desktop);
    }

    private IEnumerator StartGameCoroutine()
    {
        _gameState.Value = GameState.Initializing;

        StartGameTime();

        // TODO: spawn enemies

        // TODO: spawn players

        // TODO: show countdown

        // TODO: start game

        yield return null;
    }

    public enum GameState
    {
        Idle,
        Initializing,
        WaitingToStart,
        RandomWalking,
        Dancing,
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(NetworkFollowGameManager))]
public class NetworkFollowGameManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var networkFollowGameManager = (NetworkFollowGameManager)target;

        if (GUILayout.Button("Start Game"))
        {
            networkFollowGameManager.StartGame();
        }
    }
}
#endif