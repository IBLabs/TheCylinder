using System.Collections.Generic;

using Unity.Netcode;

using UnityEngine.Events;

public class NetworkSimpleButtonCoordinator : NetworkBehaviour
{
    public List<UnityEvent> buttonEvents;

    public void OnButton1Pressed()
    {
        InvokeButtonEventServerRpc(0);
    }

    public void OnButton2Pressed()
    {
        InvokeButtonEventServerRpc(1);
    }

    public void OnButton3Pressed()
    {
        InvokeButtonEventServerRpc(2);
    }

    public void OnButton4Pressed()
    {
        InvokeButtonEventServerRpc(3);
    }

    [ServerRpc(RequireOwnership = false)]
    private void InvokeButtonEventServerRpc(int buttonIndex)
    {
        if (buttonIndex < 0 || buttonIndex >= buttonEvents.Count)
        {
            return;
        }

        buttonEvents[buttonIndex].Invoke();
    }
}
