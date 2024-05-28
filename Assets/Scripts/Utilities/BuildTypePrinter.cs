using UnityEngine;
using TMPro;

public class BuildTypePrinter : MonoBehaviour
{
    public PlatformIdentifier platformIdentifier;
    public TextMeshProUGUI textMeshPro;

    private void Start()
    {
        if (platformIdentifier != null && textMeshPro != null)
        {
            textMeshPro.text = platformIdentifier.identifier;
        }
        else
        {
            Debug.LogError("PlatformIdentifier or TextMeshPro reference is missing!");
        }
    }
}