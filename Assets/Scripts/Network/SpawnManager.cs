using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject objectToSpawn;
    
    public Camera cameraToFace;
    public bool ignoreCamera;

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
            Instantiate(objectToSpawn, position, Quaternion.identity);
        }
    }
}