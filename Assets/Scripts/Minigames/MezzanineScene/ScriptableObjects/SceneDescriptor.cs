using System.Collections;
using System.Collections.Generic;

using UnityEngine;

[CreateAssetMenu(fileName = "SceneDescriptor", menuName = "Oversight/Scene Descriptor")]
public class SceneDescriptor : ScriptableObject
{
    public string sceneId;
    public string sceneName;
    public string sceneFileName;
}
