using System;
using DG.Tweening;
using UnityEngine;

namespace DefaultNamespace
{
    public class ObjectExposer : MonoBehaviour
    {
        [SerializeField] private Renderer targetMeshRenderer;
        
        public float startValue = 0f;
        public float endValue = 1f;
        public float duration = 1f;
        public Ease ease = Ease.Linear;
        
        private bool _isExposed;

        public void SetExposeProgress(float progress)
        {
            if (targetMeshRenderer == null) return;
            float newValue = Mathf.Lerp(startValue, endValue, progress);
            targetMeshRenderer.sharedMaterial.SetFloat("_Progress", newValue);
        }
    }
}