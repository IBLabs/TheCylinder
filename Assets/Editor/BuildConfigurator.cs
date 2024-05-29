using System.Diagnostics;
using UnityEngine;
using UnityEditor;
using DG.Tweening.Plugins;
using Unity.VisualScripting;

public class BuildConfigurator : MonoBehaviour
{
    private static readonly string[] scenes = new string[] {
        "Assets/Scenes/Utilities/NetworkBootstrapScene.unity",
        "Assets/Scenes/Minigames/RigidbodyTestScene/RigidbodyTestScene.unity",
        "Assets/Scenes/Minigames/TVShowSetScene/TVShowSetScene.unity",
        "Assets/Scenes/Minigames/SocketsScene/SocketsScene.unity",
        "Assets/Scenes/Minigames/NosePickScene/NosePickScene.unity",
        "Assets/Scenes/Minigames/BowScene/BowScene.unity",
     };

    private static readonly string platformIdentifierAssetPath = "Assets/ScriptableObjects/ProductionPlatformIdentifier.asset";

    [MenuItem("Build/Build for Mobile")]
    public static void BuildMobile()
    {
        string[] clientScenes = { "Assets/Scenes/ChoiceScene.unity", "Assets/Scenes/ChoiceNetworkScene.unity" };
        BuildPipeline.BuildPlayer(clientScenes, "Builds/MobileBuild", BuildTarget.StandaloneOSX, BuildOptions.None);
    }

    [MenuItem("Build/Build for VR")]
    public static void BuildVR()
    {
        UpdatePlatformIdentifier("VR_BUILD");

        var buildPath = "Builds/VRBuild.apk";

        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.Android);

        string[] vrScenes = scenes;

        BuildPipeline.BuildPlayer(vrScenes, buildPath, BuildTarget.Android, BuildOptions.None);
    }

    [MenuItem("Build/Build for macOS (Test Client)")]
    public static void BuildMacOS()
    {
        UpdatePlatformIdentifier("MAC1_BUILD");

        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneOSX);

        SetPlayerSettingsForMacBuild();

        string[] macScenes = scenes;
        BuildPipeline.BuildPlayer(macScenes, "Builds/PAGMAR Desktop Client", BuildTarget.StandaloneOSX, BuildOptions.None);
    }

    private static void UpdatePlatformIdentifier(string newIdentifier)
    {
        PlatformIdentifier buildIdentifier = AssetDatabase.LoadAssetAtPath<PlatformIdentifier>(platformIdentifierAssetPath);
        buildIdentifier.identifier = newIdentifier;
        EditorUtility.SetDirty(buildIdentifier);
        AssetDatabase.SaveAssets();
    }

    private static void SetPlayerSettingsForMacBuild()
    {
        PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
        PlayerSettings.defaultScreenWidth = 720;
        PlayerSettings.defaultScreenHeight = 1280;
        PlayerSettings.resizableWindow = true;
    }
}