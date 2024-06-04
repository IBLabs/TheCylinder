using System.Collections;
using System.Collections.Generic;

using UnityEngine;

[RequireComponent(typeof(AgentSpawner))]
public class AgentDuplicator : MonoBehaviour
{
    public bool duplicatorEnabled = true;

    [SerializeField] private int copiesToSpawn = 1;

    private AgentSpawner _spawner;

    void Start()
    {
        _spawner = GetComponent<AgentSpawner>();
    }

    public void OnEnemyHit(GameObject hitObject)
    {
        if (!duplicatorEnabled) return;

        for (int i = 0; i < copiesToSpawn; i++)
        {
            _spawner.SpawnAgentWithPositionAndRotation(hitObject.transform.position, hitObject.transform.rotation);
        }
    }
}
