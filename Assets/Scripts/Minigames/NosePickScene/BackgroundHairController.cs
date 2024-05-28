using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BackgroundHairController : MonoBehaviour
{
    public float duration = 1f; // Duration of one wiggle cycle
    public float strength = 10f; // Strength of the wiggle
    public Vector3 axis = Vector3.up; // Axis of rotation

    private float elapsedTime = 0f;
    private Vector3 initialRotation;

    void Start()
    {
        initialRotation = transform.localEulerAngles;
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;

        float noise = Mathf.PerlinNoise(elapsedTime / duration, 0f) * 2f - 1f;
        float angle = noise * strength;
        
        transform.localEulerAngles = initialRotation + axis * angle;
    }
}
