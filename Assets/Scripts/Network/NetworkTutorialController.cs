using System.Collections;

using UnityEngine;

using Unity.Netcode;

using DG.Tweening;

using TMPro;
using UnityEngine.Playables;

public class NetworkTutorialController : NetworkBehaviour
{
    [Header("XR Components")]
    [SerializeField] private Transform xrOriginTransform;
    [SerializeField] private CanvasGroup xrInstructionsGroup;
    [SerializeField] private CanvasGroup xrCountdownGroup;
    [SerializeField] private ITransitionController xrTransitionController;
    [SerializeField] private Transform xrTutorialStartTransform;

    [SerializeField] private PlayableDirector xrDirector;

    [Header("Desktop Components")]
    [SerializeField] private CanvasGroup desktopInstructionsGroup;
    [SerializeField] private CanvasGroup desktopCountdownGroup;
    [SerializeField] private ITransitionController desktopTransitionController;

    [SerializeField] private PlayableDirector desktopDirector;

    [Header("Configuration")]
    [SerializeField] private bool showOnStart = true;

    private Vector3 _originalPosition;

    void Start()
    {
        if (showOnStart)
        {
            ShowTutorial();
        }
    }

    public void ShowTutorial()
    {
        _originalPosition = xrOriginTransform.position;

        xrOriginTransform.position = xrTutorialStartTransform.position;
        xrOriginTransform.rotation = xrTutorialStartTransform.rotation;

        xrCountdownGroup.alpha = 0;
        xrInstructionsGroup.alpha = 1;

        xrDirector.Play();

        ShowTutorialClientRpc();
    }

    public void HideTutorial()
    {
        StartCoroutine(FinishTutorialCoroutine());
    }

    private IEnumerator FinishTutorialCoroutine()
    {
        FadeOutInstructionsClientRpc();

        yield return xrInstructionsGroup.DOFade(0, .3f).WaitForCompletion();

        yield return new WaitForSeconds(.5f);

        for (int i = 3; i > 0; i--)
        {
            xrCountdownGroup.GetComponentInChildren<TextMeshProUGUI>().text = i.ToString();

            CountdownStepClientRpc(i.ToString());

            yield return xrCountdownGroup.DOFade(0, 1f).From(1).WaitForCompletion();
        }

        TransitionToWorldClientRpc();

        xrOriginTransform.position = _originalPosition;

        xrTransitionController.FadeToScene();
    }

    [ClientRpc]
    private void ShowTutorialClientRpc()
    {
        if (IsServer) return;

        desktopDirector.Play();

        desktopCountdownGroup.alpha = 0;
        desktopInstructionsGroup.alpha = 1;
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
}
