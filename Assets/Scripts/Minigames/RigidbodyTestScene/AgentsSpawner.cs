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
                SpawnAgentAtRandomSpawnPoint();
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            SpawnAgentAtRandomSpawnPoint();
        }
    }

    public void SpawnAgentWithPositionAndRotation(Vector3 position, Quaternion rotation)
    {
        if (NetworkManager.Singleton != null)
        {
            if (!IsServer) return;

            var newAgent = Instantiate(agentPrefab, position, rotation, transform);

            NetworkObject agentNetworkObject = newAgent.GetComponent<NetworkObject>();
            agentNetworkObject.Spawn(destroyWithScene: true);
        }
        else
        {
            Instantiate(agentPrefab, position, rotation);
        }
    }

    private void SpawnAgentAtRandomSpawnPoint()
    {
        var spawnTransform = transform;
        if (spawnPoints.Length > 0)
        {
            var spawnTransformIndex = Random.Range(0, spawnPoints.Length);
            spawnTransform = spawnPoints[spawnTransformIndex];
        }

        SpawnAgentWithPositionAndRotation(spawnTransform.position, spawnTransform.rotation);
    }
}