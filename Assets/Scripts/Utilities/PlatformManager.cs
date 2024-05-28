using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation;

public class PlatformManager : MonoBehaviour
{
    public static bool IsVR { get; private set; }

    public bool forceNonVR = false;
    public bool forceVR = false;

    public GameObject[] VRObjects;
    public GameObject[] NonVRObjects;

    public UnityEvent<bool> OnPlatformDetected;

    void Start()
    {
        DetectPlatform();
        InitializePlatform();
    }

    private void DetectPlatform()
    {
        if (forceNonVR)
        {
            IsVR = false;
        }
        else if (forceVR)
        {
            IsVR = true;
        }
        else
        {
            IsVR = UnityEngine.XR.XRSettings.isDeviceActive;

            if (!IsVR)
            {
                IsVR = IsVRDeviceSimulatorActive();
            }
        }

        OnPlatformDetected?.Invoke(IsVR);
    }

    private bool IsVRDeviceSimulatorActive()
    {
        var simulator = FindFirstObjectByType<XRDeviceSimulator>();
        return simulator != null && simulator.isActiveAndEnabled;
    }

    private void InitializePlatform()
    {
        if (IsVR)
        {
            foreach (var obj in VRObjects)
            {
                obj.SetActive(true);
            }

            foreach (var obj in NonVRObjects)
            {
                obj.SetActive(false);
            }
        }
        else
        {
            foreach (var obj in VRObjects)
            {
                obj.SetActive(false);
            }

            foreach (var obj in NonVRObjects)
            {
                obj.SetActive(true);
            }
        }
    }
}
