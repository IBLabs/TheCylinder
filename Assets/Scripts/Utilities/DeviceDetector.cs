using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class DeviceDetector
{
    public static bool IsVRDevice()
    {
        return XRSettings.isDeviceActive;
    }
}
