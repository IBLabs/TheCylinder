using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class SpectatorSphereController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.DOMoveY(-0.3f, 1f).SetRelative(true).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
