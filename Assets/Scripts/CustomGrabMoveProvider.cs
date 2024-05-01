using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

namespace DefaultNamespace
{
    [AddComponentMenu("XR/Locomotion/Custom Grab Move Provider", 11)]
    public class CustomGrabMoveProvider : ConstrainedMoveProvider
    {
        [SerializeField]
        Transform m_ControllerTransform;
        public Transform controllerTransform
        {
            get => m_ControllerTransform;
            set
            {
                m_ControllerTransform = value;
                GatherControllerInteractors();
            }
        }

        [SerializeField]
        bool m_EnableMoveWhileSelecting;
        public bool enableMoveWhileSelecting
        {
            get => m_EnableMoveWhileSelecting;
            set => m_EnableMoveWhileSelecting = value;
        }

        [SerializeField]
        float m_MoveFactor = 1f;
        public float moveFactor
        {
            get => m_MoveFactor;
            set => m_MoveFactor = value;
        }

        [SerializeField]
        InputActionProperty m_GrabMoveAction = new InputActionProperty(new InputAction("Grab Move", type: InputActionType.Button));
        public InputActionProperty grabMoveAction
        {
            get => m_GrabMoveAction;
            set => SetInputActionProperty(ref m_GrabMoveAction, value);
        }
        
        public bool canMove { get; set; } = true;
        
        [SerializeField]
        private float friction = 0.1f;
        
        bool m_IsMoving;

        Vector3 m_PreviousControllerLocalPosition;

        readonly List<IXRSelectInteractor> m_ControllerInteractors = new List<IXRSelectInteractor>();
        
        private Vector3 currentMomentum;
        
        protected override void Awake()
        {
            base.Awake();

            if (m_ControllerTransform == null)
                m_ControllerTransform = transform;

            GatherControllerInteractors();
        }
        
        protected void OnEnable()
        {
            m_GrabMoveAction.EnableDirectAction();
        }
        
        protected void OnDisable()
        {
            m_GrabMoveAction.DisableDirectAction();
        }
        
        protected override Vector3 ComputeDesiredMove(out bool attemptingMove)
        {
            attemptingMove = false;
            var xrOrigin = system.xrOrigin?.Origin;
            var wasMoving = m_IsMoving;

            m_IsMoving = canMove && IsGrabbing() && xrOrigin != null;

            if (!m_IsMoving) return Vector3.zero;

            var controllerLocalPosition = controllerTransform.localPosition;

            if (!wasMoving && m_IsMoving)
            {
                // do not move the first frame of grab
                m_PreviousControllerLocalPosition = controllerLocalPosition;
                return Vector3.zero;
            }

            if (m_IsMoving)
            {
                var originTransform = xrOrigin.transform;
                currentMomentum = originTransform.TransformVector(m_PreviousControllerLocalPosition - controllerLocalPosition) * m_MoveFactor;
                m_PreviousControllerLocalPosition = controllerLocalPosition;    
            }
            else
            {
                // apply friction to gradually stop the momentum
                currentMomentum *= (1 - friction);
                if (currentMomentum.magnitude < 0.01f) currentMomentum = Vector3.zero;
            }
            
            attemptingMove = (currentMomentum != Vector3.zero);
            
            return currentMomentum;
        }
        
        public bool IsGrabbing()
        {
            return m_GrabMoveAction.action.IsPressed() && (m_EnableMoveWhileSelecting || !ControllerHasSelection());
        }

        void GatherControllerInteractors()
        {
            m_ControllerInteractors.Clear();
            if (m_ControllerTransform != null)
                m_ControllerTransform.transform.GetComponentsInChildren(m_ControllerInteractors);
        }

        bool ControllerHasSelection()
        {
            foreach (var interactor in m_ControllerInteractors)
            {
                if (interactor.hasSelection)
                    return true;
            }

            return false;
        }

        void SetInputActionProperty(ref InputActionProperty property, InputActionProperty value)
        {
            if (Application.isPlaying)
                property.DisableDirectAction();

            property = value;

            if (Application.isPlaying && isActiveAndEnabled)
                property.EnableDirectAction();
        }
    }
}