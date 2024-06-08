using Unity.VisualScripting;

using UnityEngine;
using UnityEngine.InputSystem;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public AudioClip[] audioClips;

    [SerializeField] private InputActionProperty muteAction;

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

    void OnEnable()
    {
        RegisterActions();
    }

    void OnDisable()
    {
        UnregisterActions();
    }

    private void RegisterActions()
    {
        muteAction.action.performed += OnMuteActionPerformed;
    }

    private void UnregisterActions()
    {
        muteAction.action.performed -= OnMuteActionPerformed;
    }

    private void OnMuteActionPerformed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            AudioListener.pause = !AudioListener.pause;
        }
    }

    public void PlaySound(string soundName, Vector3 position)
    {
        AudioClip clip = GetAudioClip(soundName);
        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, position);
        }
    }

    private AudioClip GetAudioClip(string soundName)
    {
        foreach (var clip in audioClips)
        {
            if (clip.name == soundName)
            {
                return clip;
            }
        }
        return null;
    }
}
