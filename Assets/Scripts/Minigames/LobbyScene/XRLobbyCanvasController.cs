using System.Collections;

using TMPro;

using UnityEditor;

using UnityEngine;
using UnityEngine.Rendering;

using DG.Tweening;

public class XRLobbyCanvasController : MonoBehaviour
{
    [SerializeField] private CanvasGroup startGroup;
    [SerializeField] private CanvasGroup codeGroup;
    [SerializeField] private CodeGenerator codeGenerator;

    [SerializeField] private TextMeshProUGUI titleText;

    void Start()
    {
        SetInitialState();
    }

    private void SetInitialState()
    {
        startGroup.alpha = 1;
        startGroup.interactable = true;
        startGroup.blocksRaycasts = true;

        codeGroup.alpha = 0;
        codeGroup.interactable = false;
        codeGroup.blocksRaycasts = false;
    }

    public void TransitionToCodeGroup()
    {
        StartCoroutine(TransitionToCodeCoroutine());
    }

    private IEnumerator TransitionToCodeCoroutine()
    {
        codeGenerator.StartScrambling();

        startGroup.interactable = false;
        startGroup.blocksRaycasts = false;

        // startGroup.alpha = 0;
        yield return startGroup.DOFade(0, 0.3f).WaitForCompletion();

        // codeGroup.alpha = 1;
        yield return codeGroup.DOFade(1, 0.3f).WaitForCompletion();

        codeGroup.interactable = true;
        codeGroup.blocksRaycasts = true;

        yield return new WaitForSeconds(1f);

        codeGroup.GetComponentInChildren<CodeGenerator>().RevealCode();
    }

    public void OnDidFinishRevealCode()
    {
        // TODO: do something
    }

    private IEnumerator CodeGenerationTextCoroutine()
    {
        int dotCount = 1;
        while (true)
        {
            titleText.text = "GENERATING CODE" + new string('.', dotCount);
            dotCount = (dotCount + 1) % 4;
            yield return new WaitForSeconds(0.33f);
        }
    }
}