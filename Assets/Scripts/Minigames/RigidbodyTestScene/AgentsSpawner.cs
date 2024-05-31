using Unity.Netcode;

using UnityEngine;

class AgentSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject agentPrefab;

    [SerializeField] private bool autoLoad = false;
    [SerializeField] private int autoLoadAmount = 10;

    void Start()
    {
        if (IsServer && autoLoad)
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
        if (NetworkManager.Singleton != null)
        {
            if (!IsServer) return;

            var newAgent = Instantiate(agentPrefab, transform.position, Quaternion.identity);

            NetworkObject agentNetworkObject = newAgent.GetComponent<NetworkObject>();
            agentNetworkObject.Spawn();
        }
        else
        {
            Instantiate(agentPrefab, transform.position, Quaternion.identity);
        }
    }
}