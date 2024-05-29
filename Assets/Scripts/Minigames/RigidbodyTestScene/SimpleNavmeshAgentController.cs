using System.Collections;
using System.Collections.Generic;

using Unity.XR.CoreUtils;

using UnityEngine;
using UnityEngine.AI;

public class SimpleNavmeshAgentController : MonoBehaviour
{
    [SerializeField] private Transform target;

    private NavMeshAgent _agent;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();

        if (target == null)
        {
            var xrOrigin = FindFirstObjectByType<XROrigin>();
            if (xrOrigin != null)
            {
                target = xrOrigin.transform;
            }
        }

        _agent.SetDestination(target.position);
    }
}
