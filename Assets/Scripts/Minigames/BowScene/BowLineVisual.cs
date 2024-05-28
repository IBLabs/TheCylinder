using System.Collections;
using System.Collections.Generic;

using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class BowLineVisual : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;

    [SerializeField] private Transform topPoint;
    [SerializeField] private Transform middlePoint;
    [SerializeField] private Transform bottomPoint;

    void Start()
    {
        if (!lineRenderer)
        {
            lineRenderer = GetComponent<LineRenderer>();
        }
    }

    void Update()
    {
        UpdateLinePositions();
    }

    private void UpdateLinePositions()
    {
        if (!lineRenderer) return;

        lineRenderer.SetPositions(new Vector3[] { topPoint.position, middlePoint.position, bottomPoint.position });
    }
}
