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

    [MenuItem("Key Scenes/Prison (New)")]
    private static void SwitchToScene3()
    {
        SwitchToScene("Minigames/PrisonScene/PrisonScene");
    }

    [MenuItem("Key Scenes/Meadow")]
    private static void SwitchToScene4()
    {
        SwitchToScene("Minigames/MeadowScene/MeadowScene");
    }

    [MenuItem("Key Scenes/Mezzanine")]
    private static void SwitchToScene5()
    {
        SwitchToScene("Minigames/MezzanineScene/MezzanineScene");
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