using System.Collections;
using System.Collections.Generic;

using Unity.Netcode;
using Unity.VisualScripting;

using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
public class NetworkPrisonLockController : NetworkBehaviour
{
    [SerializeField] private Animator animator;

    [SerializeField] private float unlockDuration = .8f;

    [SerializeField] private ParticleSystem unlockParticles;

    private Coroutine _unlockCoroutine;
    private PrisonNetworkGameManager _gameManager;

    private NetworkVariable<bool> _isUnlocked = new NetworkVariable<bool>(false);
    private NetworkVariable<bool> _isUnlocking = new NetworkVariable<bool>(false);

    void Start()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        _gameManager = GameObject.FindAnyObjectByType<PrisonNetworkGameManager>();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
    }

    public void PlayUnlockParticles()
    {
        unlockParticles.Play();
    }

    private void StartUnlocking()
    {
        var hasNetworkAccess = NetworkManager.Singleton != null;

        if (!hasNetworkAccess)
        {
            _unlockCoroutine = StartCoroutine(UnlockCoroutine());
            LocalStartUnlocking();
        }

        StartUnlockingServerRpc();
    }

    [ServerRpc]
    private void StartUnlockingServerRpc()
    {
        if (!IsServer) return;

        _isUnlocking.Value = true;

        _unlockCoroutine = StartCoroutine(UnlockCoroutine());

        StartUnlockingClientRpc();
    }

    [ClientRpc]
    private void StartUnlockingClientRpc()
    {
        LocalStartUnlocking();
    }

    private void LocalStartUnlocking()
    {
        animator.SetBool("IsUnlocking", true);
    }

    [ServerRpc]
    private void UnlockServerRpc()
    {
        UnlockClientRpc();
    }

    [ClientRpc]
    private void UnlockClientRpc()
    {
        LocalUnlock();
    }

    private void LocalUnlock()
    {
        animator.SetBool("IsUnlocked", true);
    }

    [ServerRpc]
    private void StopUnlockServerRpc()
    {
        StopUnlockingClientRpc();
    }

    [ClientRpc]
    private void StopUnlockingClientRpc()
    {
        animator.SetBool("IsUnlocking", false);
    }

    private IEnumerator UnlockCoroutine()
    {
        yield return new WaitForSeconds(unlockDuration);

        _isUnlocked.Value = true;

        var hasNetworkAccess = NetworkManager.Singleton != null;
        if (hasNetworkAccess)
        {
            if (IsServer)
            {
                UnlockServerRpc();
            }
        }
        else
        {
            LocalUnlock();
        }

        _gameManager.OnLightTurnedOn();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!_isUnlocked.Value)
            {
                StartUnlocking();
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (_isUnlocking.Value && !_isUnlocked.Value)
            {
                var hasNetworkAccess = NetworkManager.Singleton != null;
                if (hasNetworkAccess)
                {
                    if (IsServer)
                    {
                        StopCoroutine(_unlockCoroutine);
                        StopUnlockServerRpc();
                    }
                }
                else
                {
                    StopCoroutine(_unlockCoroutine);
                    StopUnlockingClientRpc();
                }
            }
        }
    }
}
