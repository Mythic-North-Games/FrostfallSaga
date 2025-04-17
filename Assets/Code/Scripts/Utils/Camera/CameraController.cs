using System;
using Cinemachine;
using UnityEngine;
using UnityEngine.UIElements;

namespace FrostfallSaga.Utils.Camera
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera virtualCamera;

        [field: SerializeField] public bool AllowZoom { get; private set; } = true;
        [field: SerializeField] public float MinFOV { get; private set; } = 20.0f;
        [field: SerializeField] public float MaxFOV { get; private set; } = 120.0f;
        [field: SerializeField] public float BaseFOV { get; private set; } = 75.0f;
        [field: SerializeField] public float ZoomMultiplier { get; private set; } = 5.0f;

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
            UnityEngine.Cursor.SetCursor(mouseCursor, Vector2.zero, CursorMode.Auto);
        }

        private void Start()
        {
            SetFOV(BaseFOV);
        }

        private void Update()
        {
            if (AllowZoom)
            {
                float deltaZoom = -Input.mouseScrollDelta.y * ZoomMultiplier;
                SetFOV(virtualCamera.m_Lens.FieldOfView + deltaZoom);
            }

            if (Input.GetMouseButtonUp((int)MouseButton.MiddleMouse))
            {
                ToggleFollowTarget();
            }

            if (Input.GetMouseButtonDown((int)MouseButton.LeftMouse))
            {
                _mouseDragStartPosition = Input.mousePosition;
                _isDragging = false;
            }

            if (Input.GetMouseButton((int)MouseButton.LeftMouse))
            {
                if (!_isDragging && Vector3.Distance(Input.mousePosition, _mouseDragStartPosition) > DRAG_THRESHOLD)
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
                    UnityEngine.Cursor.SetCursor(grabCursor, Vector2.zero, CursorMode.Auto);
                    UpdateMouseFollowTargetPosition();
                }
            }

            if (Input.GetMouseButton((int)MouseButton.RightMouse))
            {
                UnityEngine.Cursor.SetCursor(grabCursor, Vector2.zero, CursorMode.Auto);
                UpdateCameraRotation();
            }

            if (Input.GetMouseButtonUp((int)MouseButton.LeftMouse) || Input.GetMouseButtonUp((int)MouseButton.RightMouse))
            {
                _isDragging = false;
                UnityEngine.Cursor.SetCursor(mouseCursor, Vector2.zero, CursorMode.Auto);
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
            virtualCamera.m_Lens.FieldOfView = Math.Clamp(newFOV, MinFOV, MaxFOV);
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
            Ray ray = UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition);

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
            if (Input.GetMouseButtonDown(1))
            {
                _isRotating = true;
                _lastMousePosition = Input.mousePosition;
            }
            else if (Input.GetMouseButtonUp(1))
            {
                _isRotating = false;
            }

            if (_isRotating)
            {
                Vector3 delta = Input.mousePosition - _lastMousePosition;
                _lastMousePosition = Input.mousePosition;

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