using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;

using Unity.VisualScripting;
using Unity.XR.CoreUtils;

using UnityEngine;

public class MeadowXRFollower : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private Transform uiPanelTransform;

    [SerializeField] private Transform leftControllerTransform;

    [Header("Configuration")]
    [SerializeField] private float radius;
    [SerializeField] private float followSpeed = 2f;


    private Camera _playerOriginCamera;

    void Start()
    {
        var xrOrigin = FindAnyObjectByType<XROrigin>();
        _playerOriginCamera = xrOrigin.GetComponentInChildren<Camera>();

    }

    void Update()
    {
        HandleFollowPosition();
        HandleFollowRotation();
    }

    private void HandleFollowPosition()
    {
        var normalizedPlayerForward = _playerOriginCamera.transform.forward.normalized;
        var playerPosition = _playerOriginCamera.transform.position;

        var targetPosition = playerPosition + (normalizedPlayerForward * radius);
        var lerpedPosition = Vector3.Lerp(uiPanelTransform.position, targetPosition, followSpeed * Time.deltaTime);

        uiPanelTransform.position = new Vector3(lerpedPosition.x, uiPanelTransform.transform.position.y, lerpedPosition.z);
    }

    private void HandleFollowRotation()
    {
        var playerPosition = _playerOriginCamera.transform.position;
        var lookDirection = uiPanelTransform.position - playerPosition;

        var lookRotation = Quaternion.LookRotation(lookDirection);

        if (!HandleAdvancedRotation(lookRotation, leftControllerTransform))
        {
            ApplySimpleRotation(lookRotation);
        }
    }

    private void ApplySimpleRotation(Quaternion simpleRotation)
    {
        uiPanelTransform.rotation = Quaternion.Slerp(uiPanelTransform.rotation, simpleRotation, followSpeed * Time.deltaTime);
    }

    private bool HandleAdvancedRotation(Quaternion lookRotation, Transform targetControllerTransform)
    {
        if (targetControllerTransform == null)
        {
            return false;
        }

        Ray leftRay = new Ray(targetControllerTransform.position, targetControllerTransform.forward);
        RaycastHit hit;
        if (Physics.Raycast(leftRay, out hit, 100f, LayerMask.GetMask("Pinned UI")))
        {
            var localHitPoint = uiPanelTransform.transform.InverseTransformPoint(new Vector3(-hit.point.x, -hit.point.y, hit.point.z));
            var hoverRotation = Quaternion.LookRotation(localHitPoint);

            Quaternion partialToLookRotation = Quaternion.Lerp(lookRotation, hoverRotation, .09f);

            Quaternion targetRotation = Quaternion.Slerp(uiPanelTransform.rotation, partialToLookRotation, followSpeed * Time.deltaTime);

            uiPanelTransform.rotation = targetRotation;

            return true;
        }

        return false;
    }
}
