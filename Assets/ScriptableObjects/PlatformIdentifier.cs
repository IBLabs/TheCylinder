using UnityEngine;

[CreateAssetMenu(fileName = "BuildIdentifier", menuName = "ScriptableObjects/BuildIdentifier", order = 1)]
public class PlatformIdentifier : ScriptableObject
{
    public static string VR_IDENTIFIER = "VR_BUILD";
    public static string MOBILE_IDENTIFIER = "MOBILE_BUILD";

    public string identifier;
}