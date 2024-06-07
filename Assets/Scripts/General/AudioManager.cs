using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public AudioClip[] audioClips;

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
