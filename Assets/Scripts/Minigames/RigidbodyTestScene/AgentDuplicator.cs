using System.Collections;
using System.Collections.Generic;

using UnityEngine;

[RequireComponent(typeof(NetworkAgentSpawner))]
public class AgentDuplicator : MonoBehaviour
{
    public bool duplicatorEnabled = true;

    [SerializeField] private int copiesToSpawn = 1;

    private NetworkAgentSpawner _spawner;

    void Start()
    {
        _spawner = GetComponent<NetworkAgentSpawner>();
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
