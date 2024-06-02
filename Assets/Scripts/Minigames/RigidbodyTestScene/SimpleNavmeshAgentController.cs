using UnityEngine;
using UnityEngine.AI;

public class SimpleNavmeshAgentController : MonoBehaviour
{
    [SerializeField] private float wanderRadius = 10f;
    [SerializeField] private float minWanderTimer = 1f;
    [SerializeField] private float maxWanderTimer = 3f;
    [SerializeField] private float rotationSpeed = 3f;

    private NavMeshAgent agent;
    private float _timer;
    private float _currentTime;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
    }

    private void Update()
    {
        _timer += Time.deltaTime;

        if (_timer >= _currentTime)
        {
            Vector3 newPos = RandomNavmeshLocation(wanderRadius);
            agent.SetDestination(newPos);
            _timer = 0f;
            _currentTime = Random.Range(minWanderTimer, maxWanderTimer);
        }

        if (agent.velocity.sqrMagnitude > 0)
        {
            Quaternion lookRotation = Quaternion.LookRotation(agent.velocity.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }
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
