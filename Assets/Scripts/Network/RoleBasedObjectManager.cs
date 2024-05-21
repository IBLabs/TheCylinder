using Unity.Netcode;
using UnityEngine;

public class RoleBasedObjectManager : NetworkBehaviour
{
    [SerializeField] private GameObject[] hostOnlyObjects;
    [SerializeField] private GameObject[] clientOnlyObjects;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            EnableObjects(hostOnlyObjects);
            DisableObjects(clientOnlyObjects);
        }
        else if (IsClient)
        {
            DisableObjects(hostOnlyObjects);
            EnableObjects(clientOnlyObjects);
        }
    }

    private void EnableObjects(GameObject[] objects)
    {
        foreach (GameObject obj in objects)
        {
            obj.SetActive(true);
        }
    }

    private void DisableObjects(GameObject[] objects)
    {
        foreach (GameObject obj in objects)
        {
            obj.SetActive(false);
        }
    }
}