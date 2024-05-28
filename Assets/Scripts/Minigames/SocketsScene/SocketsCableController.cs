using System.Collections;

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SocketsCableController : MonoBehaviour
{
    [SerializeField] private XRInteractionManager interactionManager;
    [SerializeField] private XRGrabInteractable startInteractable;
    [SerializeField] private XRGrabInteractable endInteractable;

    [SerializeField] private XRSocketInteractor startInitialSocket;
    [SerializeField] private XRSocketInteractor endInitialSocket;

    IEnumerator Start()
    {
        yield return new WaitForSeconds(.2f);

        CircleSpawner[] circleSpawners = FindObjectsByType<CircleSpawner>(FindObjectsSortMode.None);

        CircleSpawner randomSpawner = circleSpawners[Random.Range(0, circleSpawners.Length)];
        XRSocketInteractor[] socketInteractors = randomSpawner.GetComponentsInChildren<XRSocketInteractor>();
        XRSocketInteractor randomSocketInteractor = socketInteractors[Random.Range(0, socketInteractors.Length)];

        CircleSpawner randomSpawner2 = circleSpawners[Random.Range(0, circleSpawners.Length)];
        while (randomSpawner2 == randomSpawner)
        {
            randomSpawner2 = circleSpawners[Random.Range(0, circleSpawners.Length)];
        }

        XRSocketInteractor[] socketInteractors2 = randomSpawner2.GetComponentsInChildren<XRSocketInteractor>();
        XRSocketInteractor randomSocketInteractor2 = socketInteractors2[Random.Range(0, socketInteractors2.Length)];

        if (startInteractable is IXRSelectInteractable selectInteractable)
        {
            randomSocketInteractor.StartManualInteraction(selectInteractable);
        }

        if (endInteractable is IXRSelectInteractable selectInteractable2)
        {
            randomSocketInteractor2.StartManualInteraction(selectInteractable2);
        }
    }
}
