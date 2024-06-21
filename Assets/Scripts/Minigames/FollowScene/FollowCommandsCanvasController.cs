using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEngine;

using DG.Tweening;

public class FollowCommandsCanvasController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI commandText;

    private FollowMoveCommandsBroadcaster _broadcaster;

    void Start()
    {
        _broadcaster = FindAnyObjectByType<FollowMoveCommandsBroadcaster>();

        _broadcaster.WillBroadcastMoveCommands.AddListener(OnWillBroadcastMoveCommands);
        _broadcaster.DidBroadcastMoveCommand.AddListener(OnDidBroadcastMoveCommand);
    }

    #region Events

    public void OnWillBroadcastMoveCommands(FollowMoveCommand[] moveCommands)
    {
        Debug.Log($"[TEST] Received {moveCommands.Length} move commands.");
    }

    public void OnDidBroadcastMoveCommand(FollowMoveCommand moveCommand)
    {
        PresentCommand(moveCommand);
    }

    #endregion

    #region UI Configuration

    private void PresentCommand(FollowMoveCommand moveCommand)
    {
        commandText.text = $"{moveCommand.directionType}";

        commandText.DOFade(0, moveCommand.Time).From(1);
    }

    #endregion
}
