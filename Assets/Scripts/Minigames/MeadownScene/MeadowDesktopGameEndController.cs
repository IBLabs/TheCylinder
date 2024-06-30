using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(Animator))]
public class MeadowDesktopGameEndController : MonoBehaviour
{
    [SerializeField] private Animator animator;

    [SerializeField] private GameObject winText;
    [SerializeField] private GameObject loseText;

    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image patternImage;

    [ColorUsage(true, true)]
    [SerializeField] private Color winBackgroundColor;

    [ColorUsage(true, true)]
    [SerializeField] private Color winPatternColor;

    [ColorUsage(true, true)]
    [SerializeField] private Color loseBackgroundColor;

    [ColorUsage(true, true)]
    [SerializeField] private Color losePatternColor;

    void Start()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }


    public void SetWinner(WinnerType winner)
    {
        if (!isActiveAndEnabled) return;

        winText.SetActive(winner == WinnerType.Desktop);
        loseText.SetActive(winner == WinnerType.VR);

        backgroundImage.color = winner == WinnerType.Desktop ? winBackgroundColor : loseBackgroundColor;
        patternImage.material.SetColor("_Tint", winner == WinnerType.Desktop ? winPatternColor : losePatternColor);
    }

    public void Show()
    {
        if (!isActiveAndEnabled) return;

        animator.SetTrigger("Show");
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(MeadowDesktopGameEndController))]
public class MeadowDesktopGameEndControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MeadowDesktopGameEndController myScript = (MeadowDesktopGameEndController)target;
        if (GUILayout.Button("Show VR Winner"))
        {
            myScript.SetWinner(WinnerType.VR);
            myScript.Show();
        }

        if (GUILayout.Button("Show Desktop Winner"))
        {
            myScript.SetWinner(WinnerType.Desktop);
            myScript.Show();
        }
    }
}
#endif