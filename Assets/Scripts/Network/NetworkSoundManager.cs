using UnityEngine;
using Unity.Netcode;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class NetworkSoundManager : NetworkBehaviour
{
    public static NetworkSoundManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void PlaySoundServerRpc(string soundName, Vector3 position)
    {
        PlaySoundClientRpc(soundName, position);
    }

    [ClientRpc]
    private void PlaySoundClientRpc(string soundName, Vector3 position)
    {
        AudioManager.Instance.PlaySound(soundName, position);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(NetworkSoundManager))]
public class NetworkSoundManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Play Sound"))
        {
            ((NetworkSoundManager)target).PlaySoundServerRpc("BuzzerSound1", Vector3.zero);
        }
    }
}
#endif