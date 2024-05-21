using UnityEngine;

public class EmissionToggle : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private int materialIndex = 0;
    private bool isEmissive = true;
    private Color originalEmissionColor; // Store original emission color

    public void ToggleEmission()
    {
        if (materialIndex >= meshRenderer.materials.Length)
        {
            Debug.LogError("Material index out of range");
            return;
        }

        isEmissive = !isEmissive;

        if (isEmissive)
        {
            meshRenderer.materials[materialIndex].EnableKeyword("_EMISSION");
            meshRenderer.materials[materialIndex].SetColor("_EmissionColor", originalEmissionColor); // Set back to original color
        }
        else
        {
            meshRenderer.materials[materialIndex].DisableKeyword("_EMISSION");
            originalEmissionColor = meshRenderer.materials[materialIndex].GetColor("_EmissionColor"); // Store current color
        }
    }

    void Start()
    {
        originalEmissionColor = meshRenderer.materials[materialIndex].GetColor("_EmissionColor"); // Store initial emission color
    }
}