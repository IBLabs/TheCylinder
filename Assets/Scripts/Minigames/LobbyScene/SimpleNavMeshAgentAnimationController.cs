using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AI;

public class SimpleNavMeshAgentAnimationController : MonoBehaviour
{
    private const string IS_WALKING = "isWalking";

    [SerializeField] private Animator animator;
    [SerializeField] private NavMeshAgent agent;

    void Update()
    {
        if (agent.velocity.magnitude > 0.01f)
        {
            animator.SetBool(IS_WALKING, true);
        }
        else
        {
            animator.SetBool(IS_WALKING, false);
        }
    }
}
