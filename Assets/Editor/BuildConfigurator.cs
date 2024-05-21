using UnityEngine;
using UnityEditor;

public class BuildConfigurator : MonoBehaviour
{
    [MenuItem("Build/Build for Mobile")]
    public static void BuildMobile()
    {
        string[] clientScenes = { "Assets/Scenes/ChoiceScene.unity", "Assets/Scenes/ChoiceNetworkScene.unity" };
        BuildPipeline.BuildPlayer(clientScenes, "Builds/MobileBuild", BuildTarget.StandaloneOSX, BuildOptions.None);
    }

    [MenuItem("Build/Build for VR")]
    public static void BuildVR()
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.Android);

        string[] vrScenes = { "Assets/Scenes/UniversalChoiceScene.unity" };
        BuildPipeline.BuildPlayer(vrScenes, "Builds/VRBuild.apk", BuildTarget.Android, BuildOptions.None);
    }
    
    [MenuItem("Build/Build for macOS (Test Client)")]
    public static void BuildMacOS()
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneOSX);
        
        PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
        
        PlayerSettings.defaultScreenWidth = 720;
        PlayerSettings.defaultScreenHeight = 1280;
        PlayerSettings.resizableWindow = true;

        string[] macScenes = { "Assets/Scenes/UniversalChoiceScene.unity" };
        BuildPipeline.BuildPlayer(macScenes, "Builds/MacOSBuild2", BuildTarget.StandaloneOSX, BuildOptions.None);
    }
}