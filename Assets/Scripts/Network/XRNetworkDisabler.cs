using Unity.XR.CoreUtils;

using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.Rendering.Universal;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

public class XRNetworkDisabler : MonoBehaviour
{
    [SerializeField] private GameObject mainCameraGameObject;
    [SerializeField] private GameObject leftHandGameObject;
    [SerializeField] private GameObject rightHandGameObject;
    [SerializeField] private GameObject transitionSphere;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnPlatformDetected(bool isVR)
    {
        if (!isVR)
        {
            GetComponent<XROrigin>().enabled = false;
            GetComponent<InputActionManager>().enabled = false;

            mainCameraGameObject.GetComponent<Camera>().enabled = false;
            mainCameraGameObject.GetComponent<AudioListener>().enabled = false;
            mainCameraGameObject.GetComponent<TrackedPoseDriver>().enabled = false;
            mainCameraGameObject.GetComponent<UniversalAdditionalCameraData>().enabled = false;

            leftHandGameObject.GetComponent<ActionBasedController>().enabled = false;
            leftHandGameObject.GetComponent<XRRayInteractor>().enabled = false;
            leftHandGameObject.GetComponent<XRInteractorLineVisual>().enabled = false;
            leftHandGameObject.GetComponent<LineRenderer>().enabled = false;

            rightHandGameObject.GetComponent<ActionBasedController>().enabled = false;
            rightHandGameObject.GetComponent<XRRayInteractor>().enabled = false;
            rightHandGameObject.GetComponent<XRInteractorLineVisual>().enabled = false;
            rightHandGameObject.GetComponent<LineRenderer>().enabled = false;

            transitionSphere.SetActive(false);
        }
    }
}