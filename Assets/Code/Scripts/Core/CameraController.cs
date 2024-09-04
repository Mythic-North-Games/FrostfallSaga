using UnityEngine;
using Cinemachine;
using System;

namespace FrostfallSaga.Core
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera _camera;

        [SerializeField, Header("Zoom"), Tooltip("To activate or deactivate zoom.")] private bool _allowZoom = true;
        [SerializeField, Tooltip("Min FOV possible for zoom.")] private float _minFOV = 20.0f;
        [SerializeField, Tooltip("Max FOV possible for zoom.")] private float _maxFOV = 120.0f;
        [SerializeField, Tooltip("Base FOV.")] private float _baseFOV = 75.0f;
        [SerializeField, Tooltip("Multiplier added to the scroll delta.")] private float _zoomMultiplier = 10.0f;

        [SerializeField, Header("Translation"), Tooltip("To activate or deactivate translation.")] private bool _allowTranslation = true;
        [SerializeField, Tooltip("Edge border detection width.")] private Vector2 _edgeBorderWidthRatio = new(0.05f, 0.05f);
        [SerializeField, Tooltip("Translation speed.")] private float _translationSpeed = 15.0f;

        private Vector2 _edgeBorderWidth;

        private void Start()
        {
            SetFOV(_baseFOV);
            _edgeBorderWidth = new(Screen.width * _edgeBorderWidthRatio.x, Screen.height * _edgeBorderWidthRatio.y);
        }

        private void Update()
        {
            if (_allowZoom)
            {
                SetFOV(_camera.m_Lens.FieldOfView - Input.mouseScrollDelta.y * _zoomMultiplier);
            }
            if (_allowTranslation)
            {
                UpdateCameraPosition();
            }
        }

        private void SetFOV(float newFOV)
        {
            _camera.m_Lens.FieldOfView = Math.Clamp(newFOV, _minFOV, _maxFOV);
        }

        private void UpdateCameraPosition()
        {
            Vector3 currentPosition = _camera.transform.position;
            Vector3 targetPosition = currentPosition;
            float horizontalSpeedFactor = 0;
            float verticalSpeedFactor = 0;

            // Check horizontal edges and adjust target position accordingly
            if (Input.mousePosition.x >= Screen.width - _edgeBorderWidth.x)
            {
                DetachIfFollowing();
                horizontalSpeedFactor = GetSpeedFactor(Input.mousePosition.x, _edgeBorderWidth.x, Screen.width);
                targetPosition.x += _translationSpeed * horizontalSpeedFactor;
            }
            else if (Input.mousePosition.x <= _edgeBorderWidth.x)
            {
                DetachIfFollowing();
                horizontalSpeedFactor = GetSpeedFactor(Input.mousePosition.x, _edgeBorderWidth.x, Screen.width);
                targetPosition.x -= _translationSpeed * horizontalSpeedFactor;
            }

            // Check vertical edges and adjust target position accordingly
            if (Input.mousePosition.y >= Screen.height - _edgeBorderWidth.y)
            {
                DetachIfFollowing();
                verticalSpeedFactor = GetSpeedFactor(Input.mousePosition.y, _edgeBorderWidth.y, Screen.height);
                targetPosition.z += _translationSpeed * verticalSpeedFactor;
            }
            else if (Input.mousePosition.y <= _edgeBorderWidth.y)
            {
                DetachIfFollowing();
                verticalSpeedFactor = GetSpeedFactor(Input.mousePosition.y, _edgeBorderWidth.y, Screen.height);
                targetPosition.z -= _translationSpeed * verticalSpeedFactor;
            }

            // Compute max distance delta
            float maxDistanceDelta = Time.deltaTime * _translationSpeed;
            if (horizontalSpeedFactor > 1)
            {
                maxDistanceDelta *= horizontalSpeedFactor;
            }
            else if (verticalSpeedFactor > 1)
            {
                maxDistanceDelta *= verticalSpeedFactor;
            }

            // Move the camera towards the target position
            _camera.transform.position = Vector3.MoveTowards(currentPosition, targetPosition, maxDistanceDelta);
        }

        private void DetachIfFollowing()
        {
            if (_camera.Follow != null)
            {
                DetachTarget();
            }
        }

        private float GetSpeedFactor(float mousePosition, float edgeBorderWidth, float screenSize)
        {
            // Determine how close the mouse is to the screen edge
            float distanceToEdge = 0f;

            if (mousePosition >= screenSize - edgeBorderWidth)
            {
                distanceToEdge = screenSize - mousePosition; // Distance from right or top edge
            }
            else if (mousePosition <= edgeBorderWidth)
            {
                distanceToEdge = mousePosition; // Distance from left or bottom edge
            }

            // Calculate speed factor based on distance to edge
            float maxDistance = edgeBorderWidth; // Maximum distance within the edge border to start speeding up
            float speedFactor = 1f + (1f - (distanceToEdge / maxDistance)); // Scale speed factor from 1 to 2 as it approaches the edge
            return Mathf.Clamp(speedFactor, 1f, 2f); // Ensure the factor is between 1 and 2
        }

        private void DetachTarget()
        {
            _camera.Follow = null;
            _camera.LookAt = null;
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