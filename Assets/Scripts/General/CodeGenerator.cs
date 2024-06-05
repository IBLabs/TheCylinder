using System.Collections;

using TMPro;

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class CodeGenerator : MonoBehaviour
{
    public TextMeshProUGUI textMeshPro;
    public float scrambleInterval = 0.05f;
    public float revealDelay = 0.1f;
    private string code = "000000";
    private string currentScrambledText;

    private int scrambleStartIndex = 0;

    void Start()
    {
        currentScrambledText = new string('_', code.Length);
        StartCoroutine(ScrambleText());
    }

    IEnumerator ScrambleText()
    {
        while (scrambleStartIndex < code.Length)
        {
            char[] scrambledArray = currentScrambledText.ToCharArray();

            for (int i = scrambleStartIndex; i < scrambledArray.Length; i++)
            {
                scrambledArray[i] = GetRandomCharacter();
            }

            currentScrambledText = new string(scrambledArray);
            textMeshPro.text = currentScrambledText;

            yield return new WaitForSeconds(scrambleInterval);
        }
    }

    public void RevealCode(string newCode)
    {
        code = newCode;
        StartCoroutine(RevealText());
    }

    IEnumerator RevealText()
    {
        for (int i = 0; i < code.Length; i++)
        {
            char[] scrambleArray = currentScrambledText.ToCharArray();
            scrambleArray[i] = code[i];
            currentScrambledText = new string(scrambleArray);
            textMeshPro.text = currentScrambledText;
            scrambleStartIndex = i + 1;
            yield return new WaitForSeconds(revealDelay);
        }

        StopAllCoroutines();
    }

    char GetRandomCharacter()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return chars[Random.Range(0, chars.Length)];
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(CodeGenerator))]
public class CodeGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        CodeGenerator myComponent = (CodeGenerator)target;
        if (GUILayout.Button("Reveal Code"))
        {
            myComponent.RevealCode("4321DC");
        }
    }
}
#endif