using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class PlayFromScene : EditorWindow
{
    private string sceneName = "YourSceneName"; // Default scene name

    [MenuItem("Tools/Play From Scene")]
    public static void ShowWindow()
    {
        GetWindow<PlayFromScene>("Play From Scene");
    }

    private void OnGUI()
    {
        GUILayout.Label("Enter Scene Name to Play From", EditorStyles.boldLabel);
        sceneName = EditorGUILayout.TextField("Scene Name", sceneName);

        if (GUILayout.Button("Play"))
        {
            PlayFromSpecificScene();
        }
    }

    private void PlayFromSpecificScene()
    {
        // Check if the scene exists in the build settings
        if (!SceneExistsInBuildSettings(sceneName))
        {
            EditorUtility.DisplayDialog("Error", "Scene not found in build settings", "OK");
            return;
        }

        // Save current scene and open the specified scene
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene("Assets/Scenes/" + sceneName + ".unity");
            EditorApplication.isPlaying = true;
        }
    }

    private bool SceneExistsInBuildSettings(string sceneName)
    {
        foreach (var scene in EditorBuildSettings.scenes)
        {
            if (scene.path.Contains(sceneName))
            {
                return true;
            }
        }
        return false;
    }
}
