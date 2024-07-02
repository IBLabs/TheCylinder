using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class DoorTransitionController : ITransitionController
{
    private const string TRIGGER_TO_BLACK = "ToBlack";
    private const string TRIGGER_TO_SCENE = "ToScene";

    [SerializeField] private Animator animator;
    [SerializeField] private bool transitionOnStart = true;

    void Start()
    {
        if (transitionOnStart) FadeToScene();
    }

    public override void FadeToBlack()
    {
        animator.SetTrigger(TRIGGER_TO_BLACK);
    }

    public override void FadeToScene()
    {
        animator.SetTrigger(TRIGGER_TO_SCENE);
    }
}
