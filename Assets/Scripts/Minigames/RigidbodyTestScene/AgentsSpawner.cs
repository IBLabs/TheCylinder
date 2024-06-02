using Unity.Netcode;

using UnityEngine;

class AgentSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject agentPrefab;
    [SerializeField] private bool autoLoad = false;
    [SerializeField] private int autoLoadAmount = 10;
    [SerializeField] private Transform[] spawnPoints;

    void Start()
    {
        var shouldSpawn = IsServer || NetworkManager.Singleton == null;
        if (shouldSpawn && autoLoad)
        {
            for (int i = 0; i < autoLoadAmount; i++)
            {
                SpawnAgent();
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            SpawnAgent();
        }
    }

    private void SpawnAgent()
    {
        var spawnTransform = transform;
        if (spawnPoints.Length > 0)
        {
            var spawnTransformIndex = Random.Range(0, spawnPoints.Length);
            spawnTransform = spawnPoints[spawnTransformIndex];
        }

        if (NetworkManager.Singleton != null)
        {
            if (!IsServer) return;

            var newAgent = Instantiate(agentPrefab, spawnTransform.position, spawnTransform.rotation);

            NetworkObject agentNetworkObject = newAgent.GetComponent<NetworkObject>();
            agentNetworkObject.Spawn();
        }
        else
        {
            Instantiate(agentPrefab, spawnTransform.position, spawnTransform.rotation);
        }
    }
}