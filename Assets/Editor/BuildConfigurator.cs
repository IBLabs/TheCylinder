using UnityEngine;
using UnityEditor;
using System.Diagnostics;

public class BuildConfigurator : MonoBehaviour
{
    private static readonly string[] scenes = new string[] {
        "Assets/Scenes/Utilities/NetworkBootstrapScene.unity",
        "Assets/Scenes/Minigames/MezzanineScene/MezzanineScene.unity",
        "Assets/Scenes/Minigames/PrisonScene/PrisonScene.unity",
        "Assets/Scenes/Minigames/MeadowScene/MeadowScene.unity",
        "Assets/Scenes/Minigames/RigidbodyTestScene/RigidbodyTestScene.unity",
     };



    [MenuItem("Build/Build for Mobile")]
    public static void BuildMobile()
    {
        string[] clientScenes = { "Assets/Scenes/ChoiceScene.unity", "Assets/Scenes/ChoiceNetworkScene.unity" };
        BuildPipeline.BuildPlayer(clientScenes, "Builds/MobileBuild", BuildTarget.StandaloneOSX, BuildOptions.None);
    }

    [MenuItem("Build/Build for VR")]
    public static void BuildVR()
    {
        var buildPath = "Builds/VRBuild.apk";

        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.Android);

        string[] vrScenes = scenes;

        BuildPipeline.BuildPlayer(vrScenes, buildPath, BuildTarget.Android, BuildOptions.None);
        RunInstallCommand();
    }

    [MenuItem("Build/Build for macOS (Test Client)")]
    public static void BuildMacOS()
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneOSX);

        SetPlayerSettingsForDesktopBuild();

        string[] macScenes = scenes;
        BuildPipeline.BuildPlayer(macScenes, "Builds/PAGMAR Desktop Client", BuildTarget.StandaloneOSX, BuildOptions.None);
    }

    [MenuItem("Build/Build for Windows")]
    public static void BuildWindows()
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64);

        SetPlayerSettingsForDesktopBuild();

        string[] windowsScenes = scenes;
        BuildPipeline.BuildPlayer(windowsScenes, "Builds/WindowsBuild/Oversight.exe", BuildTarget.StandaloneWindows64, BuildOptions.None);
    }

    [MenuItem("Build/Build for VR + macOS")]
    public static void BuildVRMacOS()
    {
        BuildMacOS();
        BuildVR();
    }

    private static void SetPlayerSettingsForDesktopBuild()
    {
        PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
        PlayerSettings.defaultScreenWidth = 720;
        PlayerSettings.defaultScreenHeight = 1280;
        PlayerSettings.resizableWindow = true;
    }

    private static void RunInstallCommand()
    {
        if (SystemInfo.operatingSystemFamily == OperatingSystemFamily.MacOSX)
        {
            string command = "adb install -r ~/Documents/Development/Unity/TheCylinder/Builds/VRBuild.apk";
            ProcessStartInfo procStartInfo = new ProcessStartInfo("/bin/bash", "-c \"" + command + "\"")
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            Process proc = new Process
            {
                StartInfo = procStartInfo
            };
            proc.Start();

            string result = proc.StandardOutput.ReadToEnd();
            UnityEngine.Debug.Log(result);
        }
        else
        {
            UnityEngine.Debug.Log("Current OS is not macOS.");
        }
    }
}