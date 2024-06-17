using UnityEngine;

public abstract class MezzanineCanvasController : MonoBehaviour
{
    [SerializeField] private CanvasGroup scoreCanvasGroup;
    [SerializeField] private CanvasGroup levelSelectionCanvasGroup;

    public void ShowScoreCanvas()
    {
        SwitchCanvasGroup(scoreCanvasGroup, levelSelectionCanvasGroup);
    }

    public void ShowLevelSelectionCanvas()
    {
        SwitchCanvasGroup(levelSelectionCanvasGroup, scoreCanvasGroup);
    }

    private void SwitchCanvasGroup(CanvasGroup canvasGroupToShow, CanvasGroup canvasGroupToHide)
    {
        canvasGroupToShow.alpha = 1;
        canvasGroupToShow.interactable = true;
        canvasGroupToShow.blocksRaycasts = true;

        canvasGroupToHide.alpha = 0;
        canvasGroupToHide.interactable = false;
        canvasGroupToHide.blocksRaycasts = false;
    }
}
