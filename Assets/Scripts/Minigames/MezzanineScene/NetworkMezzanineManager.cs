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

    void Start()
    {
        var lastRoundWinner = NetworkScoreKeeper.Instance.LastRoundWinner;
        if (lastRoundWinner != WinnerType.Unset)
        {
            ShowScore();
        }
        else
        {
            ShowLevelSelection();
        }
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
        if (desktopScoreCanvasController) desktopScoreCanvasController.OnScoreCanvasFinished.AddListener(OnScoreCanvasFinished);
        if (xrScoreCanvasController) xrScoreCanvasController.OnScoreCanvasFinished.AddListener(OnScoreCanvasFinished);
    }

    void OnDisable()
    {
        if (desktopScoreCanvasController) desktopScoreCanvasController.OnScoreCanvasFinished.RemoveListener(OnScoreCanvasFinished);
        if (xrScoreCanvasController) xrScoreCanvasController.OnScoreCanvasFinished.RemoveListener(OnScoreCanvasFinished);
    }

    private void ShowScore()
    {
        if (desktopScoreCanvasController && desktopScoreCanvasController.gameObject.activeInHierarchy)
        {
            desktopMezzanineCanvasController.ShowScoreCanvas();
            desktopScoreCanvasController.ShowLastRoundWinner(NetworkScoreKeeper.Instance.LastRoundWinner);
        }

        if (xrScoreCanvasController && xrScoreCanvasController.gameObject.activeInHierarchy)
        {
            xrMezzanineCanvasController.ShowScoreCanvas();
            xrScoreCanvasController.ShowLastRoundWinner(NetworkScoreKeeper.Instance.LastRoundWinner);
        }
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
