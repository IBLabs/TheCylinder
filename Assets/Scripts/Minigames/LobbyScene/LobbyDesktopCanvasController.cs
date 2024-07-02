using System.Collections;

using DG.Tweening;

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class LobbyDesktopCanvasController : MonoBehaviour
{
    private const string TRIGGER_CONNECT = "Connect";
    private const string TRIGGER_EXIT = "Exit";

    [SerializeField] private Animator entryScreenAnimator;
    [SerializeField] private NetworkRelayManager relayManager;
    [SerializeField] private DoorTransitionController transitionController;
    [SerializeField] private Volume postProcessingVolume;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            AttemptJoinRelay();
        }
    }

    public void OnClientConnected(ulong clientId)
    {
        StartCoroutine(ClientConnectedCoroutine());
    }

    private IEnumerator ClientConnectedCoroutine()
    {
        entryScreenAnimator.SetTrigger(TRIGGER_EXIT);

        yield return new WaitForSeconds(1f);

        DisableFilmGrain();

        transitionController.FadeToScene();
    }

    public void DidClickButton()
    {
        AttemptJoinRelay();
    }

    private void AttemptJoinRelay()
    {
        if (relayManager.JoinRelay())
        {
            entryScreenAnimator.SetTrigger(TRIGGER_CONNECT);
        }
    }

    private void DisableFilmGrain()
    {
        FilmGrain filmGrain = null;

        if (postProcessingVolume.profile.TryGet<FilmGrain>(out filmGrain))
        {
            filmGrain.active = false;
        }
    }
}
