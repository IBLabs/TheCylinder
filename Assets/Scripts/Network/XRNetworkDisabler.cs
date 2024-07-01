using Unity.XR.CoreUtils;

using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.Rendering.Universal;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

[RequireComponent(typeof(XROrigin)), RequireComponent(typeof(InputActionManager))]
public class XRNetworkDisabler : MonoBehaviour
{
    [SerializeField] private GameObject mainCameraGameObject;
    [SerializeField] private GameObject leftHandGameObject;
    [SerializeField] private GameObject rightHandGameObject;
    [SerializeField] private GameObject transitionSphere;

    [SerializeField] private Component[] toDisable;

    public void OnPlatformDetected(bool isVR)
    {
        if (!isVR)
        {
            if (toDisable != null && toDisable.Length > 0)
            {
                foreach (var component in toDisable)
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
            else
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

                // leftHandGameObject.GetComponent<LineRenderer>().enabled = false;

                rightHandGameObject.GetComponent<ActionBasedController>().enabled = false;
                rightHandGameObject.GetComponent<XRRayInteractor>().enabled = false;
                rightHandGameObject.GetComponent<XRInteractorLineVisual>().enabled = false;

                // rightHandGameObject.GetComponent<LineRenderer>().enabled = false;

                transitionSphere.SetActive(false);
            }
        }
    }
}