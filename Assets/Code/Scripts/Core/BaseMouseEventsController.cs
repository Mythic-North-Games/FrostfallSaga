using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace FrostfallSaga.Core
{

    /// <summary>
    /// Defines mouse events to be detected on an element.
    /// </summary>
	[RequireComponent(typeof(Collider))]
    public class BaseMouseEventsController<T> : MonoBehaviour where T : MonoBehaviour
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

        protected bool _isMouseInside = false;
        protected T _target;
        protected List<BaseUIController> _scenesUIControllers = new();
        protected bool _mouseWasInsideUI = false;

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

            _scenesUIControllers = FindObjectsOfType<BaseUIController>().ToList();
        }

        private void OnMouseEnter()
        {
            if (_isMouseInside) return;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, ~LayerMask.GetMask("Ignore Raycast")))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    _isMouseInside = true;
                    OnElementHover?.Invoke(_target);
                }
            }
        }

        private void Update()
        {
            // If any of the UI controllers has mouse inside, do nothing
            if (_scenesUIControllers.Any(uiController => uiController.IsMouseOverUIDocument())) // * If performance problem one day, this could be an improvement.
            {
                if (!_mouseWasInsideUI)
                {
                    _mouseWasInsideUI = true;
                }
                if (_isMouseInside)
                {
                    OnElementUnhover?.Invoke(_target);
                }
                return;
            }

            if (!_isMouseInside)
            {
                return;
            }

            if (_mouseWasInsideUI)
            {
                _mouseWasInsideUI = false;
                if (_isMouseInside)
                {
                    OnElementHover?.Invoke(_target);
                }
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
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, ~LayerMask.GetMask("Ignore Raycast")))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    return;
                }
            }

            _isMouseInside = false;
            OnElementUnhover?.Invoke(_target);
        }

        private void TriggerMouseClickEvent(MouseButton mouseButton, MouseButtonDirection mouseButtonDirection)
        {
            if (mouseButton == MouseButton.LeftMouse && mouseButtonDirection == MouseButtonDirection.UP)
            {
                OnLeftMouseUp?.Invoke(_target);
            }
            else if (mouseButton == MouseButton.LeftMouse && mouseButtonDirection == MouseButtonDirection.DOWN)
            {
                OnLeftMouseDown?.Invoke(_target);
            }
            else if (mouseButton == MouseButton.RightMouse && mouseButtonDirection == MouseButtonDirection.UP)
            {
                OnRightMouseUp?.Invoke(_target);
            }
            else if (mouseButton == MouseButton.RightMouse && mouseButtonDirection == MouseButtonDirection.DOWN)
            {
                OnRightMouseDown?.Invoke(_target);
            }
            else if (mouseButton == MouseButton.MiddleMouse && mouseButtonDirection == MouseButtonDirection.UP)
            {
                OnMiddleMouseUp?.Invoke(_target);
            }
            else if (mouseButton == MouseButton.MiddleMouse && mouseButtonDirection == MouseButtonDirection.DOWN)
            {
                OnMiddleMouseDown?.Invoke(_target);
            }
        }
    }
}
