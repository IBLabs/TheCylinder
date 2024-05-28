using Unity.Netcode;

using UnityEngine;

public class RotationSync : NetworkBehaviour
{
    public NetworkVariable<Quaternion> networkRot = new NetworkVariable<Quaternion>();

    [SerializeField] private Transform targetTransform;

    private bool _shouldUpdate;

    private void OnEnable()
    {
        _shouldUpdate = true;
    }

    private void OnDisable()
    {
        _shouldUpdate = false;
    }

    public void UpdateRotation(Quaternion newRot)
    {
        if (IsHost && _shouldUpdate)
        {
            networkRot.Value = newRot;
        }
    }

    private void Update()
    {
        if (!IsHost)
        {
            Vector3 euler = networkRot.Value.eulerAngles;
            euler.y = -euler.y;
            targetTransform.rotation = Quaternion.Euler(euler);
        }
    }
}