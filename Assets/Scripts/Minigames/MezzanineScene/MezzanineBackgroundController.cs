using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using DG.Tweening;

public class MezzanineBackgroundController : MonoBehaviour
{
    [SerializeField] private List<BackgroundConfiguration> backgroundConfigurations;

    [SerializeField] private MeshRenderer backgroundMeshRenderer;
    [SerializeField] private ParticleSystem debrisParticleSystem;

    private NetworkSceneSelector _sceneSelector;

    void Start()
    {
        var sceneSelector = FindAnyObjectByType<NetworkSceneSelector>();
        if (sceneSelector == null)
        {
            Debug.LogError("No NetworkSceneSelector found in the scene");
            return;
        }

        _sceneSelector = sceneSelector;
        sceneSelector.DidSelectScene.AddListener(OnDidSelectScene);
    }

    void OnDestroy()
    {
        if (_sceneSelector != null)
        {
            _sceneSelector.DidSelectScene.RemoveListener(OnDidSelectScene);
        }
    }

    private void OnDidSelectScene(string sceneId)
    {
        var backgroundConfiguration = backgroundConfigurations.Find(configuration => configuration.sceneId == sceneId);
        if (backgroundConfiguration == null)
        {
            Debug.LogError($"No background configuration found for scene {sceneId}");
            return;
        }

        // backgroundMeshRenderer.material.color = backgroundConfiguration.backgroundColor;
        backgroundMeshRenderer.material.DOColor(backgroundConfiguration.backgroundColor, "_BaseColor", .5f);

        var particlesMain = debrisParticleSystem.main;
        particlesMain.startColor = backgroundConfiguration.particlesColor;
    }

    [Serializable]
    class BackgroundConfiguration
    {
        public string sceneId;
        public Color backgroundColor;
        public Color particlesColor;
    }
}
