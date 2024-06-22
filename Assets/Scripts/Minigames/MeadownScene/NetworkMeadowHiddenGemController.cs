using System.Collections;
using System.Collections.Generic;

using Unity.Netcode;

using UnityEngine;

using DG.Tweening;

public class NetworkMeadowHiddenGemController : NetworkBehaviour
{
    [Header("Configuration")]
    [SerializeField] private float exposeAnimationDuration = .6f;
    [SerializeField] private float gemRotationSpeed = 1.0f;

    private Transform _gemObjectTransform;

    void Start()
    {
        var hasNetworkAccess = NetworkManager.Singleton != null;
        if (!hasNetworkAccess)
        {
            SetInitialState();
        }
    }

    public override void OnNetworkSpawn()
    {
        SetInitialState();

        base.OnNetworkSpawn();
    }

    public void ExposeSelf()
    {
        var hasNetworkAccess = NetworkManager.Singleton != null;
        if (hasNetworkAccess)
        {
            ExposeSelfServerRpc();
        }
        else
        {
            LocalExposeSelf();
        }
    }

    #region Networking

    [ServerRpc(RequireOwnership = false)]
    private void ExposeSelfServerRpc()
    {
        ExposeSelfClientRpc();
    }

    [ClientRpc]
    private void ExposeSelfClientRpc()
    {
        LocalExposeSelf();
    }

    #endregion

    #region Private Implementation Details

    private void SetInitialState()
    {
        _gemObjectTransform = transform.GetChild(0);

        _gemObjectTransform.localPosition = new Vector3(0, -1, 0);
    }

    private void LocalExposeSelf()
    {
        _gemObjectTransform.DOLocalMoveY(.4f, exposeAnimationDuration).SetEase(Ease.OutBack);

        _gemObjectTransform.DORotate(new Vector3(0, 360f, 0), gemRotationSpeed, RotateMode.LocalAxisAdd).SetEase(Ease.Linear).SetLoops(-1, LoopType.Incremental);
    }

    #endregion
}
