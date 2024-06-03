using UnityEditor;
using UnityEditor.SceneManagement;

public class SceneSwitcher : EditorWindow
{
    [MenuItem("Key Scenes/Lobby")]
    private static void SwitchToScene1()
    {
        SwitchToScene("Utilities/NetworkBootstrapScene");
    }

    [MenuItem("Key Scenes/Prison")]
    private static void SwitchToScene2()
    {
        SwitchToScene("Minigames/RigidbodyTestScene/RigidbodyTestScene");
    }

    // Add more scene switch methods here

    private static void SwitchToScene(string sceneName)
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene($"Assets/Scenes/{sceneName}.unity");
        }
    }
}