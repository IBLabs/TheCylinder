using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

public class GorillaMovementController : MonoBehaviour
{
    public ActionBasedController leftController;
    public ActionBasedController rightController;
    public Transform playerRig; // The player's rig transform

    private Vector3 initialLeftHandPosition, initialRightHandPosition;
    private Vector3 leftHandDisplacement, rightHandDisplacement;
    private bool isLeftGrabbing, isRightGrabbing;

    void OnEnable()
    {
        leftController.selectAction.action.started += ctx => GrabStarted(ctx, leftController);
        leftController.selectAction.action.canceled += ctx => GrabEnded(ctx, leftController);

        rightController.selectAction.action.started += ctx => GrabStarted(ctx, rightController);
        rightController.selectAction.action.canceled += ctx => GrabEnded(ctx, rightController);
    }

    void OnDisable()
    {
        leftController.selectAction.action.started -= ctx => GrabStarted(ctx, leftController);
        leftController.selectAction.action.canceled -= ctx => GrabEnded(ctx, leftController);

        rightController.selectAction.action.started -= ctx => GrabStarted(ctx, rightController);
        rightController.selectAction.action.canceled -= ctx => GrabEnded(ctx, rightController);
    }

    private void GrabStarted(InputAction.CallbackContext context, ActionBasedController controller)
    {
        if (controller == leftController)
        {
            isLeftGrabbing = true;
            initialLeftHandPosition = controller.transform.position;
            
            Debug.Log("[TEST]: Left hand grabbed!, initial position: " + initialLeftHandPosition);
        }
        else if (controller == rightController)
        {
            isRightGrabbing = true;
            initialRightHandPosition = controller.transform.position;
            
            Debug.Log("[TEST]: Right hand grabbed!, initial position: " + initialRightHandPosition);
        }
    }

    private void GrabEnded(InputAction.CallbackContext context, ActionBasedController controller)
    {
        if (controller == leftController)
        {
            isLeftGrabbing = false;
            leftHandDisplacement = Vector3.zero;
        }
        else if (controller == rightController)
        {
            isRightGrabbing = false;
            rightHandDisplacement = Vector3.zero;
        }
    }

    void Update()
    {
        if (isLeftGrabbing || isRightGrabbing)
        {
            if (isLeftGrabbing)
            {
                leftHandDisplacement = leftController.transform.position - initialLeftHandPosition;
                initialLeftHandPosition = leftController.transform.position;
            }
            if (isRightGrabbing)
            {
                rightHandDisplacement = rightController.transform.position - initialRightHandPosition;
                initialRightHandPosition = rightController.transform.position;
            }

            Vector3 averageDisplacement = (leftHandDisplacement + rightHandDisplacement) / (isLeftGrabbing && isRightGrabbing ? 2 : 1) * -1;
            Vector3 normalizedAverageDisplacement = averageDisplacement.normalized;
            float force = 100f;
            
            Debug.Log($"[TEST]: average displacement: {averageDisplacement}");
            
            playerRig.position += normalizedAverageDisplacement * force * Time.deltaTime;
        }
    }
}
