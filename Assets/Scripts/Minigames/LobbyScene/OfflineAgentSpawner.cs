using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class OfflineAgentSpawner : MonoBehaviour
{
    [SerializeField] private GameObject offlineAgentPrefab;

    [SerializeField] private Transform[] spawnPoints;

    public void SpawnAgentAtRandomPosition()
    {
        int randomIndex = Random.Range(0, spawnPoints.Length);

        GameObject agent = Instantiate(offlineAgentPrefab, spawnPoints[randomIndex].position, Quaternion.identity);

        agent.transform.SetParent(spawnPoints[randomIndex]);
    }
}
