using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class CircleFollowTarget : MonoBehaviour
{
    public Transform target;

    private void Update()
    {
        Quaternion targetRotation = Quaternion.Euler(0f, target.rotation.eulerAngles.y, 0f);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime);
    }
}
