using System.Collections;

using TMPro;

using UnityEngine;

using DG.Tweening;

public abstract class LevelCanvasController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI sceneNameText;

    [SerializeField] private float animationDuration = 0.8f;

    public virtual void SetActiveScene(SceneDescriptor descriptor)
    {
        StartCoroutine(ChangeSceneNameCoroutine(descriptor.sceneName));
    }

    private IEnumerator ChangeSceneNameCoroutine(string newSceneName)
    {
        if (!sceneNameText.TryGetComponent(out RectTransform rectTransform)) yield break;

        sceneNameText.DOFade(0, animationDuration / 2f);
        yield return rectTransform.DOLocalMoveX(.5f, animationDuration / 2f).SetEase(Ease.InQuint).WaitForCompletion();

        sceneNameText.text = newSceneName;

        sceneNameText.DOFade(1, animationDuration / 2f);
        yield return rectTransform.DOLocalMoveX(0f, animationDuration / 2f).From(-.5f).SetEase(Ease.OutQuint).WaitForCompletion();
    }
}
