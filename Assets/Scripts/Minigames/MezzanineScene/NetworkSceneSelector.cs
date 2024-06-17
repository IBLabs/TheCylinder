using System.Linq;

using Unity.Mathematics;
using Unity.Netcode;

using UnityEngine;

public class NetworkSceneSelector : NetworkBehaviour
{
    [SerializeField] private SceneDescriptor[] sceneDescriptors;

    [SerializeField] private DekstopLevelCanvasController desktopLevelCanvasController;
    [SerializeField] private XRLevelCanvasController xrLevelCanvasController;

    private string _selectedSceneId = "prison";

    private SceneLoader _sceneLoader;

    void Start()
    {
        _sceneLoader = FindAnyObjectByType<SceneLoader>();

        SetInitialState();
    }

    public void SetMeadowScene()
    {
        var hasNetworkAccess = NetworkManager.Singleton != null;
        if (!hasNetworkAccess)
        {
            SetActiveSceneClientRpc("meadow");
            return;
        }

        SetSceneWithIdServerRpc("meadow");
    }

    public void SetPrisonScene()
    {
        var hasNetworkAccess = NetworkManager.Singleton != null;
        if (!hasNetworkAccess)
        {
            SetActiveSceneClientRpc("prison");
            return;
        }

        SetSceneWithIdServerRpc("prison");
    }

    public void LaunchSelectdScene()
    {
        var sceneDescriptor = sceneDescriptors.FirstOrDefault(descriptor => descriptor.sceneId == _selectedSceneId);
        if (!sceneDescriptor) return;
        _sceneLoader.LoadSpecificScene(sceneDescriptor.sceneFileName);
    }

    private void SetInitialState()
    {
        var hasNetworkAccess = NetworkManager.Singleton != null;

        if (hasNetworkAccess)
        {
            if (!IsServer)
            {
                SetSceneWithIdServerRpc(_selectedSceneId);
            }
        }
        else
        {
            SetActiveSceneClientRpc(_selectedSceneId);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetSceneWithIdServerRpc(string sceneId)
    {
        SetActiveSceneClientRpc(sceneId);
    }

    [ClientRpc]
    private void SetActiveSceneClientRpc(string sceneId)
    {
        if (_selectedSceneId == sceneId) return;

        var targetDescriptor = sceneDescriptors.FirstOrDefault(descriptor => descriptor.sceneId == sceneId);

        if (!targetDescriptor) return;

        _selectedSceneId = sceneId;

        if (desktopLevelCanvasController.gameObject.activeInHierarchy) desktopLevelCanvasController.SetActiveScene(targetDescriptor);
        if (xrLevelCanvasController.gameObject.activeInHierarchy) xrLevelCanvasController.SetActiveScene(targetDescriptor);
    }
}
