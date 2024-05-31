using UnityEngine;
using UnityEngine.AI;

public class SimpleNavmeshAgentController : MonoBehaviour
{
    [SerializeField] private float wanderRadius = 10f;

    [SerializeField] private float minWanderTimer = 1f;
    [SerializeField] private float maxWanderTimer = 3f;

    private NavMeshAgent agent;
    private float _timer;
    private float _currentTime;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
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
