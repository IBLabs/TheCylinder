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

    [SerializeField] private CanvasGroup gameEndGroup;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI subtitleText;

    [SerializeField] private CanvasGroup vrGameEndGroup;
    [SerializeField] private TextMeshProUGUI vrTitleText;
    [SerializeField] private TextMeshProUGUI vrSubtitleText;

    private int lightsOnCount = 0;

    public void OnLightTurnedOn()
    {
        if (!IsServer) return;

        lightsOnCount++;

        if (lightsOnCount >= LIGHTS_ON_TO_WIN)
        {
            Debug.Log("Lights on count reached, desktop players wins!");

            // StartCoroutine(ReloadSceneAfterDelay());

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

            // StartCoroutine(ReloadSceneAfterDelay());

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

        ShowGameEnd(winnerType);

        ShowGameEndClientRpc(winnerType);
    }

    private void ShowGameEnd(WinnerType winnerType)
    {
        vrTitleText.text = winnerType == WinnerType.Desktop ? "OUTLAWS WIN" : "ENFORCER WINS";
        vrSubtitleText.text = WinnerType.VR == winnerType ? "GOOD JOB!" : "BETTER LUCK NEXT TIME!";

        vrGameEndGroup.DOFade(1, .3f);
        vrGameEndGroup.interactable = true;
        vrGameEndGroup.blocksRaycasts = true;
    }

    [ClientRpc]
    private void ShowGameEndClientRpc(WinnerType winnerType)
    {
        titleText.text = winnerType == WinnerType.Desktop ? "OUTLAWS WIN" : "ENFORCER WINS";
        subtitleText.text = WinnerType.Desktop == winnerType ? "GOOD JOB!" : "BETTER LUCK NEXT TIME!";

        gameEndGroup.DOFade(1, .3f);
        gameEndGroup.interactable = true;
        gameEndGroup.blocksRaycasts = true;
    }
}

enum WinnerType
{
    Desktop,
    VR
}
