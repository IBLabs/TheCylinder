using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class SimpleLookFollowTarget : MonoBehaviour
{
    public Transform target;
    public Vector3 lookAxis = Vector3.forward;

    void Update()
    {
        Vector3 direction = target.position - transform.position;

        Quaternion targetRotation = Quaternion.LookRotation(-direction, lookAxis);

        transform.rotation = targetRotation;
    }
}
