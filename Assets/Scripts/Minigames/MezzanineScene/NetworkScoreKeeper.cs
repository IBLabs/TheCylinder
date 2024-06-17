using System.Collections;
using System.Collections.Generic;

using Unity.Netcode;

using UnityEngine;

public class NetworkScoreKeeper : NetworkBehaviour
{
    public static NetworkScoreKeeper Instance { get; private set; }

    public readonly NetworkVariable<int> DesktopScore = new NetworkVariable<int>(0);
    public readonly NetworkVariable<int> XrScore = new NetworkVariable<int>(0);

    public WinnerType LastRoundWinner => _lastRoundWinner.Value;

    private readonly NetworkVariable<WinnerType> _lastRoundWinner = new NetworkVariable<WinnerType>();

    void Awake()
    {
        if (Instance == null)
        {
            SetInitialState();

            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddScore(WinnerType winnerType)
    {
        if (winnerType == WinnerType.Desktop)
        {
            AddDesktopScore();
        }
        else
        {
            AddXrScore();
        }
    }

    public override void OnNetworkSpawn()
    {
        Debug.Log("network score keeper spawned");

        base.OnNetworkSpawn();
    }

    public void AddDesktopScore()
    {
        if (!IsServer)
        {
            Debug.Log("only server can increase desktop score, returning");
            return;
        }

        Debug.Log("server adding score to desktop player");

        DesktopScore.Value += 1;
        _lastRoundWinner.Value = WinnerType.Desktop;
    }

    public void AddXrScore()
    {
        if (!IsServer)
        {
            Debug.Log("only server can increase XR score, returning");
            return;
        }

        Debug.Log("server adding score to XR player");

        XrScore.Value += 1;
        _lastRoundWinner.Value = WinnerType.VR;
    }

    private void SetInitialState()
    {
        if (!IsServer) return;

        _lastRoundWinner.Value = WinnerType.Unset;
    }
}
