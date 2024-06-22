using System.Collections;
using System.Linq;

using DG.Tweening;

using TMPro;

using Unity.Netcode;

using UnityEngine;

public class PrisonNetworkGameManager : NetworkBehaviour
{
    private const int LIGHTS_ON_TO_WIN = 3;

    [SerializeField] private SceneLoader sceneLoader;
    [SerializeField] private MeadowDesktopGameEndController desktopGameEndController;

    private int lightsOnCount = 0;

    public void OnLightTurnedOn()
    {
        if (!IsServer) return;

        lightsOnCount++;

        if (lightsOnCount >= LIGHTS_ON_TO_WIN)
        {
            Debug.Log("Lights on count reached, desktop players wins!");

            EndGame(WinnerType.Desktop);
        }
    }

    public void OnPlayerKilled()
    {
        StartCoroutine(CheckAllPlayersDead());
    }

    private IEnumerator ReloadSceneAfterDelay()
    {
        yield return new WaitForSeconds(3);
        sceneLoader.ReloadScene();
    }

    private IEnumerator CheckAllPlayersDead()
    {
        yield return new WaitForEndOfFrame();

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        var allPlayersDead = players.Length == 0 || players.All(player => player == null);

        Debug.Log("players death state: " + allPlayersDead + " " + players.Length);

        if (allPlayersDead)
        {
            Debug.Log("VR player wins!");

            EndGame(WinnerType.VR);
        }
    }

    private void EndGame(WinnerType winnerType)
    {
        if (!IsServer)
        {
            Debug.LogError("only the server can end a game, ignoring...");
            return;
        }

        NetworkScoreKeeper.Instance.AddScore(winnerType);

        ShowGameEnd(winnerType);

        ShowGameEndClientRpc(winnerType);
    }

    private void ShowGameEnd(WinnerType winnerType)
    {
        var gameEndController = FindAnyObjectByType<MeadowGameEndController>();
        gameEndController.ShowGameEndScreen(winnerType);
    }

    [ClientRpc]
    private void ShowGameEndClientRpc(WinnerType winnerType)
    {
        desktopGameEndController.SetWinner(winnerType);
        desktopGameEndController.Show();
    }
}
