using System.Collections;
using System.Collections.Generic;

using Unity.Netcode;

using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class NetworkGunSoundController : NetworkBehaviour
{
    [SerializeField] private XRPlayerShooter _shooter;

    [SerializeField] private AudioClip shootSound;
    [SerializeField] private AudioClip ricochetSound;

    private AudioSource _audioSource;

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();

        ListenForPlayerShooterEvents();
    }

    public void PlayShootSound()
    {
        var hasNetworkAccess = NetworkManager.Singleton != null;
        if (hasNetworkAccess)
        {
            if (!IsServer)
            {
                Debug.LogWarning("Only the server should play the shoot sound");
                return;
            }

            PlayShootSoundServerRpc();
        }
        else
        {
            LocalPlayShootSound();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void PlayShootSoundServerRpc()
    {
        PlayShootSoundClientRpc();
    }

    [ClientRpc]
    private void PlayShootSoundClientRpc()
    {
        LocalPlayShootSound();
    }

    private void LocalPlayShootSound()
    {
        if (shootSound != null)
        {
            _audioSource.pitch = Random.Range(0.9f, 1.1f);
            _audioSource.PlayOneShot(shootSound);

            if (Random.value < 0.1f && ricochetSound != null)
            {
                _audioSource.pitch = Random.Range(0.9f, 1.1f);
                _audioSource.PlayOneShot(ricochetSound);
            }
        }
        else
        {
            Debug.LogWarning("Shoot sound is not set");
        }
    }

    private void ListenForPlayerShooterEvents()
    {
        if (_shooter == null)
        {
            _shooter = GetComponent<XRPlayerShooter>();

            if (_shooter == null)
            {
                Debug.LogWarning("Player shooter is not set");
                return;
            }
        }

        _shooter.DidShoot.AddListener(OnDidShoot);
    }

    private void OnDidShoot(Vector3 position, Vector3 direction)
    {
        PlayShootSound();
    }
}
