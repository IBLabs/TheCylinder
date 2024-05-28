using System.Collections;
using System.Linq;

using Unity.Netcode;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private TransitionSphereController transitionController;

    [Header("Interaction")]
    public InputActionProperty nextSceneAction;
    public InputActionProperty previousSceneAction;

    [Header("Events")]
    public UnityEvent<float> TransitionWillStart;

    private string[] scenesToSkip = { "NetworkScene", "NetworkBootstrapScene" };

    private void Start()
    {
        if (!transitionController)
        {
            transitionController = GetComponent<TransitionSphereController>();
        }
    }

    private void OnEnable()
    {
        nextSceneAction.action.performed += OnLoadNextSceneAction;
    }

    private void OnDisable()
    {
        nextSceneAction.action.performed -= OnLoadNextSceneAction;
    }

    private void OnLoadNextSceneAction(InputAction.CallbackContext context)
    {
        LoadNextScene();
    }

    public void LoadNextScene()
    {
        StartCoroutine(LoadNextSceneWithTransition());
    }

    private IEnumerator LoadNextSceneWithTransition()
    {
        TransitionWillStart?.Invoke(transitionController.transitionDuration);

        yield return transitionController.FadeToBlackAsync();

        PerformLoadNextScene();
    }

    private void PerformLoadNextScene()
    {
        if (!transitionController) return;

        string nextSceneName = GetNextSceneName();

        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.SceneManager.LoadScene(nextSceneName, LoadSceneMode.Single);
        }
        else
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }

    private string GetNextSceneName()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int sceneCount = SceneManager.sceneCountInBuildSettings;
        int nextSceneIndex = (currentSceneIndex >= sceneCount - 1) ? 0 : currentSceneIndex + 1;
        string nextSceneName = GetSceneNameByIndex(nextSceneIndex);

        while (scenesToSkip.Contains(nextSceneName))
        {
            nextSceneIndex = (nextSceneIndex >= sceneCount - 1) ? 0 : nextSceneIndex + 1;
            nextSceneName = GetSceneNameByIndex(nextSceneIndex);
        }

        return nextSceneName;
    }

    private string GetSceneNameByIndex(int sceneIndex)
    {
        string nextScenePath = SceneUtility.GetScenePathByBuildIndex(sceneIndex);
        return System.IO.Path.GetFileNameWithoutExtension(nextScenePath);
    }
}