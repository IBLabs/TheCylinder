using System.Collections;
using System.Collections.Generic;

using Unity.Netcode;

using UnityEngine;

public class NetworkScoreKeeper : NetworkBehaviour
{
    public static NetworkScoreKeeper Instance { get; private set; }

    public int DesktopScore { get; private set; }
    public int XrScore { get; private set; }

    public WinnerType LastRoundWinner { get; private set; }

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

    public void AddDesktopScore()
    {
        DesktopScore += 1;
        LastRoundWinner = WinnerType.Desktop;
    }

    public void AddXrScore()
    {
        XrScore += 1;
        LastRoundWinner = WinnerType.VR;
    }

    private void SetInitialState()
    {
        LastRoundWinner = WinnerType.Unset;
    }
}
