using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class NetworkTransitionCoordinator : NetworkBehaviour
{
    public ITransitionController transitionController;

    void Start()
    {
        ConfigureTransitionController();
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            ListenForSceneLoaderEvents();
        }

        base.OnNetworkSpawn();
    }

    public void OnTransitionWillStart(float duration)
    {
        if (IsServer)
        {
            StartOutTransitionClientRpc(duration);
        }
    }

    [ClientRpc]
    private void StartOutTransitionClientRpc(float duration)
    {
        if (IsServer) return;

        transitionController.FadeToBlack();
    }

    private void ConfigureTransitionController()
    {
        if (transitionController == null)
        {
            var controllers = FindObjectsByType<ITransitionController>(FindObjectsSortMode.None);

            foreach (var controller in controllers)
            {
                if (controller.isActiveAndEnabled)
                {
                    transitionController = controller;
                    break;
                }
            }

            if (transitionController == null)
            {
                Debug.LogError("No transition controller found in scene.");
                return;
            }
        }
    }

    private void ListenForSceneLoaderEvents()
    {
        var sceneLoader = FindAnyObjectByType<SceneLoader>();
        if (sceneLoader == null)
        {
            Debug.LogError("No scene loader found in scene.");
            return;
        }

        sceneLoader.TransitionWillStart.AddListener(OnTransitionWillStart);
    }
}