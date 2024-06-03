using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace FrostfallSaga.Core
{

    /// <summary>
    /// Check mouse actions related to the ability visual and send events when an action has happened.
    /// </summary>
	[RequireComponent(typeof(Collider))]
    public class MouseEventsController<T> : MonoBehaviour where T : MonoBehaviour
    {
        #region Mouse hover actions
        public Action<T> OnElementHover;
        public Action<T> OnElementUnhover;
        #endregion

        #region Mouse click actions
        public Action<T> OnLeftMouseDown;
        public Action<T> OnLeftMouseUp;

        public Action<T> OnRightMouseDown;
        public Action<T> OnRightMouseUp;

        public Action<T> OnMiddleMouseDown;
        public Action<T> OnMiddleMouseUp;
        #endregion

        /// <summary>
        /// To know whether the button has been pressed down or up
        /// </summary>
        protected enum MouseButtonDirection
        {
            UP = 0,
            DOWN = 1,
        }

        protected readonly Dictionary<Tuple<MouseButton, MouseButtonDirection>, Action<T>> _clickEventsByMouseButtonAndDirection = new();
        protected bool _isMouseInside = false;
        protected T _target;

        private void Awake()
        {
            if (GetComponentInParent<T>() != null)
            {
                _target = GetComponentInParent<T>();
            }
            else if (GetComponent<T>() != null)
            {
                _target = GetComponent<T>();
            }
            else if (GetComponentInChildren<T>() != null)
            {
                _target = GetComponentInChildren<T>();
            }
            else
            {
                Debug.LogError("Target of type " + typeof(T).ToString() + " not found in parent, current or children components. Will not send events.");
            }

            if (_target != null)
            {
                _clickEventsByMouseButtonAndDirection.Add(new(MouseButton.LeftMouse, MouseButtonDirection.DOWN), OnLeftMouseDown);
                _clickEventsByMouseButtonAndDirection.Add(new(MouseButton.LeftMouse, MouseButtonDirection.UP), OnLeftMouseUp);

                _clickEventsByMouseButtonAndDirection.Add(new(MouseButton.RightMouse, MouseButtonDirection.DOWN), OnRightMouseDown);
                _clickEventsByMouseButtonAndDirection.Add(new(MouseButton.RightMouse, MouseButtonDirection.UP), OnRightMouseUp);

                _clickEventsByMouseButtonAndDirection.Add(new(MouseButton.MiddleMouse, MouseButtonDirection.DOWN), OnMiddleMouseDown);
                _clickEventsByMouseButtonAndDirection.Add(new(MouseButton.MiddleMouse, MouseButtonDirection.UP), OnMiddleMouseUp);
            }
        }

        private void OnMouseEnter()
        {
            _isMouseInside = true;
            OnElementHover?.Invoke(_target);
        }

        private void Update()
        {
            if (!_isMouseInside)
            {
                return;
            }

            foreach (MouseButton mouseButton in Enum.GetValues(typeof(MouseButton)))
            {
                if (Input.GetMouseButtonUp((int)mouseButton))
                {
                    TriggerMouseClickEvent(mouseButton, MouseButtonDirection.UP);
                }
                else if (Input.GetMouseButtonDown((int)mouseButton))
                {
                    TriggerMouseClickEvent(mouseButton, MouseButtonDirection.DOWN);
                }
            }
        }

        private void OnMouseExit()
        {
            _isMouseInside = false;
            OnElementUnhover?.Invoke(_target);
        }

        private void TriggerMouseClickEvent(MouseButton mouseButton, MouseButtonDirection mouseButtonDirection)
        {
            Tuple<MouseButton, MouseButtonDirection> actionIdentifier = new(mouseButton, mouseButtonDirection);
            if (_clickEventsByMouseButtonAndDirection.TryGetValue(actionIdentifier, out Action<T> actionToTrigger))
            {
                actionToTrigger?.Invoke(_target);
            }
        }
    }
}
