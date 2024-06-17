using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEngine;

using DG.Tweening;
using UnityEngine.Events;


#if UNITY_EDITOR
using UnityEditor;
#endif

public abstract class ScoreCanvasController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI desktopScoreText;
    [SerializeField] private TextMeshProUGUI desktopNameText;

    [SerializeField] private TextMeshProUGUI xrScoreText;
    [SerializeField] private TextMeshProUGUI xrNameText;

    [SerializeField] private float fadeInDuration = .1f;
    [SerializeField] private float fadeInDelay = .1f;

    [SerializeField] private float punchYDelta = .5f;
    [SerializeField] private float punchDuration = .5f;
    [SerializeField] private int punchVibrato = 10;
    [SerializeField] private float punchElasticity = 1;

    [Header("Events")]
    public UnityEvent OnScoreCanvasFinished;

    void Start()
    {
        SetInitialState();
    }

    public void ShowLastRoundWinner(WinnerType winnerType)
    {
        StartCoroutine(ShowLastRoundWinnerCoroutine(winnerType));
    }

    private void SetInitialState()
    {
        var desktopScore = NetworkScoreKeeper.Instance.DesktopScore;
        var xrScore = NetworkScoreKeeper.Instance.XrScore;

        switch (NetworkScoreKeeper.Instance.LastRoundWinner)
        {
            case WinnerType.Desktop:
                desktopScore -= 1;
                break;
            case WinnerType.VR:
                xrScore -= 1;
                break;
            case WinnerType.Unset:
                break;
        }

        desktopScoreText.text = desktopScore.ToString();
        xrScoreText.text = xrScore.ToString();
    }

    private IEnumerator ShowLastRoundWinnerCoroutine(WinnerType winnerType)
    {
        var animateInElements = new TextMeshProUGUI[] { desktopScoreText, desktopNameText, xrScoreText, xrNameText };

        foreach (var element in animateInElements)
        {
            element.alpha = 0;
        }

        foreach (var element in animateInElements)
        {
            element.DOFade(1, fadeInDuration).From(0);
            yield return new WaitForSeconds(fadeInDelay);
        }

        var winnerScoreText = winnerType == WinnerType.Desktop ? desktopScoreText : xrScoreText;
        var winnerScore = winnerType == WinnerType.Desktop ? NetworkScoreKeeper.Instance.DesktopScore : NetworkScoreKeeper.Instance.XrScore;

        if (winnerScoreText.TryGetComponent(out RectTransform winnerTextRectTransform))
        {
            winnerScoreText.text = winnerScore.ToString();
            yield return winnerTextRectTransform.DOPunchPosition(new Vector3(0, punchYDelta, 0), punchDuration, punchVibrato, punchElasticity).WaitForCompletion();
        }

        yield return new WaitForSeconds(1);

        foreach (var element in animateInElements)
        {
            element.DOFade(0, fadeInDuration).From(1);
            yield return new WaitForSeconds(fadeInDelay);
        }

        OnScoreCanvasFinished?.Invoke();
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(ScoreCanvasController), true)]
public class ScoreCanvasControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.Space();

        if (GUILayout.Button("Show Last Round Winner"))
        {
            NetworkScoreKeeper.Instance.AddDesktopScore();

            var scoreCanvasController = (ScoreCanvasController)target;
            scoreCanvasController.ShowLastRoundWinner(WinnerType.Desktop);
        }
    }
}
#endif