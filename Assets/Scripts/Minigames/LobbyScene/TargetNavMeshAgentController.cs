using System.Collections;
using System.Collections.Generic;

using Unity.VisualScripting;

using UnityEngine;
using UnityEngine.AI;

public class TargetNavMeshAgentController : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform target;

    void Start()
    {
        agent.SetDestination(target.position);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "NavMeshTarget")
        {
            Debug.Log("Target reached");

            var targets = GameObject.FindGameObjectsWithTag("NavMeshTarget");
            var newTarget = targets[Random.Range(0, targets.Length)];
            while (newTarget.transform.position == target.position)
            {
                newTarget = targets[Random.Range(0, targets.Length)];
            }

            Debug.Log("setting new target to " + newTarget.name);

            target = newTarget.transform;
            agent.SetDestination(target.position);
        }
    }
}
