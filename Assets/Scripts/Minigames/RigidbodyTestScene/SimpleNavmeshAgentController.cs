using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class SimpleNavmeshAgentController : MonoBehaviour
{
    private const string IS_WALKING = "isWalking";

    public bool IsPaused => _isPaused;

    [SerializeField] private Animator animator;

    [SerializeField] private float wanderRadius = 10f;
    [SerializeField] private float minWanderTimer = 1f;
    [SerializeField] private float maxWanderTimer = 3f;
    [SerializeField] private float rotationSpeed = 3f;

    private NavMeshAgent agent;
    private float _timer;
    private float _currentTime;

    private bool _isPaused = false;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;

        if (animator == null)
        {
            Debug.LogError("Animator is not set on " + gameObject.name);
        }
    }

    private void Update()
    {
        if (!_isPaused)
        {
            _timer += Time.deltaTime;

            if (_timer >= _currentTime)
            {
                Vector3 newPos = RandomNavmeshLocation(wanderRadius);
                agent.SetDestination(newPos);
                _timer = 0f;
                _currentTime = Random.Range(minWanderTimer, maxWanderTimer);
            }
        }

        if (agent.velocity.sqrMagnitude > 0)
        {
            Quaternion lookRotation = Quaternion.LookRotation(agent.velocity.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }

        if (agent.velocity.magnitude > 0.01f)
        {
            animator.SetBool(IS_WALKING, true);
        }
        else
        {
            animator.SetBool(IS_WALKING, false);
        }
    }

    public void Pause()
    {
        _isPaused = true;
    }

    public void Resume()
    {
        _isPaused = false;
    }

    private Vector3 RandomNavmeshLocation(float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += transform.position;
        NavMeshHit navHit;
        NavMesh.SamplePosition(randomDirection, out navHit, radius, -1);
        return navHit.position;
    }
}
