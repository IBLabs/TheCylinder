using System.Collections;
using System.Collections.Generic;

using Unity.Netcode;

using UnityEngine;
using UnityEngine.InputSystem;

public class CubeShooterController : NetworkBehaviour
{
    [SerializeField] private GameObject cubePrefab;
    [SerializeField] private float shootForce = 500f;

    [SerializeField] private InputActionProperty shootAction;

    private void Update()
    {
        if (shootAction.action.triggered)
        {
            bool hasNetworkAccess = NetworkManager.Singleton != null;
            if (hasNetworkAccess)
            {
                ShootCubeServerRpc();
            }
            else
            {
                // TODO: handle offline scenario
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void ShootCubeServerRpc()
    {
        var newCube = Instantiate(cubePrefab, transform.position, Quaternion.identity);
        newCube.GetComponent<NetworkObject>().Spawn(true);

        newCube.GetComponent<Rigidbody>().AddForce(transform.forward * shootForce);
    }
}
