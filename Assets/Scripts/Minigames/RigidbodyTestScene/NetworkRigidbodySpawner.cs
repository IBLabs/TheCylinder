using UnityEditor;
using Unity.Netcode;
using UnityEngine;

public class NetworkRigidbodySpawner : MonoBehaviour
{
    [SerializeField] private GameObject targetPrefab;

    public void SpawnTargetObject()
    {
        var instance = Instantiate(targetPrefab, transform.position, transform.rotation);

        var instanceNetworkObject = instance.GetComponent<NetworkObject>();
        instanceNetworkObject.Spawn(destroyWithScene: true);
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(NetworkRigidbodySpawner))]
    public class NetworkRigidbodySpawnerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            NetworkRigidbodySpawner spawner = (NetworkRigidbodySpawner)target;
            if (GUILayout.Button("Spawn Target Object"))
            {
                spawner.SpawnTargetObject();
            }
        }
    }
#endif
}
