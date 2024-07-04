using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Timeline;

[CreateAssetMenu(fileName = "TutorialStep", menuName = "Custom/Tutorial Step")]
public class TutorialStep : ScriptableObject
{
    public string stepId;
    public TimelineAsset timelineAsset;
    public bool autoContinue = true;
    public string xrText;
    public string desktopText;
}
