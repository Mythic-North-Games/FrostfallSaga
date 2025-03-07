using System;
using Cinemachine;
using UnityEngine;
using UnityEngine.UIElements;

namespace FrostfallSaga.Utils.Camera
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera _camera;

        [field: SerializeField]
        [field: Header("Zoom")]
        [field: Tooltip("To activate or deactivate zoom.")]
        public bool AllowZoom { get; private set; } = true;

        [field: SerializeField]
        [field: Tooltip("Min FOV possible for zoom.")]
        public float MinFOV { get; private set; } = 20.0f;

        [field: SerializeField]
        [field: Tooltip("Max FOV possible for zoom.")]
        public float MaxFOV { get; private set; } = 120.0f;

        [field: SerializeField]
        [field: Tooltip("Base FOV.")]
        public float BaseFOV { get; private set; } = 75.0f;

        [field: SerializeField]
        [field: Tooltip("Multiplier added to the scroll delta.")]
        public float ZoomMultiplier { get; private set; } = 5.0f;

        [field: SerializeField]
        [field: Header("Translation")]
        [field: Tooltip("To activate or deactivate translation.")]
        public bool AllowTranslation { get; private set; } = true;

        [field: SerializeField] [field: Tooltip("The invisible target that the camera will follow.")]
        public Transform MouseFollowTarget;

        [field: SerializeField]
        [field: Tooltip("Offset to keep camera away from edges.")]
        public Vector2 MouseTargetOffset { get; private set; } = new(0.1f, 0.1f);

        [field: SerializeField]
        [field: Tooltip("The maximum translation speed for the follow target.")]
        public float MaxTranslationSpeed { get; private set; } = 30f;

        [field: SerializeField]
        [field: Tooltip("Min Y position for the follow target (zoomed in).")]
        public float MinY { get; private set; } = 2f;

        [field: SerializeField]
        [field: Tooltip("Max Y position for the follow target (zoomed out).")]
        public float MaxY { get; private set; } = 10f;

        private Transform _initialTarget;

        #region Setup & teardown

        private void Awake()
        {
            if (_camera == null) Debug.LogError("No camera to control");
        }

        #endregion

        private void Start()
        {
            SetFOV(BaseFOV);
        }

        private void Update()
        {
            if (AllowZoom) SetFOV(_camera.m_Lens.FieldOfView - Input.mouseScrollDelta.y * ZoomMultiplier);
            if (AllowTranslation)
            {
                if (Input.GetMouseButtonUp((int)MouseButton.MiddleMouse))
                {
                    _camera.Follow = _initialTarget;
                }
                else if (Input.GetMouseButtonDown((int)MouseButton.RightMouse) && _camera.Follow != null &&
                         _camera.Follow != MouseFollowTarget)
                {
                    _initialTarget = _camera.Follow;
                    MouseFollowTarget.position = _camera.transform.position = _initialTarget.position;
                    _camera.Follow = MouseFollowTarget;
                }
                else if (Input.GetMouseButton((int)MouseButton.RightMouse))
                {
                    UpdateMouseFollowTargetPosition();
                }
            }
        }

        private void SetFOV(float newFOV)
        {
            _camera.m_Lens.FieldOfView = Math.Clamp(newFOV, MinFOV, MaxFOV);

            // Normalize the FOV value between 0 and 1
            float zoomFactor = Mathf.InverseLerp(MinFOV, MaxFOV, _camera.m_Lens.FieldOfView);

            // Linearly interpolate the Y position based on the FOV
            float targetY = Mathf.Lerp(MinY, MaxY, zoomFactor);

            // Apply the calculated Y position to the mouse follow target
            Vector3 targetPosition = MouseFollowTarget.position;
            targetPosition.y = targetY;
            MouseFollowTarget.position = targetPosition;
        }

        private void UpdateMouseFollowTargetPosition()
        {
            // Raycast from the mouse position into the world, using the plane at the Y position of the target
            Plane plane = new(Vector3.up, new Vector3(0, MouseFollowTarget.position.y, 0));
            Ray ray = UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition);

            // Raycast to find where the mouse hits the plane
            if (plane.Raycast(ray, out float distance))
            {
                Vector3 mouseWorldPos = ray.GetPoint(distance);

                // Offset the mouse position so the camera doesn't center exactly on the mouse
                mouseWorldPos += new Vector3(MouseTargetOffset.x, 0, MouseTargetOffset.y);

                // Move the follow target to the new position (only X and Z axes are affected)
                Vector3 nextTargetPosition = new(mouseWorldPos.x, MouseFollowTarget.position.y, mouseWorldPos.z);

                // Calculate the distance the follow target would move
                Vector3 direction = nextTargetPosition - MouseFollowTarget.position;

                // Clamp the magnitude of the direction vector to the maximum speed
                if (direction.magnitude > MaxTranslationSpeed * Time.deltaTime)
                    direction = MaxTranslationSpeed * Time.deltaTime * direction.normalized;

                // Smoothly move the follow target to the mouse position, clamped by max speed
                MouseFollowTarget.position += direction;
            }
        }
    }
}