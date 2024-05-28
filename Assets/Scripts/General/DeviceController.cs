using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace DefaultNamespace
{
    public class DeviceController : MonoBehaviour
    {
        [Header("Input Properties")] 
        [SerializeField] private InputActionProperty selectActionValue;
        [SerializeField] private InputActionProperty activateActionValue;
        
        [Header("Spin Configuration")]
        public GameObject spinningObject;
        public float maxSpinSpeed = 100f;
        public float spinSpeedMultiplier = 1f;

        [Header("Emission Configuration")]
        public int material1Index;
        public int material2Index;

        [Header("Tilt Configuration")]
        public GameObject tiltingObject;
        public float maxTiltDegrees = 30f;

        [Header("Events")]
        public UnityEvent<float> onDeviceValueChanged;

        private Material _material1;
        private Material _material2;

        [ColorUsage(true, true)]
        public Color material1StartColor;
        [ColorUsage(true, true)]
        public Color material1OnColor;
        
        [ColorUsage(true, true)]
        public Color material2StartColor;
        [ColorUsage(true, true)]
        public Color material2OnColor;
        
        private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

        private float _currentRotation = 0f;

        private void Start()
        {
            MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                if (meshRenderer.materials.Length > material1Index)
                    _material1 = meshRenderer.materials[material1Index];
                
                if (meshRenderer.materials.Length > material2Index)
                    _material2 = meshRenderer.materials[material2Index];
            }
        }
        
        private void Update()
        {
            float selectValue = selectActionValue.action.ReadValue<float>();
            float activateValue = activateActionValue.action.ReadValue<float>();

            float finalValue = selectValue - activateValue;
            
            ControlDevice(finalValue);
        }

        public void ControlDevice(float input)
        {
            // Validate the input
            if (input < -1f || input > 1f)
            {
                Debug.LogError("Input must be between 0 and 1.");
                return;
            }
            
            float rotationAmount = maxSpinSpeed * input * Time.deltaTime;

            // Spin the GameObject
            if (spinningObject != null)
            {
                spinningObject.transform.Rotate(0, 0, rotationAmount * spinSpeedMultiplier);
                UpdateRotationValue(rotationAmount);
            }

            if (input > 0 && _material1 != null)
            {
                Color targetColor = Color.Lerp(material1StartColor, material1OnColor, input);
                _material1.SetColor(EmissionColor, targetColor);    
            }
            else if (input < 0)
            {
                Color targetColor = Color.Lerp(material2StartColor, material2OnColor, -input);
                _material2.SetColor(EmissionColor, targetColor);
            }
            else
            {
                _material1.SetColor(EmissionColor, material1StartColor);
                _material2.SetColor(EmissionColor, material2StartColor);
            }

            if (tiltingObject != null)
            {
                tiltingObject.transform.localEulerAngles = new Vector3(0, 0, maxTiltDegrees * input);
            }
        }
        
        private void UpdateRotationValue(float rotationIncrement)
        {
            _currentRotation += rotationIncrement;
        
            float wrapValue = (_currentRotation % 360f) / 360f;
            if (wrapValue < 0)
                wrapValue += 1;
            
            onDeviceValueChanged.Invoke(wrapValue);
        }
    }
}