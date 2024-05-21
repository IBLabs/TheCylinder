using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets
{
    public class TimeHandleController : MonoBehaviour
    {
        public MeshRenderer targetMeshRenderer;

        [ColorUsage(true, true)]
        public Color glowColor;

        private Color _startColor;

        private readonly int _emissionPropertyName = Shader.PropertyToID("_EmissionColor");

        private void Start()
        {
            _startColor = targetMeshRenderer.material.GetColor(_emissionPropertyName);
        }

        public void OnSelectEnter(SelectEnterEventArgs args)
        {
            targetMeshRenderer.material.DOColor(glowColor, _emissionPropertyName, 0.5f);
        }

        public void OnSelectExit(SelectExitEventArgs args)
        {
            targetMeshRenderer.material.DOColor(_startColor, _emissionPropertyName, 0.5f);
        }
    }
}