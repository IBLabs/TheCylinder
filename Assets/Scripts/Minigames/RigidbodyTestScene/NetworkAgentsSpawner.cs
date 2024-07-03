using Unity.Netcode;

using UnityEngine;
using UnityEngine.Rendering;

class NetworkAgentSpawner : NetworkBehaviour
{
    public GameObject[] SpawnedAgents => _spawnedAgents;

    [SerializeField] private GameObject agentPrefab;
    [SerializeField] private bool autoLoad = false;
    [SerializeField] private int autoLoadAmount = 10;
    [SerializeField] private Transform[] spawnPoints;

    private GameObject[] _spawnedAgents;

    void Start()
    {
        var shouldSpawn = IsServer || NetworkManager.Singleton == null;
        if (shouldSpawn && autoLoad)
        {
            _spawnedAgents = new GameObject[autoLoadAmount];
            for (int i = 0; i < autoLoadAmount; i++)
            {
                var newAgent = SpawnAgentAtRandomSpawnPoint();
                if (newAgent != null) { _spawnedAgents[i] = newAgent; };
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

    public GameObject SpawnAgentWithPositionAndRotation(Vector3 position, Quaternion rotation)
    {
        if (NetworkManager.Singleton != null)
        {
            if (!IsServer) return null;

            var newAgent = Instantiate(agentPrefab, position, rotation, transform);

            NetworkObject agentNetworkObject = newAgent.GetComponent<NetworkObject>();
            agentNetworkObject.Spawn(destroyWithScene: true);

            return newAgent;
        }
        else
        {
            return Instantiate(agentPrefab, position, rotation);
        }
    }

    public GameObject SpawnAgentAtRandomSpawnPoint()
    {
        var spawnTransform = transform;
        if (spawnPoints.Length > 0)
        {
            var spawnTransformIndex = Random.Range(0, spawnPoints.Length);
            spawnTransform = spawnPoints[spawnTransformIndex];
        }

        return SpawnAgentWithPositionAndRotation(spawnTransform.position, spawnTransform.rotation);
    }
}