using UnityEngine;

public abstract class ITransitionController : MonoBehaviour
{
    public abstract void FadeToScene();
    public abstract void FadeToBlack();
}