using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class SimpleAxisWiggle : MonoBehaviour
{
    public float amplitude = 1f;
    public float frequency = 1f;
    public Vector3 axis = Vector3.up;

    private Vector3 originalPosition;

    private void Start()
    {
        originalPosition = transform.localPosition;
    }

    private void Update()
    {
        float noise = Mathf.PerlinNoise(Time.time * frequency, 0f);
        Vector3 offset = axis * amplitude * noise;
        transform.localPosition = originalPosition + offset;
    }
}
