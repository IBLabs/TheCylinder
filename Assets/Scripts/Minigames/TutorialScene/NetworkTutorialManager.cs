using System.Collections;
using System.Collections.Generic;

using TMPro;

using Unity.Netcode;

using UnityEngine;
using UnityEngine.Playables;

public class NetworkTutorialManager : NetworkBehaviour
{
    private const string STEP_ID_KILL_PLAYER = "kill_player";
    private const string STEP_ID_HIT_ENEMY = "hit_enemy";

    [SerializeField] private PlayableDirector director;

    [SerializeField] private NetworkSimplePlayerSpawner networkPlayerSpawner;
    [SerializeField] private NetworkAgentSpawner networkAgentSpawner;
    [SerializeField] private AgentDuplicator agentDuplicator;
    [SerializeField] private NetworkPrisonLockController lockController;
    [SerializeField] private TextMeshPro xrTaskText;
    [SerializeField] private TextMeshProUGUI desktopTaskText;

    [SerializeField] private bool autoStart = true;
    [SerializeField] private TutorialStep[] steps;

    private int _currentStepIndex = 0;
    private bool _shouldContinue = false;
    private bool _finishedPlayingStep = false;

    void Start()
    {
        ListenForPlayerShooterEvents();
        ListenForPlayableDirectorEvents();

        var hasNetworkAccess = NetworkManager.Singleton != null;
        if (!hasNetworkAccess)
        {
            if (autoStart)
            {
                StartCoroutine(TutorialCoroutine());
            }
        }
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            StartCoroutine(TutorialCoroutine());
        }

        base.OnNetworkSpawn();
    }

    // TODO: remove, testing only
    private IEnumerator SpawnEnemyCoroutine()
    {
        yield return new WaitForSeconds(1f);

        networkAgentSpawner.SpawnAgentAtRandomSpawnPoint();
    }

    // TODO: remove, testing only
    private IEnumerator LockCourtine()
    {
        yield return new WaitForSeconds(2f);

        lockController.TurnOn();
    }

    public void NextStep()
    {
        _shouldContinue = true;
    }

    private void ListenForPlayerShooterEvents()
    {
        var shooters = FindObjectsByType<XRPlayerShooter>(FindObjectsSortMode.None);

        foreach (var shooter in shooters)
        {
            shooter.OnPlayerKilled.AddListener(OnPlayerKilled);
            shooter.OnEnemyHit.AddListener(OnEnemyHit);
        }
    }

    private void ListenForPlayableDirectorEvents()
    {
        director.played += OnPlayableDirectorPlayed;
        director.stopped += OnPlayableDirectorStopped;
    }

    private void RemovePlayableDirectorEvents()
    {
        director.played -= OnPlayableDirectorPlayed;
        director.stopped -= OnPlayableDirectorStopped;
    }

    private IEnumerator TutorialCoroutine()
    {
        var hasNetworkAccess = NetworkManager.Singleton != null;
        if (hasNetworkAccess)
        {
            if (!IsServer)
            {
                Debug.Log("only the server can start the tutorial, ignoring...");
                yield break;
            }
        }

        for (int i = 0; i < steps.Length; i++)
        {
            _currentStepIndex = i;

            yield return StartCoroutine(PlayStepCoroutine(i));
        }

        Debug.Log("tutorial finished");
    }

    private IEnumerator PlayStepCoroutine(int stepIndex)
    {
        var hasNetworkAccess = NetworkManager.Singleton != null;
        if (hasNetworkAccess)
        {
            if (!IsServer)
            {
                Debug.Log("only the server can play a step, ignoring...");
                yield break;
            }

            PlayStepClientRpc(stepIndex);
        }
        else
        {
            LocalPlayStep(stepIndex);
        }

        yield return new WaitUntil(() => _finishedPlayingStep);

        yield return new WaitUntil(() => _shouldContinue);

        _shouldContinue = false;

        _finishedPlayingStep = false;
    }

    [ClientRpc]
    private void PlayStepClientRpc(int stepIndex)
    {
        LocalPlayStep(stepIndex);
    }

    private void LocalPlayStep(int stepIndex)
    {
        _currentStepIndex = stepIndex;

        var tutorialStep = steps[stepIndex];

        director.playableAsset = tutorialStep.timelineAsset;

        ListenForPlayableDirectorEvents();

        director.Play();
    }

    private IEnumerator RespawnCoroutine(ulong clientId)
    {
        yield return new WaitForSeconds(1f);

        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { clientId }
            }
        };

        RequestSpawnClientRpc(clientRpcParams);
    }

    [ClientRpc]
    private void RequestSpawnClientRpc(ClientRpcParams clientRpcParams = default)
    {
        var localId = NetworkManager.Singleton.LocalClientId;
        networkPlayerSpawner.RequestSpawnFromServer(localId);
    }

    #region Events

    private void OnPlayerKilled(ulong clientId)
    {
        Debug.Log("player killed");

        StartCoroutine(RespawnCoroutine(clientId));

        if (steps[_currentStepIndex].stepId == STEP_ID_KILL_PLAYER)
        {
            if (!_finishedPlayingStep) return;

            Debug.Log("player killed on correct step, moving to next step");

            NextStep();
        }
        else
        {
            Debug.Log("player killed on wrong step");
        }
    }

    private void OnEnemyHit(GameObject hitObject)
    {
        agentDuplicator.OnEnemyHit(hitObject);

        if (steps[_currentStepIndex].stepId == STEP_ID_HIT_ENEMY)
        {
            if (!_finishedPlayingStep) return;

            agentDuplicator.duplicatorEnabled = false;

            NextStep();
        }
    }

    public void OnUnlock()
    {
        if (steps[_currentStepIndex].stepId == "unlock")
        {
            NextStep();
        }
    }

    private void OnPlayableDirectorPlayed(PlayableDirector obj)
    {
        Debug.Log("started playing timline step!");
    }

    private void OnPlayableDirectorStopped(PlayableDirector obj)
    {
        Debug.Log("reached finished playing step");

        if (obj == director)
        {
            Debug.Log("finished playing step on correct director");

            _finishedPlayingStep = true;

            if (steps[_currentStepIndex].autoContinue)
            {
                _shouldContinue = true;
            }
        }
    }

    public void OnReachedKillPlayerSignal()
    {
        Debug.Log("reached kill player signal");
    }

    public void OnReachedKillEnemySignal()
    {
        Debug.Log("reached kill enemy signal");

        networkAgentSpawner.SpawnAgentAtRandomSpawnPoint();
    }

    public void OnReachedUnlockSignal()
    {
        Debug.Log("reached unlock signal");

        if (IsServer)
        {
            lockController.TurnOn();
        }
    }

    public void OnReachedUpdateTaskText()
    {
        Debug.Log("reached update task text");

        xrTaskText.text = steps[_currentStepIndex].xrText;
        desktopTaskText.text = steps[_currentStepIndex].desktopText;
    }

    #endregion
}