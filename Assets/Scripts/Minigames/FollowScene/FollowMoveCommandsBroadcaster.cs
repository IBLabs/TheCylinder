using System.Collections;

using UnityEngine;
using UnityEngine.Events;
using Unity.Netcode;




#if UNITY_EDITOR
using UnityEditor;
#endif

public class FollowMoveCommandsBroadcaster : MonoBehaviour
{
    [SerializeField] private float minCommandDuration = 1f;
    [SerializeField] private float maxCommandDuration = 3f;

    [Header("Events")]
    public UnityEvent<FollowMoveCommand[]> WillBroadcastMoveCommands;
    public UnityEvent<FollowMoveCommand, FollowMoveCommand> DidBroadcastMoveCommand;

    private NetworkAgentSpawner _agentSpawner;

    void Start()
    {
        _agentSpawner = FindAnyObjectByType<NetworkAgentSpawner>();
    }

    public void BoradcastRandomMoveCommands(int amount)
    {
        var moveCommands = GenerateRandomMoveCommands(amount);
        WillBroadcastMoveCommands?.Invoke(moveCommands);
        StartCoroutine(BroadcastMoveCommandsCoroutine(moveCommands));
    }

    private FollowMoveCommand[] GenerateRandomMoveCommands(int amount)
    {
        FollowMoveCommand[] moveCommands = new FollowMoveCommand[amount];
        for (int i = 0; i < amount; i++)
        {
            var targetDirectionType = (FollowMoveCommand.DirectionType)Random.Range(0, 5);
            while (targetDirectionType == FollowMoveCommand.DirectionType.None)
            {
                targetDirectionType = (FollowMoveCommand.DirectionType)Random.Range(0, 5);
            }

            if (i > 0)
            {
                while (targetDirectionType == moveCommands[i - 1].directionType || targetDirectionType == FollowMoveCommand.DirectionType.None)
                {
                    targetDirectionType = (FollowMoveCommand.DirectionType)Random.Range(0, 5);
                }
            }

            moveCommands[i] = new FollowMoveCommand(targetDirectionType, Random.Range(minCommandDuration, maxCommandDuration));
        }

        return moveCommands;
    }

    private IEnumerator BroadcastMoveCommandsCoroutine(FollowMoveCommand[] moveCommands)
    {
        for (int i = 0; i < moveCommands.Length; i++)
        {
            foreach (var agent in _agentSpawner.SpawnedAgents)
            {
                var agentController = agent.GetComponent<FollowAgentController>();
                agentController.PerformMoveCommand(moveCommands[i]);
            }

            FollowMoveCommand currentCommand = moveCommands[i];
            FollowMoveCommand nextCommand = i + 1 < moveCommands.Length ? moveCommands[i + 1] : FollowMoveCommand.None;

            DidBroadcastMoveCommand?.Invoke(currentCommand, nextCommand);

            yield return new WaitForSeconds(moveCommands[i].Time);
        }

        foreach (var agent in _agentSpawner.SpawnedAgents)
        {
            var agentController = agent.GetComponent<FollowAgentController>();
            agentController.StartWandering();
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(FollowMoveCommandsBroadcaster))]
public class FollowMoveCommandsBroadcasterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        FollowMoveCommandsBroadcaster broadcaster = (FollowMoveCommandsBroadcaster)target;
        if (GUILayout.Button("Test Broadcast"))
        {
            broadcaster.BoradcastRandomMoveCommands(3);
        }
    }
}
#endif