using System.Collections;
using System.Collections.Generic;

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

    [SerializeField] private bool autoStart = true;
    [SerializeField] private TutorialStep[] steps;

    private int _currentStepIndex = 0;
    private bool _shouldContinue = false;
    private bool _finishedPlayingStep = false;

    void Start()
    {
        ListenForPlayerShooterEvents();

        ListenForPlayableDirectorEvents();

        if (autoStart)
        {
            StartCoroutine(TutorialCoroutine());
        }
    }

    public override void OnNetworkSpawn()
    {
        // TODO: remove, testing only
        if (IsServer)
        {
            StartCoroutine(SpawnEnemyCoroutine());
        }

        base.OnNetworkSpawn();
    }

    // TODO: remove, testing only
    private IEnumerator SpawnEnemyCoroutine()
    {
        yield return new WaitForSeconds(1f);

        networkAgentSpawner.SpawnAgentAtRandomSpawnPoint();
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

    private IEnumerator TutorialCoroutine()
    {
        for (int i = 0; i < steps.Length; i++)
        {
            _currentStepIndex = i;

            var tutorialStep = steps[i];

            director.playableAsset = tutorialStep.timelineAsset;
            director.Play();

            yield return new WaitUntil(() => _finishedPlayingStep);

            _finishedPlayingStep = false;

            yield return new WaitUntil(() => _shouldContinue);

            _shouldContinue = false;
        }

        Debug.Log("tutorial finished");
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
        if (steps[_currentStepIndex].stepId == STEP_ID_KILL_PLAYER)
        {
            NextStep();
        }

        StartCoroutine(RespawnCoroutine(clientId));
    }

    private void OnEnemyHit(GameObject hitObject)
    {
        if (steps[_currentStepIndex].stepId == STEP_ID_HIT_ENEMY)
        {
            NextStep();
        }

        agentDuplicator.OnEnemyHit(hitObject);
    }

    private void OnPlayableDirectorPlayed(PlayableDirector obj)
    {
    }

    private void OnPlayableDirectorStopped(PlayableDirector obj)
    {
        if (obj == director)
        {
            _finishedPlayingStep = true;

            if (steps[_currentStepIndex].autoContinue)
            {
                _shouldContinue = true;
            }
        }
    }

    #endregion
}