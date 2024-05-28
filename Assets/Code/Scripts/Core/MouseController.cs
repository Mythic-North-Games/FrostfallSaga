using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace FrostfallSaga.Core
{
    /// <summary>
    /// Responsible for tracking mouse events throughout the entire game.
    /// </summary>
    public class MouseController : MonoBehaviourPersistingSingleton<MouseController>
    {
        #region Mouse hover actions
        public Action<RaycastHit> OnElementHover;
        public Action<RaycastHit> OnElementUnhover;
        #endregion

        #region Mouse click actions
        public Action<RaycastHit> OnLeftMouseDown;
        public Action<RaycastHit> OnLeftMouseUp;

        public Action<RaycastHit> OnRightMouseDown;
        public Action<RaycastHit> OnRightMouseUp;

        public Action<RaycastHit> OnMiddleMouseDown;
        public Action<RaycastHit> OnMiddleMouseUp;
        #endregion

        /// <summary>
        /// To know whether the button has been pressed down or up
        /// </summary>
        private enum MouseButtonDirection
        {
            UP = 0,
            DOWN = 1,
        }

        private readonly Dictionary<Tuple<MouseButton, MouseButtonDirection>, Action<RaycastHit>> _clickEventsByMouseButtonAndDirection = new();
        private RaycastHit _currentHoveredElement;
        private bool _currentHoveredElementSet = false;

        private void OnEnable()
        {
            SceneManager.sceneUnloaded += OnSceneUnload;
        }

        private void Start()
        {
            _clickEventsByMouseButtonAndDirection.Add(new(MouseButton.LeftMouse, MouseButtonDirection.DOWN), OnLeftMouseDown);
            _clickEventsByMouseButtonAndDirection.Add(new(MouseButton.LeftMouse, MouseButtonDirection.UP), OnLeftMouseUp);

            _clickEventsByMouseButtonAndDirection.Add(new(MouseButton.RightMouse, MouseButtonDirection.DOWN), OnRightMouseDown);
            _clickEventsByMouseButtonAndDirection.Add(new(MouseButton.RightMouse, MouseButtonDirection.UP), OnRightMouseUp);

            _clickEventsByMouseButtonAndDirection.Add(new(MouseButton.MiddleMouse, MouseButtonDirection.DOWN), OnMiddleMouseDown);
            _clickEventsByMouseButtonAndDirection.Add(new(MouseButton.MiddleMouse, MouseButtonDirection.UP), OnMiddleMouseUp);
        }

        private void Update()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hoveredElement, Mathf.Infinity))
            {
                if (!_currentHoveredElement.Equals(hoveredElement))
                {
                    if (_currentHoveredElementSet)
                    {
                        OnElementUnhover.Invoke(_currentHoveredElement);
                    }
                    _currentHoveredElement = hoveredElement;
                    OnElementHover.Invoke(_currentHoveredElement);
                }
                if (!_currentHoveredElementSet)
                {
                    _currentHoveredElementSet = true;
                }

                TriggerMouseClickEventIfAny(hoveredElement);
            }
            else if (_currentHoveredElementSet)
            {
                OnElementUnhover.Invoke(_currentHoveredElement);
                _currentHoveredElementSet = false;
            }
        }

        private void TriggerMouseClickEventIfAny(RaycastHit hoveredElement)
        {
            foreach (MouseButton mouseButton in Enum.GetValues(typeof(MouseButton)))
            {
                if (Input.GetMouseButtonUp((int)mouseButton))
                {
                    TriggerMouseClickEvent(mouseButton, MouseButtonDirection.UP, hoveredElement);
                }
                else if (Input.GetMouseButtonDown((int)mouseButton))
                {
                    TriggerMouseClickEvent(mouseButton, MouseButtonDirection.DOWN, hoveredElement);
                }
            }
        }

        private void TriggerMouseClickEvent(MouseButton mouseButton, MouseButtonDirection mouseButtonDirection, RaycastHit hoveredElement)
        {
            Tuple<MouseButton, MouseButtonDirection> actionIdentifier = new(mouseButton, mouseButtonDirection);
            if (_clickEventsByMouseButtonAndDirection.TryGetValue(actionIdentifier, out Action<RaycastHit> actionToTrigger))
            {
                actionToTrigger?.Invoke(hoveredElement);
            }
        }

        private void OnSceneUnload(Scene _scene)
        {
            _currentHoveredElementSet = false;
        }

        private void OnDisable()
        {
            SceneManager.sceneUnloaded -= OnSceneUnload;
        }
    }
}