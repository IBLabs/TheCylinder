using System.Collections;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;
using DG.Tweening;

public class MeadowBankController : NetworkBehaviour
{
    [Header("Configuration")]
    [SerializeField] private Transform[] relocateTransforms;

    [SerializeField] private InputActionProperty relocateAction;

    private Vector3 _currentPosition;
    private bool _isRelocating;

    void Update()
    {
        if (relocateAction.action.triggered)
        {
            PerformRandomRelocate();
        }
    }

    public Transform PerformRandomRelocate()
    {
        bool hasNetworkAccess = NetworkManager.Singleton != null;

        if (hasNetworkAccess && !IsServer) return null;

        if (_isRelocating) return null;

        Transform targetTransform = GetRandomDistinctTransform();

        StartCoroutine(RelocateCoroutine(targetTransform.position));

        return targetTransform;
    }

    public void PerformRelocate(Vector3 position)
    {
        StartCoroutine(RelocateCoroutine(position));
    }

    private Transform GetRandomDistinctTransform()
    {
        Transform newTransform = relocateTransforms[Random.Range(0, relocateTransforms.Length)];
        while (newTransform.position == _currentPosition)
        {
            newTransform = relocateTransforms[Random.Range(0, relocateTransforms.Length)];
        }

        return newTransform;
    }

    private IEnumerator RelocateCoroutine(Vector3 position)
    {
        _isRelocating = true;

        var originalScale = transform.localScale;

        yield return transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack).WaitForCompletion();

        transform.position = position;

        yield return transform.DOScale(originalScale, 0.3f).SetEase(Ease.OutBack).WaitForCompletion();

        _currentPosition = position;

        _isRelocating = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.GetComponentsInChildren<NetworkPickupable>().Length > 0)
            {
                ulong droppedBy = GetPlayerOwnerClientId(other);
                NetworkMeadowGameManager.Instance.OnPlayerDroppedPickupables(droppedBy);
            }
        }
    }

    private ulong GetPlayerOwnerClientId(Collider other)
    {
        ulong droppedBy = 0;

        if (NetworkManager.Singleton != null)
        {
            NetworkObject playerNetworkObject = other.GetComponent<NetworkObject>();
            if (playerNetworkObject.IsOwner)
            {
                droppedBy = playerNetworkObject.OwnerClientId;
            }
        }

        return droppedBy;
    }
}
