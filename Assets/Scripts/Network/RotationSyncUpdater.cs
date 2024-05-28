using UnityEngine;

public class RotationSyncUpdater : MonoBehaviour
{
    public RotationSync rotationSync;

    public Camera targetCamera;

    private void Update()
    {
        if (rotationSync.IsHost)
        {
            rotationSync.UpdateRotation(targetCamera.transform.rotation);
        }
    }
}