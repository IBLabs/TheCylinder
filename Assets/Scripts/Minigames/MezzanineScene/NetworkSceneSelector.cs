using System.Linq;

using Unity.Collections;
using Unity.Netcode;

using UnityEngine;

public class NetworkSceneSelector : NetworkBehaviour
{
    private const string DEFAULT_SCENE_ID = "meadow";

    [SerializeField] private SceneDescriptor[] sceneDescriptors;

    [SerializeField] private DekstopLevelCanvasController desktopLevelCanvasController;
    [SerializeField] private XRLevelCanvasController xrLevelCanvasController;

    private readonly NetworkVariable<FixedString32Bytes> _selectedSceneId = new NetworkVariable<FixedString32Bytes>();

    private SceneLoader _sceneLoader;

    void Start()
    {
        _sceneLoader = FindAnyObjectByType<SceneLoader>();

        // SetInitialState();
    }

    public override void OnNetworkSpawn()
    {
        SetInitialState();

        base.OnNetworkSpawn();
    }

    public void SetMeadowScene()
    {
        var hasNetworkAccess = NetworkManager.Singleton != null;
        if (!hasNetworkAccess)
        {
            SetActiveScene("meadow");
            return;
        }

        SetSceneWithIdServerRpc("meadow");
    }

    public void SetPrisonScene()
    {
        var hasNetworkAccess = NetworkManager.Singleton != null;
        if (!hasNetworkAccess)
        {
            SetActiveScene("prison");
            return;
        }

        SetSceneWithIdServerRpc("prison");
    }
    public void SetFollowScene()
    {
        var hasNetworkAccess = NetworkManager.Singleton != null;
        if (!hasNetworkAccess)
        {
            SetActiveScene("follow");
            return;
        }

        SetSceneWithIdServerRpc("follow");
    }

    public void LaunchSelectdScene()
    {
        var sceneDescriptor = sceneDescriptors.FirstOrDefault(descriptor => descriptor.sceneId == _selectedSceneId.Value);
        if (!sceneDescriptor) return;
        _sceneLoader.LoadSpecificScene(sceneDescriptor.sceneFileName);
    }

    private void SetInitialState()
    {
        var hasNetworkAccess = NetworkManager.Singleton != null;

        if (hasNetworkAccess)
        {
            if (!IsServer) return;

            SetSceneWithIdServerRpc(DEFAULT_SCENE_ID);
        }
        else
        {
            SetActiveScene(DEFAULT_SCENE_ID);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetSceneWithIdServerRpc(string sceneId)
    {
        if (sceneId == _selectedSceneId.Value) return;

        _selectedSceneId.Value = sceneId;

        SetActiveSceneClientRpc(sceneId);
    }

    [ClientRpc]
    private void SetActiveSceneClientRpc(string sceneId)
    {
        SetActiveScene(sceneId);
    }

    private void SetActiveScene(string sceneId)
    {
        var targetDescriptor = sceneDescriptors.FirstOrDefault(descriptor => descriptor.sceneId == sceneId);

        if (!targetDescriptor) return;

        if (desktopLevelCanvasController.gameObject.activeInHierarchy)
            desktopLevelCanvasController.SetActiveScene(targetDescriptor);

        if (xrLevelCanvasController.gameObject.activeInHierarchy)
            xrLevelCanvasController.SetActiveScene(targetDescriptor);
    }
}
