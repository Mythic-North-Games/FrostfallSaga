using System;
using UnityEngine;
using UnityEngine.UIElements;
using Cinemachine;

namespace FrostfallSaga.Core
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera _camera;

        [SerializeField, Header("Zoom"), Tooltip("To activate or deactivate zoom.")] private bool _allowZoom = true;
        [SerializeField, Tooltip("Min FOV possible for zoom.")] private float _minFOV = 20.0f;
        [SerializeField, Tooltip("Max FOV possible for zoom.")] private float _maxFOV = 120.0f;
        [SerializeField, Tooltip("Base FOV.")] private float _baseFOV = 75.0f;
        [SerializeField, Tooltip("Multiplier added to the scroll delta.")] private float _zoomMultiplier = 5.0f;

        [SerializeField, Header("Translation"), Tooltip("To activate or deactivate translation.")] private bool _allowTranslation = true;
        [SerializeField, Tooltip("The invisible target that the camera will follow.")] private Transform _mouseFollowTarget;
        [SerializeField, Tooltip("Offset to keep camera away from edges.")] private Vector2 _mouseTargetOffset = new(0.1f, 0.1f);
        [SerializeField, Tooltip("The amount of smoothing to apply to the camera movement")] private float _smoothing = 5f;
        [SerializeField, Tooltip("The maximum translation speed for the follow target.")] private float _maxTranslationSpeed = 30f;

        [SerializeField, Tooltip("Min Y position for the follow target (zoomed in).")] private float _minY = 2f;
        [SerializeField, Tooltip("Max Y position for the follow target (zoomed out).")] private float _maxY = 10f;

        private Transform _initialTarget;

        private void Start()
        {
            SetFOV(_baseFOV);
        }

        private void Update()
        {
            if (_allowZoom)
            {
                SetFOV(_camera.m_Lens.FieldOfView - Input.mouseScrollDelta.y * _zoomMultiplier);
            }
            if (_allowTranslation)
            {
                if (Input.GetMouseButtonUp((int)MouseButton.MiddleMouse))
                {
                    _camera.Follow = _initialTarget;
                }
                else if (Input.GetMouseButtonDown((int)MouseButton.RightMouse) && _camera.Follow != null && _camera.Follow != _mouseFollowTarget)
                {
                    _initialTarget = _camera.Follow;
                    _mouseFollowTarget.position = _camera.transform.position = _initialTarget.position;
                    _camera.Follow = _mouseFollowTarget;
                }
                else if (Input.GetMouseButton((int)MouseButton.RightMouse))
                {
                    UpdateMouseFollowTargetPosition();
                }
            }
        }

        private void SetFOV(float newFOV)
        {
            _camera.m_Lens.FieldOfView = Math.Clamp(newFOV, _minFOV, _maxFOV);

            // Normalize the FOV value between 0 and 1
            float zoomFactor = Mathf.InverseLerp(_minFOV, _maxFOV, _camera.m_Lens.FieldOfView);

            // Linearly interpolate the Y position based on the FOV
            float targetY = Mathf.Lerp(_minY, _maxY, zoomFactor);

            // Apply the calculated Y position to the mouse follow target
            Vector3 targetPosition = _mouseFollowTarget.position;
            targetPosition.y = targetY;
            _mouseFollowTarget.position = targetPosition;
        }

        private void UpdateMouseFollowTargetPosition()
        {
            // Raycast from the mouse position into the world, using the plane at the Y position of the target
            Plane plane = new(Vector3.up, new Vector3(0, _mouseFollowTarget.position.y, 0));
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // Raycast to find where the mouse hits the plane
            if (plane.Raycast(ray, out float distance))
            {
                Vector3 mouseWorldPos = ray.GetPoint(distance);

                // Offset the mouse position so the camera doesn't center exactly on the mouse
                mouseWorldPos += new Vector3(_mouseTargetOffset.x, 0, _mouseTargetOffset.y);

                // Move the follow target to the new position (only X and Z axes are affected)
                Vector3 nextTargetPosition = new(mouseWorldPos.x, _mouseFollowTarget.position.y, mouseWorldPos.z);

                // Calculate the distance the follow target would move
                Vector3 direction = nextTargetPosition - _mouseFollowTarget.position;

                // Clamp the magnitude of the direction vector to the maximum speed
                if (direction.magnitude > _maxTranslationSpeed * Time.deltaTime)
                {
                    direction = _maxTranslationSpeed * Time.deltaTime * direction.normalized;
                }

                // Smoothly move the follow target to the mouse position, clamped by max speed
                _mouseFollowTarget.position += direction;
            }
        }

        #region Setup & teardown

        private void Awake()
        {
            if (_camera == null)
            {
                Debug.LogError("No camera to control");
                return;
            }
        }

        #endregion
    }
}
