using System.Collections;

using UnityEngine;
using UnityEngine.AI;

public class FollowAgentController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5.0f;

    private NavMeshAgent _agent;
    private SimpleNavmeshAgentController _simpleController;

    private Coroutine _activeCoroutine;

    private AgentState _currentState = AgentState.Idle;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _simpleController = GetComponent<SimpleNavmeshAgentController>();
    }

    public void PerformMoveCommand(FollowMoveCommand moveCommand)
    {
        if (_activeCoroutine != null) { StopCoroutine(_activeCoroutine); }
        _activeCoroutine = StartCoroutine(MoveCommandCoroutine(moveCommand));
    }

    public void StartWandering()
    {
        _simpleController.Resume();
    }

    private IEnumerator MoveCommandCoroutine(FollowMoveCommand command)
    {
        _currentState = AgentState.Moving;

        if (!_simpleController.IsPaused)
        {
            _simpleController.Pause();
        }

        Vector3 newDestination;

        newDestination = transform.position + command.Direction * 10;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(newDestination, out hit, 50.0f, NavMesh.AllAreas))
        {
            newDestination = hit.position;
        }
        else
        {
            Debug.LogError("Failed to find a point on the NavMesh close to newDestination.");
        }

        _agent.SetDestination(newDestination);

        yield return new WaitForSeconds(command.Time);

        _currentState = AgentState.Idle;
    }

    private enum AgentState
    {
        Idle,
        Moving
    }
}
