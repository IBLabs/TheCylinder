using System.Collections;

using TMPro;

using UnityEngine;
using UnityEngine.Events;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class CodeGenerator : MonoBehaviour
{
    public TextMeshProUGUI textMeshPro;
    public AudioSource sfxAudioSource;
    public AudioClip codeScrambleSound;
    public float scrambleInterval = 0.05f;
    public float revealDelay = 0.1f;

    public UnityEvent DidFinishRevealCode;


    private string code = "000000";
    private string currentScrambledText;

    private int scrambleStartIndex = 0;

    private bool _shouldRevealCodeOnReceived = false;

    void Start()
    {
        currentScrambledText = new string('_', code.Length);
    }

    public void StartScrambling()
    {
        StartCoroutine(ScrambleText());
    }

    IEnumerator ScrambleText()
    {
        sfxAudioSource.clip = codeScrambleSound;
        sfxAudioSource.loop = true;
        sfxAudioSource.Play();

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

    public void RevealCode()
    {
        if (code == "000000")
        {
            _shouldRevealCodeOnReceived = true;
            return;
        }

        StartCoroutine(RevealText());
    }

    public void OnRelayJoinCodeReceived(string newCode)
    {
        code = newCode;

        if (_shouldRevealCodeOnReceived)
        {
            RevealCode();
        }
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

            if (i < code.Length - 1) yield return new WaitForSeconds(revealDelay);
        }

        StopAllCoroutines();

        sfxAudioSource.Stop();

        DidFinishRevealCode?.Invoke();
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
            myComponent.OnRelayJoinCodeReceived("4321DC");
        }
    }
}
#endif