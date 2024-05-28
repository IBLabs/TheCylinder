using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject objectToSpawn;
    
    public Camera cameraToFace;
    public bool ignoreCamera;
    public float spawnHeight = 10f;

    public void SpawnObjectAtPosition(Vector3 position)
    {
        if (!ignoreCamera && cameraToFace != null)
        {
            float distance = 2;
            var forward = cameraToFace.transform.forward;
            Vector3 spawnPosition = forward * distance;
            Instantiate(objectToSpawn, spawnPosition, Quaternion.identity);
        }
        else
        {
            Vector3 modifiedPosition = new Vector3(position.x, spawnHeight, position.z);
            Instantiate(objectToSpawn, modifiedPosition, Quaternion.identity);
        }
    }
}