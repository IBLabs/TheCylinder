using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEngine;

using DG.Tweening;

public class FollowCommandsCanvasController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI commandText;
    [SerializeField] private TextMeshProUGUI nextCommandText;

    private FollowMoveCommandsBroadcaster _broadcaster;

    private Coroutine _presentCoroutine;

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

    public void OnDidBroadcastMoveCommand(FollowMoveCommand moveCommand, FollowMoveCommand nextCommand)
    {
        if (_presentCoroutine != null)
        {
            StopCoroutine(_presentCoroutine);
        }

        _presentCoroutine = StartCoroutine(PresentCommand(moveCommand, nextCommand));
    }

    #endregion

    #region UI Configuration

    private IEnumerator PresentCommand(FollowMoveCommand moveCommand, FollowMoveCommand nextCommand)
    {
        commandText.text = $"{moveCommand.directionType}";

        SetNextCommandText(nextCommand);

        float duration = .2f;

        commandText.rectTransform.DOLocalMoveY(0, moveCommand.Time).From(.1f).SetEase(Ease.OutQuint);
        commandText.DOFade(1, duration).From(0).SetEase(Ease.OutQuint);

        AudioManager.Instance.PlaySound("MoveCommand1", transform.position);

        yield return new WaitForSeconds(moveCommand.Time);

        commandText.rectTransform.DOLocalMoveY(-.1f, duration).SetEase(Ease.InQuint);
        commandText.DOFade(0, duration).SetEase(Ease.InQuint);
    }

    private void SetNextCommandText(FollowMoveCommand nextCommand)
    {
        if (nextCommandText == null) return;

        if (nextCommand.directionType == FollowMoveCommand.DirectionType.None)
        {
            nextCommandText.text = "";
        }
        else
        {
            nextCommandText.text = $"{nextCommand.directionType}";
        }
    }

    #endregion
}
