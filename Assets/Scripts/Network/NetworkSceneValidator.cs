using UnityEngine;

using Unity.Netcode;

using UnityEngine.SceneManagement;

public class NetworkSceneValidator : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        NetworkManager.SceneManager.VerifySceneBeforeLoading = ClientVerifyScene;
        base.OnNetworkSpawn();
    }

    private bool ClientVerifyScene(int sceneIndex, string sceneName, LoadSceneMode loadSceneMode)
    {
        if (sceneName == "NetworkScene")
        {
            return false;
        }

        return true;
    }
}