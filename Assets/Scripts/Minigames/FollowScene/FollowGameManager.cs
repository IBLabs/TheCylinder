using System.Collections;
using System.Collections.Generic;

using Unity.Netcode;

using UnityEngine;

public class FollowGameManager : NetworkBehaviour
{
    public GameState CurrentGameState => _gameState.Value;

    [SerializeField] private float gameDuration = 120f;

    private NetworkVariable<GameState> _gameState = new NetworkVariable<GameState>(GameState.Idle);

    public void StartGame()
    {
        if (IsServer)
        {
            _gameState.Value = GameState.Initializing;
            StartCoroutine(StartGameCoroutine());
        }
    }

    private IEnumerator StartGameCoroutine()
    {
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
