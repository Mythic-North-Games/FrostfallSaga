using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace FrostfallSaga.Utils.Inputs
{
    public class GlobalWorldMouseInteractor : MonoBehaviourPersistingSingleton<GlobalWorldMouseInteractor>
    {
        public Action<GameObject> onLeftDown;
        public Action<GameObject> onRightDown;
        public Action<GameObject> onMiddleDown;

        public Action<GameObject> onLeftUp;
        public Action<GameObject> onRightUp;
        public Action<GameObject> onMiddleUp;

        public Action<GameObject> onLeftClickHold;
        public Action<GameObject> onRightClickHold;
        public Action<GameObject> onMiddleClickHold;

        public Action<GameObject> onLeftClickHoldReleased;
        public Action<GameObject> onRightClickHoldReleased;
        public Action<GameObject> onMiddleClickHoldReleased;

        public Action<GameObject> onHovered;
        public Action<GameObject> onUnhovered;

        [SerializeField]
        private string[] allowedLayerNames =
        {
            ELayersName.DEFAULT.ToLayerName(),
            ELayersName.CELL.ToLayerName(),
            ELayersName.FIGHTER.ToLayerName(),
            ELayersName.BUILDING.ToLayerName(),
        };
        private LayerMask allowedLayers;

        [SerializeField] private float rayDistance = 100f;
        [SerializeField] private float holdThreshold = 0.4f;

        private GameObject _currentHoveredTarget;

        private float _leftDownTime;
        private float _rightDownTime;
        private float _middleDownTime;

        private bool _leftHoldTriggered;
        private bool _rightHoldTriggered;
        private bool _middleHoldTriggered;

        private bool _leftWasLongClick;
        private bool _rightWasLongClick;
        private bool _middleWasLongClick;

        protected override void Init()
        {
            foreach (var name in allowedLayerNames)
                allowedLayers |= 1 << LayerMask.NameToLayer(name);
        }

        private void Update()
        {
            if (IsPointerOverUI())
            {
                UnhoverIfNecessary();
                return;
            }

            UpdateHover();

            HandleMouseState(Mouse.current.leftButton, ref _leftDownTime, ref _leftHoldTriggered, ref _leftWasLongClick,
                onLeftDown, onLeftUp, onLeftClickHold, onLeftClickHoldReleased);

            HandleMouseState(Mouse.current.rightButton, ref _rightDownTime, ref _rightHoldTriggered, ref _rightWasLongClick,
                onRightDown, onRightUp, onRightClickHold, onRightClickHoldReleased);

            HandleMouseState(Mouse.current.middleButton, ref _middleDownTime, ref _middleHoldTriggered, ref _middleWasLongClick,
                onMiddleDown, onMiddleUp, onMiddleClickHold, onMiddleClickHoldReleased);
        }

        private void HandleMouseState(
            ButtonControl button,
            ref float downTime,
            ref bool holdTriggered,
            ref bool wasLongClick,
            Action<GameObject> onDown,
            Action<GameObject> onUp,
            Action<GameObject> onHold,
            Action<GameObject> onLongReleased
        )
        {
            if (button.wasPressedThisFrame)
            {
                if (TryGetHoveredTarget(out GameObject target))
                {
                    downTime = Time.time;
                    holdTriggered = false;
                    wasLongClick = false;
                    onDown?.Invoke(target);
                }
            }

            if (button.wasReleasedThisFrame)
            {
                if (TryGetHoveredTarget(out GameObject target))
                {
                    onUp?.Invoke(target);

                    float heldTime = Time.time - downTime;

                    if (heldTime > holdThreshold)
                    {
                        wasLongClick = true;
                        onLongReleased?.Invoke(target);
                    }
                }
            }

            if (button.isPressed && !holdTriggered)
            {
                if ((Time.time - downTime) > holdThreshold)
                {
                    if (TryGetHoveredTarget(out GameObject target))
                    {
                        holdTriggered = true;
                        wasLongClick = true;
                        onHold?.Invoke(target);
                    }
                }
            }
        }

        private void UpdateHover()
        {
            if (TryGetHoveredTarget(out GameObject target))
            {
                if (!Equals(_currentHoveredTarget, target))
                {
                    if (_currentHoveredTarget != null) onUnhovered?.Invoke(_currentHoveredTarget);
                    _currentHoveredTarget = target;
                    onHovered?.Invoke(target);
                }
            }
            else
            {
                UnhoverIfNecessary();
            }
        }

        private void UnhoverIfNecessary()
        {
            if (_currentHoveredTarget != null)
            {
                onUnhovered?.Invoke(_currentHoveredTarget);
                _currentHoveredTarget = null;
            }
        }

        private bool TryGetHoveredTarget(out GameObject target)
        {
            Ray ray = UnityEngine.Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (Physics.Raycast(ray, out RaycastHit hit, rayDistance, allowedLayers) &&
                hit.collider != null && hit.collider.gameObject != null)
            {
                target = hit.collider.gameObject;
                return true;
            }

            target = null;
            return false;
        }

        private bool IsPointerOverUI()
        {
            return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
        }

        
    }
}
