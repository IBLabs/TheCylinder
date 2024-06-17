using System;
using System.Collections;
using System.Collections.Generic;

using Unity.Netcode;

using UnityEngine;

public class NetworkMezzanineManager : NetworkBehaviour
{
    [SerializeField] private ScoreCanvasController desktopScoreCanvasController;
    [SerializeField] private ScoreCanvasController xrScoreCanvasController;

    [SerializeField] private MezzanineCanvasController desktopMezzanineCanvasController;
    [SerializeField] private MezzanineCanvasController xrMezzanineCanvasController;


    public override void OnNetworkSpawn()
    {
        var lastRoundWinner = NetworkScoreKeeper.Instance.LastRoundWinner;

        Debug.Log("network mezzanine manager received last round winner: " + lastRoundWinner.ToString());

        if (lastRoundWinner != WinnerType.Unset)
        {
            Debug.Log("showing score canvas");
            ShowScore();
        }
        else
        {
            Debug.Log("showing level selection canvas");
            ShowLevelSelection();
        }

        base.OnNetworkSpawn();
    }

    public void OnScoreCanvasFinished()
    {
        if (desktopMezzanineCanvasController && desktopMezzanineCanvasController.gameObject.activeInHierarchy)
        {
            desktopMezzanineCanvasController.ShowLevelSelectionCanvas();
        }

        if (xrMezzanineCanvasController && xrMezzanineCanvasController.gameObject.activeInHierarchy)
        {
            xrMezzanineCanvasController.ShowLevelSelectionCanvas();
        }
    }

    void OnEnable()
    {
        if (desktopScoreCanvasController)
            desktopScoreCanvasController.OnScoreCanvasFinished.AddListener(OnScoreCanvasFinished);

        if (xrScoreCanvasController)
            xrScoreCanvasController.OnScoreCanvasFinished.AddListener(OnScoreCanvasFinished);
    }

    void OnDisable()
    {
        if (desktopScoreCanvasController)
            desktopScoreCanvasController.OnScoreCanvasFinished.RemoveListener(OnScoreCanvasFinished);

        if (xrScoreCanvasController)
            xrScoreCanvasController.OnScoreCanvasFinished.RemoveListener(OnScoreCanvasFinished);
    }

    private void ShowScore()
    {
        StartCoroutine(ShowScoreCoroutine());
    }

    private IEnumerator ShowScoreCoroutine()
    {
        NetworkScoreKeeper scoreKeeper = NetworkScoreKeeper.Instance;

        desktopMezzanineCanvasController.ShowScoreCanvas();
        desktopScoreCanvasController.ShowLastRoundWinner(scoreKeeper.LastRoundWinner, scoreKeeper.DesktopScore.Value, scoreKeeper.XrScore.Value);

        xrMezzanineCanvasController.ShowScoreCanvas();
        xrScoreCanvasController.ShowLastRoundWinner(scoreKeeper.LastRoundWinner, scoreKeeper.DesktopScore.Value, scoreKeeper.XrScore.Value);

        yield return null;
    }

    private void ShowLevelSelection()
    {
        if (desktopMezzanineCanvasController && desktopMezzanineCanvasController.gameObject.activeInHierarchy)
        {
            desktopMezzanineCanvasController.ShowLevelSelectionCanvas();
        }

        if (xrMezzanineCanvasController && xrMezzanineCanvasController.gameObject.activeInHierarchy)
        {
            xrMezzanineCanvasController.ShowLevelSelectionCanvas();
        }
    }
}
