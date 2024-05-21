using UnityEngine;

namespace UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets
{
    public class SimpleAnimationController : MonoBehaviour
    {
        private Animator animator;
        public string animationStateName = "YourAnimationStateName"; // Set this in the inspector or via code

        void Start()
        {
            animator = GetComponent<Animator>();
        }

        // Call this method with a value between 0 and 1 to set the animation position
        public void SetAnimationPosition(float normalizedTime)
        {
            if (animator == null)
                return;

            // Ensure the value is within the correct range
            normalizedTime = Mathf.Clamp01(normalizedTime);

            // Play the animation state at the normalized time
            animator.Play(animationStateName, 0, normalizedTime);
            animator.speed = 0; // This stops the animation from playing further automatically
        }
    }
}