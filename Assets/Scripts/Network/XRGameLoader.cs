using UnityEngine;
using UnityEngine.InputSystem;

public class XRGameLoader : MonoBehaviour
{
    public InputActionProperty triggerAction;

    [SerializeField] private SceneLoader targetSceneLoader;

    private void OnEnable()
    {
        triggerAction.action.performed += OnInputPerformed;
    }

    private void OnDisable()
    {
        triggerAction.action.performed -= OnInputPerformed;
    }

    private void OnInputPerformed(InputAction.CallbackContext context)
    {
        targetSceneLoader.LoadNextScene();
    }
}