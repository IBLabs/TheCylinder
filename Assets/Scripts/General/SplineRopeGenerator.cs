using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Splines;

public class SplineRopeGenerator : MonoBehaviour
{
    [SerializeField] private SplineContainer splineContainer;
    [SerializeField] private Transform targetObject;

    void Start()
    {
        if (splineContainer == null)
        {
            splineContainer = GetComponent<SplineContainer>();
        }
    }

    void Update()
    {
        HandleSpline();
    }

    private void HandleSpline()
    {
        if (splineContainer == null)
        {
            return;
        }

        if (splineContainer.Spline.Count > 0)
        {
            int lastKnotIndex = splineContainer.Spline.Count - 1;
            var lastKnot = splineContainer.Spline.ToArray()[lastKnotIndex];

            // Update the position of the last knot to match the target object's position
            // lastKnot.Position = targetObject.transform.position;

            lastKnot.Position = splineContainer.transform.InverseTransformPoint(targetObject.transform.position);
            lastKnot.Rotation = Quaternion.Inverse(splineContainer.transform.rotation) * targetObject.transform.rotation;

            splineContainer.Spline.SetKnot(lastKnotIndex, lastKnot);
        }
    }
}
