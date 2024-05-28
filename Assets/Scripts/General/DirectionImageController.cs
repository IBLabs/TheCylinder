using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class DirectionImageController : MonoBehaviour
{
    public RotationSync rotationSync;
    public RectTransform directionImage;

    void Update()
    {
        float yRot = rotationSync.networkRot.Value.eulerAngles.y;
        directionImage.rotation = Quaternion.Euler(0, 0, -yRot);
    }
}
