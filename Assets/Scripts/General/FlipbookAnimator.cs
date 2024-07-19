using UnityEngine;

public class FlipbookAnimator : MonoBehaviour
{
    [SerializeField] private bool randomizeOffset = true;
    [SerializeField] private Renderer targetRenderer;
    [SerializeField] private int materialIndex = 0;

    public Material material; // The material to animate
    public int totalFrames = 16; // Total number of frames in the texture
    public int rows = 4; // Number of rows in the texture
    public int columns = 4; // Number of columns in the texture
    public float framesPerSecond = 10f; // Frames per second

    private float frameTime; // Time per frame
    private int currentFrame; // Current frame index
    private float timer; // Timer to track frame changes

    void Start()
    {
        if (targetRenderer != null)
        {
            if (materialIndex < 0 || materialIndex >= targetRenderer.materials.Length)
            {
                Debug.LogError("Material index is out of range.");
                return;
            }

            material = targetRenderer.materials[materialIndex];
        }
        else if (material == null)
        {
            Debug.LogError("Material is not set.");
            return;
        }

        frameTime = 1f / framesPerSecond;
        currentFrame = randomizeOffset ? Random.Range(0, totalFrames) : 0;
        timer = 0f;
    }

    void Update()
    {
        if (material == null)
            return;

        timer += Time.deltaTime; // Update the timer

        if (timer >= frameTime)
        {
            timer -= frameTime; // Reset the timer for the next frame
            currentFrame = (currentFrame + 1) % totalFrames; // Move to the next frame and loop back if necessary

            UpdateUVOffset(); // Update the UV offset based on the current frame
        }
    }

    void UpdateUVOffset()
    {
        // Calculate the row and column of the current frame
        int row = currentFrame / columns;
        int column = currentFrame % columns;

        // Calculate the size of each frame
        Vector2 frameSize = new Vector2(1f / columns, 1f / rows);

        // Calculate the UV offset for the current frame
        Vector2 offset = new Vector2(column * frameSize.x, 1f - frameSize.y - row * frameSize.y);

        // Apply the UV offset and scale to the material
        // material.SetTextureOffset("_Offset", offset);
        // material.SetTextureScale("_Tiling", frameSize);
        material.SetVector("_Offset", offset);
        material.SetVector("_Tiling", frameSize);
    }
}
