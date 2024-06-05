#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
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
            InitializeVR();
        }
        else
        {
            InitializeNonVR();
        }
    }

    public void InitializeVR()
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

    public void InitializeNonVR()
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

#if UNITY_EDITOR
[CustomEditor(typeof(PlatformManager))]
public class PlatformManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        PlatformManager platformManager = (PlatformManager)target;

        if (GUILayout.Button("Initialize VR"))
        {
            platformManager.InitializeVR();
        }

        if (GUILayout.Button("Initialize Non-VR"))
        {
            platformManager.InitializeNonVR();
        }
    }
}
#endif