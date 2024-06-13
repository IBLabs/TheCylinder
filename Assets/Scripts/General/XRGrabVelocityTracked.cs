using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRGrabVelocityTracked : XRGrabInteractable
{
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        SetParentToXRRig();
        base.OnSelectEntered(args);
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        SetParentToWorld();
        base.OnSelectExited(args);
    }

    public void SetParentToXRRig()
    {
        Debug.Log("setting parent to XR rig");

        var oldestInteractor = this.GetOldestInteractorSelecting();

        if (oldestInteractor == null)
        {
            Debug.Log("oldest interactor is null");
            return;

        }

        transform.SetParent(oldestInteractor.transform);
    }

    public void SetParentToWorld()
    {
        Debug.Log("setting parent to world");
        transform.SetParent(null);
    }
}
