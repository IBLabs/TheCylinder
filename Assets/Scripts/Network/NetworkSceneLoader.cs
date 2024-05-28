using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class NetworkSceneLoader : MonoBehaviour
{
    [SerializeField] private string networkSceneName = "NetworkScene";

    private void Start()
    {
        if (NetworkManager.Singleton == null)
        {
            SceneManager.LoadScene(networkSceneName, LoadSceneMode.Additive);
        }
    }
}