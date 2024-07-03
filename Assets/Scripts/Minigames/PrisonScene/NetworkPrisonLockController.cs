using System.Collections;
using System.Collections.Generic;

using Unity.Netcode;
using Unity.VisualScripting;

using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[RequireComponent(typeof(Animator))]
public class NetworkPrisonLockController : NetworkBehaviour
{
    private const string LOCK_ANIMATOR_STATE_OFF = "Prison_LockOff";
    private const string LOCK_ANIMATOR_TRIGGER_TURN_ON = "TurnOn";

    [SerializeField] private bool networkStartOff = false;
    [SerializeField] private Animator animator;

    [SerializeField] private float unlockDuration = .8f;

    [SerializeField] private ParticleSystem unlockParticles;

    public UnityEvent OnUnlock;

    private Coroutine _unlockCoroutine;
    private PrisonNetworkGameManager _gameManager;

    private NetworkVariable<bool> _isUnlocked = new NetworkVariable<bool>(false);
    private NetworkVariable<bool> _isUnlocking = new NetworkVariable<bool>(false);
    private NetworkVariable<bool> _isEnabled = new NetworkVariable<bool>(true);

    void Start()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        _gameManager = FindAnyObjectByType<PrisonNetworkGameManager>();
    }

    public override void OnNetworkSpawn()
    {
        if (networkStartOff)
        {
            if (IsServer)
            {
                _isEnabled.Value = false;
            }

            animator.Play(LOCK_ANIMATOR_STATE_OFF);
        }

        base.OnNetworkSpawn();
    }

    public void PlayUnlockParticles()
    {
        unlockParticles.Play();
    }

    public void TurnOn()
    {
        var hasNetworkAccess = NetworkManager.Singleton != null;
        if (!hasNetworkAccess)
        {
            _isEnabled.Value = true;
            LocalTurnOn();
            return;
        }

        if (!IsServer)
        {
            Debug.LogWarning("Only the server can turn on the lock");
            return;
        }

        _isEnabled.Value = true;
        TurnOnClientRpc();
    }

    [ClientRpc]
    private void TurnOnClientRpc()
    {
        LocalTurnOn();
    }

    private void LocalTurnOn()
    {
        animator.SetTrigger(LOCK_ANIMATOR_TRIGGER_TURN_ON);
    }

    public void TurnOff()
    {
        var hasNetworkAccess = NetworkManager.Singleton != null;
        if (!hasNetworkAccess)
        {
            _isEnabled.Value = false;
            LocalTurnOff();
            return;
        }

        if (!IsServer)
        {
            Debug.LogWarning("Only the server can turn off the lock");
            return;
        }

        _isEnabled.Value = false;
        TurnOffClientRpc();
    }

    [ClientRpc]
    private void TurnOffClientRpc()
    {
        LocalTurnOff();
    }

    private void LocalTurnOff()
    {
        animator.Play(LOCK_ANIMATOR_STATE_OFF);
    }

    private void StartUnlocking()
    {
        var hasNetworkAccess = NetworkManager.Singleton != null;

        if (!hasNetworkAccess)
        {
            _unlockCoroutine = StartCoroutine(UnlockCoroutine());
            LocalStartUnlocking();
            return;
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

        if (_gameManager != null)
        {
            _gameManager.OnLightTurnedOn();
        }
        else
        {
            Debug.LogWarning("Game manager not found");
        }

        OnUnlock?.Invoke();
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
            if (!_isEnabled.Value) return;

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
