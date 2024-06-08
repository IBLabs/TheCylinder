using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using DG.Tweening;

using TMPro;

using Unity.Netcode;

using UnityEngine;
using UnityEngine.Rendering;

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

    [Header("Tutorial Related")]
    [SerializeField] private Transform xrOriginTransform;
    [SerializeField] private CanvasGroup instructionsGroup;
    [SerializeField] private CanvasGroup countdownGroup;
    [SerializeField] private TransitionSphereController transitionController;
    [SerializeField] private CanvasGroup desktopInstructionsGroup;
    [SerializeField] private CanvasGroup desktopCountdownGroup;
    [SerializeField] private SliderTransitionController desktopTransitionController;

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

    #region Tutorial Related

    public void FinishTutorial()
    {
        StartCoroutine(FinishTutorialCoroutine());
    }

    private IEnumerator FinishTutorialCoroutine()
    {
        FadeOutInstructionsClientRpc();

        yield return instructionsGroup.DOFade(0, .3f).WaitForCompletion();

        yield return new WaitForSeconds(.5f);

        for (int i = 3; i > 0; i--)
        {
            countdownGroup.GetComponentInChildren<TextMeshProUGUI>().text = i.ToString();

            CountdownStepClientRpc(i.ToString());

            yield return countdownGroup.DOFade(0, 1f).From(1).WaitForCompletion();
        }

        TransitionToWorldClientRpc();

        xrOriginTransform.position = Vector3.zero;

        transitionController.FadeToWorld();
    }

    [ClientRpc]
    private void FadeOutInstructionsClientRpc()
    {
        if (IsServer) return;

        desktopInstructionsGroup.DOFade(0, .3f);
    }

    [ClientRpc]
    private void CountdownStepClientRpc(string text)
    {
        if (IsServer) return;

        desktopCountdownGroup.GetComponentInChildren<TextMeshProUGUI>().text = text;
        desktopCountdownGroup.DOFade(0, 1f).From(1);
    }

    [ClientRpc]
    private void TransitionToWorldClientRpc()
    {
        if (IsServer) return;

        desktopTransitionController.FadeToScene();
    }

    #endregion
}

enum WinnerType
{
    Desktop,
    VR
}
