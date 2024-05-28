using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SliderTransitionController : ITransitionController
{
    public Slider slider;
    public float transitionDuration = 1f;
    public Ease transitionEase = Ease.Linear; // Add a public variable for the easing type

    private bool isTransitioning;

    private void Start()
    {
        FadeToScene();
    }

    public override void FadeToScene()
    {
        if (isTransitioning) return;

        isTransitioning = true;

        slider.direction = Slider.Direction.RightToLeft;

        slider.DOValue(0f, transitionDuration).From(1f).SetEase(transitionEase).OnComplete(() =>
        {
            isTransitioning = false;
        });
    }

    public override void FadeToBlack()
    {
        if (isTransitioning) return;

        isTransitioning = true;

        slider.direction = Slider.Direction.LeftToRight;

        slider.DOValue(1f, transitionDuration).From(0f).SetEase(transitionEase).OnComplete(() =>
        {
            isTransitioning = false;
        });
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(SliderTransitionController))]
    public class SliderTransitionControllerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            SliderTransitionController controller = (SliderTransitionController)target;

            if (GUILayout.Button("Play Fade To Scene"))
            {
                controller.FadeToScene();
            }

            if (GUILayout.Button("Play Fade To Black"))
            {
                controller.FadeToBlack();
            }
        }
    }
#endif
}
