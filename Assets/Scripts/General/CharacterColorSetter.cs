using System.Collections;
using System.Collections.Generic;

using Unity.Netcode;

using UnityEngine;

public class CharacterColorSetter : MonoBehaviour
{
    private const int AGENT_COLOR_MATERIAL_INDEX = 3;

    private Renderer _renderer;
    private NetworkAgentSpawner _networkAgentSpawner;

    void Start()
    {
        _renderer = GetComponentInChildren<Renderer>();
        _networkAgentSpawner = FindAnyObjectByType<NetworkAgentSpawner>();

        if (_networkAgentSpawner != null)
        {
            var materials = _renderer.materials;
            materials[AGENT_COLOR_MATERIAL_INDEX] = _networkAgentSpawner.LevelAgentMaterial;
            _renderer.materials = materials;
        }
    }
}
