using System;
using System.Collections;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements.Experimental;

namespace FrostfallSaga.Utils.Camera
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera virtualCamera;

        [field: SerializeField] public bool AllowZoom { get; private set; } = true;
        [field: SerializeField] public float MinFOV { get; private set; } = 20.0f;
        [field: SerializeField] public float MaxFOV { get; private set; } = 120.0f;
        [field: SerializeField] public float BaseFOV { get; private set; } = 75.0f;
        [field: SerializeField] public float ZoomMultiplier { get; private set; } = 0.01f;

        [field: SerializeField] public Transform mouseFollowTarget;
        [field: SerializeField] public Vector2 MouseTargetOffset { get; private set; } = new(0.1f, 0.1f);
        [field: SerializeField] public float MaxTranslationSpeed { get; private set; } = 30f;
        [field: SerializeField] public float MinY { get; private set; } = 2f;
        [field: SerializeField] public float MaxY { get; private set; } = 10f;

        [SerializeField] private Texture2D grabCursor;
        [SerializeField] private Texture2D mouseCursor;
        [SerializeField] private float rotationSpeed = 10f;

        private Transform _initialTarget;
        private Vector3 _lastMousePosition;
        private Vector3 _mouseDragStartPosition;
        private const float DRAG_THRESHOLD = 4f;
        private bool _isDragging;
        private bool _isRotating;
        private bool _hasToggledToMouseFollow;

        private void Awake()
        {
            if (!virtualCamera) Debug.LogError("No camera to control");
            Cursor.SetCursor(mouseCursor, Vector2.zero, CursorMode.Auto);
        }

        private void Start()
        {
            SetFOV(BaseFOV);
        }

        public void ZoomIn(float zoomAmount, float zoomDuration)
        {
            StartCoroutine(ZoomCoroutine(zoomAmount, zoomDuration));
        }

        private IEnumerator ZoomCoroutine(float zoomAmount, float zoomDuration)
        {
            float startFOV = virtualCamera.m_Lens.FieldOfView;
            float targetFOV = Mathf.Clamp(startFOV - zoomAmount, MinFOV, MaxFOV);
            float elapsedTime = 0f;

            while (elapsedTime < zoomDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / zoomDuration);
                SetFOV(Mathf.Lerp(startFOV, targetFOV, Easing.OutQuad(t)));
                yield return null;
            }

            SetFOV(targetFOV);
        }

        private void Update()
        {
            if (Mouse.current == null) return;

            // Zoom
            if (AllowZoom)
            {
                float deltaZoom = -Mouse.current.scroll.ReadValue().y * ZoomMultiplier;
                SetFOV(virtualCamera.m_Lens.FieldOfView + deltaZoom);
            }

            // Toggle follow
            if (Mouse.current.middleButton.wasReleasedThisFrame)
            {
                ToggleFollowTarget();
            }

            // Start drag
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                _mouseDragStartPosition = Mouse.current.position.ReadValue();
                _isDragging = false;
            }

            // Dragging
            if (Mouse.current.leftButton.isPressed)
            {
                Vector2 currentPos = Mouse.current.position.ReadValue();
                if (!_isDragging && Vector2.Distance(currentPos, _mouseDragStartPosition) > DRAG_THRESHOLD)
                {
                    _isDragging = true;

                    if (_hasToggledToMouseFollow && !IsFollowingMouse() && virtualCamera.Follow && mouseFollowTarget)
                    {
                        _initialTarget = virtualCamera.Follow;
                        mouseFollowTarget.position = _initialTarget.position;
                        virtualCamera.Follow = mouseFollowTarget;
                    }
                }

                if (_isDragging && IsFollowingMouse())
                {
                    Cursor.SetCursor(grabCursor, Vector2.zero, CursorMode.Auto);
                    UpdateMouseFollowTargetPosition();
                }
            }

            // Camera rotation
            if (Mouse.current.rightButton.isPressed)
            {
                Cursor.SetCursor(grabCursor, Vector2.zero, CursorMode.Auto);
                UpdateCameraRotation();
            }

            // Reset drag/rotation
            if (Mouse.current.leftButton.wasReleasedThisFrame || Mouse.current.rightButton.wasReleasedThisFrame)
            {
                _isDragging = false;
                _isRotating = false;
                Cursor.SetCursor(mouseCursor, Vector2.zero, CursorMode.Auto);
            }
        }

        private void ToggleFollowTarget()
        {
            if (IsFollowingMouse() && _initialTarget)
            {
                virtualCamera.Follow = _initialTarget;
                _hasToggledToMouseFollow = false;
            }
            else if (!IsFollowingMouse() && mouseFollowTarget && virtualCamera.Follow)
            {
                _initialTarget = virtualCamera.Follow;
                mouseFollowTarget.position = _initialTarget.position;
                virtualCamera.Follow = mouseFollowTarget;
                _hasToggledToMouseFollow = true;
            }
        }

        private bool IsFollowingMouse() => virtualCamera.Follow == mouseFollowTarget;

        private void SetFOV(float newFOV)
        {
            virtualCamera.m_Lens.FieldOfView = Mathf.Clamp(newFOV, MinFOV, MaxFOV);
            float zoomFactor = Mathf.InverseLerp(MinFOV, MaxFOV, virtualCamera.m_Lens.FieldOfView);
            float targetY = Mathf.Lerp(MinY, MaxY, zoomFactor);

            if (mouseFollowTarget)
            {
                Vector3 targetPosition = mouseFollowTarget.position;
                targetPosition.y = targetY;
                mouseFollowTarget.position = targetPosition;
            }
        }

        private void UpdateMouseFollowTargetPosition()
        {
            if (!mouseFollowTarget) return;

            Plane plane = new(Vector3.up, new Vector3(0, mouseFollowTarget.position.y, 0));
            Ray ray = UnityEngine.Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (!plane.Raycast(ray, out float distance)) return;
            Vector3 mouseWorldPos = ray.GetPoint(distance);
            mouseWorldPos += new Vector3(MouseTargetOffset.x, 0, MouseTargetOffset.y);

            Vector3 nextTargetPosition = new(mouseWorldPos.x, mouseFollowTarget.position.y, mouseWorldPos.z);
            Vector3 direction = nextTargetPosition - mouseFollowTarget.position;

            if (direction.magnitude > MaxTranslationSpeed * Time.deltaTime)
                direction = MaxTranslationSpeed * Time.deltaTime * direction.normalized;

            mouseFollowTarget.position += direction;
        }

        private void UpdateCameraRotation()
        {
            if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                _isRotating = true;
                _lastMousePosition = Mouse.current.position.ReadValue();
            }

            if (_isRotating)
            {
                Vector3 mousePosition = Mouse.current.position.ReadValue();
                Vector3 delta = mousePosition - _lastMousePosition;
                _lastMousePosition = mousePosition;

                float rotationY = delta.x * rotationSpeed * Time.deltaTime;
                Quaternion currentRotation = virtualCamera.transform.rotation;
                float currentX = currentRotation.eulerAngles.x;
                Quaternion targetRotation = Quaternion.Euler(currentX, currentRotation.eulerAngles.y + rotationY, 0f);
                virtualCamera.transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, 0.4f);
            }
        }

        private void OnDestroy()
        {
            if (mouseFollowTarget == null || _initialTarget == null) return;

            mouseFollowTarget.position = _initialTarget.position;
            virtualCamera.Follow = _initialTarget;
        }
    }
}
