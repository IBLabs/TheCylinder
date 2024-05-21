using UnityEngine;

public class SpawnInputManager : MonoBehaviour
{
    public Camera mainCamera;
    public NetworkEventManager networkEventManager;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 tapPosition = Input.mousePosition;
            ProcessTap(tapPosition);
        }
    }

    void ProcessTap(Vector3 screenPosition)
    {
        Ray ray = mainCamera.ScreenPointToRay(screenPosition);
        RaycastHit hit;
        
        Debug.DrawRay(ray.origin, ray.direction * 100, Color.red, 5f);
        
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject.CompareTag("SpawnPlane"))
            {
                Debug.Log("[TEST]: hit point: " + hit.point);

                Vector3 spawnPoint = new Vector3(hit.point.x, 0, hit.point.y);
                networkEventManager.RequestSpawnObject(spawnPoint);
            }
        }
    }
}