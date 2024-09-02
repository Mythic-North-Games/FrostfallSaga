using UnityEngine;
using Cinemachine;
using System;

namespace FrostfallSaga.Core
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera _camera;

        [SerializeField, Header("Zoom"), Tooltip("Min FOV possible for zoom.")] private float _minFOV = 20.0f;
        [SerializeField, Tooltip("Max FOV possible for zoom.")] private float _maxFOV = 120.0f;
        [SerializeField, Tooltip("Base FOV.")] private float _baseFOV = 75.0f;
        [SerializeField, Tooltip("Multiplier added to the scroll delta.")] private float _zoomMultiplier = 10.0f;

        private void Start()
        {
            SetFOV(_baseFOV);
        }

        private void Update()
        {
            SetFOV(_camera.m_Lens.FieldOfView - Input.mouseScrollDelta.y * _zoomMultiplier);
        }

        private void SetFOV(float newFOV)
        {
            _camera.m_Lens.FieldOfView = Math.Clamp(newFOV, _minFOV, _maxFOV);
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