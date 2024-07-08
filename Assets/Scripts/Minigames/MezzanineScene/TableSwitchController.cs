using UnityEngine;

using DG.Tweening;
using UnityEngine.Events;
using Unity.Netcode;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class TableSwitchController : NetworkBehaviour
{
    public bool IsActivated { get; private set; }


    [Header("Configuration")]


    [Header("Events")]
    public UnityEvent<TableSwitchController> OnSwitchActivated;


    private bool _isSwitchEnabled = true;

    void Start()
    {
        SetInitialState();
    }

    public void SetInitialState()
    {
        IsActivated = false;
    }

    public void SetSwitchEnabled(bool isEnabled)
    {
        _isSwitchEnabled = isEnabled;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_isSwitchEnabled) return;

        if (other.CompareTag("Player"))
        {
            var hasNetworkAccess = NetworkManager.Singleton != null;
            if (!hasNetworkAccess)
            {
                SetSwitchPressed();
                return;
            }

            SetSwitchPressedServerRpc();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!_isSwitchEnabled) return;

        if (other.CompareTag("Player"))
        {
            var hasNetworkAccess = NetworkManager.Singleton != null;
            if (!hasNetworkAccess)
            {
                SetSwitchUnpressed();
                return;
            }

            SetSwitchUnpressedServerRpc();
        }
    }

    private void SetSwitchPressed()
    {
        IsActivated = true;

        OnSwitchActivated?.Invoke(this);
    }

    private void SetSwitchUnpressed()
    {
        IsActivated = false;
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetSwitchPressedServerRpc()
    {
        SetSwitchPressedClientRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetSwitchUnpressedServerRpc()
    {
        SetSwitchUnpressedClientRpc();
    }

    [ClientRpc]
    private void SetSwitchPressedClientRpc()
    {
        SetSwitchPressed();
    }

    [ClientRpc]
    private void SetSwitchUnpressedClientRpc()
    {
        SetSwitchUnpressed();
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(TableSwitchController))]
public class TableSwitchControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        TableSwitchController tableSwitchController = (TableSwitchController)target;

        if (GUILayout.Button("Trigger"))
        {
            tableSwitchController.OnSwitchActivated?.Invoke(tableSwitchController);
        }
    }
}
#endif