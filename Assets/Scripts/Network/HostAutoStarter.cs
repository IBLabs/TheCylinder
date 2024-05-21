using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class HostAutoStarter : MonoBehaviour
{
    public void OnBootstrapped()
    {
        NetworkManager.Singleton.StartHost();
    }
}
