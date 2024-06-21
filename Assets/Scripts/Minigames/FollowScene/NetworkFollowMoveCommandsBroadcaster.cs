using System.Collections;
using System.Collections.Generic;

using Unity.Netcode;

using UnityEngine;

[RequireComponent(typeof(NetworkFollowMoveCommandsBroadcaster))]
public class NetworkFollowMoveCommandsBroadcaster : NetworkBehaviour
{
    private FollowMoveCommandsBroadcaster _localBroadcaster;
    private FollowCommandsCanvasController _canvasController;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            _localBroadcaster = GetComponent<FollowMoveCommandsBroadcaster>();

            _localBroadcaster.WillBroadcastMoveCommands.AddListener(OnWillBroadcastMoveCommands);
            _localBroadcaster.DidBroadcastMoveCommand.AddListener(OnDidBroadcastMoveCommand);
        }
        else
        {
            _canvasController = FindAnyObjectByType<FollowCommandsCanvasController>();
        }



        base.OnNetworkSpawn();
    }

    private void OnWillBroadcastMoveCommands(FollowMoveCommand[] moveCommands)
    {
        if (!IsServer) return;

        OnWillBroadcastMoveCommandsClientRpc(moveCommands);
    }

    private void OnDidBroadcastMoveCommand(FollowMoveCommand moveCommand)
    {
        if (!IsServer) return;

        OnDidBroadcastMoveCommandClientRpc(moveCommand);
    }

    [ClientRpc]
    private void OnWillBroadcastMoveCommandsClientRpc(FollowMoveCommand[] moveCommands)
    {
        if (IsServer) return;
        _canvasController.OnWillBroadcastMoveCommands(moveCommands);
    }

    [ClientRpc]
    private void OnDidBroadcastMoveCommandClientRpc(FollowMoveCommand moveCommand)
    {
        if (IsServer) return;
        _canvasController.OnDidBroadcastMoveCommand(moveCommand);
    }
}
