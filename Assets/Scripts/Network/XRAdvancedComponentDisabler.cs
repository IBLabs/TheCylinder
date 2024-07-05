using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class XRAdvancedComponentDisabler : MonoBehaviour
{
    private PlatformManager _platformManager;

    void Start()
    {
        _platformManager = FindAnyObjectByType<PlatformManager>();
        if (_platformManager != null)
        {
            _platformManager.OnPlatformDetected.AddListener(OnPlatformDetected);

            if (_platformManager.DidDetect)
            {
                OnPlatformDetected(PlatformManager.IsVR);
            }
        }
        else
        {
            Debug.LogError("PlatformManager not found in scene");
        }
    }

    [SerializeField] private Component[] componentsToDisable;

    public void DisableComponents()
    {
        foreach (Component component in componentsToDisable)
        {
            if (component is Behaviour behaviour)
            {
                behaviour.enabled = false;
            }
            else if (component is Renderer renderer)
            {
                renderer.enabled = false;
            }
        }
    }

    #region Events

    private void OnPlatformDetected(bool isVR)
    {
        if (!isVR)
        {
            DisableComponents();
        }
    }

    #endregion
}
