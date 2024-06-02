using DG.Tweening;

using UnityEngine;
using UnityEngine.UI;

public class LobbyDesktopCanvasController : MonoBehaviour
{
    [SerializeField] private Image backgroundImage;
    [SerializeField] private CanvasGroup uiGroup;

    public void OnClientConnected(ulong clientId)
    {
        SetCanvasVisible(false);
    }

    private void SetCanvasVisible(bool isVisible)
    {
        backgroundImage.DOFade(isVisible ? 1 : 0, 0.2f);
        uiGroup.DOFade(isVisible ? 1 : 0, 0.2f);
    }
}
