using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class OfflineAgentSpawner : MonoBehaviour
{
    private const int AGENT_COLOR_MATERIAL_INDEX = 0;
    [SerializeField] private GameObject offlineAgentPrefab;

    [SerializeField] private Transform[] spawnPoints;

    [SerializeField] private Material agentColorMaterial;

    public void SpawnAgentAtRandomPosition()
    {
        int randomIndex = Random.Range(0, spawnPoints.Length);

        GameObject agent = Instantiate(offlineAgentPrefab, spawnPoints[randomIndex].position, Quaternion.identity);

        Renderer agentRenderer = agent.GetComponentInChildren<Renderer>();
        if (agentRenderer != null)
        {
            var materials = agentRenderer.materials;
            materials[3] = agentColorMaterial;
            agentRenderer.materials = materials;
        }
        else
        {
            Debug.LogError("Agent Renderer is null");
        }


        agent.transform.SetParent(transform);
    }
}
