using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DefaultNamespace
{
    public class ObjectSetSwitcher : MonoBehaviour
    {
        [Serializable]
        public class GameObjectsSet
        {
            public List<GameObject> set;
        }

        public InputActionProperty triggerAction;
        
        public List<GameObjectsSet> gameObjectSets;

        private int _currentSetIndex;

        private void OnEnable()
        {
            triggerAction.action.performed += SwitchSet;
        }

        private void OnDisable()
        {
            triggerAction.action.performed -= SwitchSet;
        }

        private void SwitchSet(InputAction.CallbackContext context)
        {
            var currentSet = gameObjectSets[_currentSetIndex];

            foreach (var currentGameObject in currentSet.set)
            {
                currentGameObject.SetActive(false);
            }

            _currentSetIndex++;

            if (_currentSetIndex > gameObjectSets.Count - 1) { _currentSetIndex = 0; }

            currentSet = gameObjectSets[_currentSetIndex];

            foreach (var currentGameObject in currentSet.set)
            {
                currentGameObject.SetActive(true);
            }
        }
    }
}