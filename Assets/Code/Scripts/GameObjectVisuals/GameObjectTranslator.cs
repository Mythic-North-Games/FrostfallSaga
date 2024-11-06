using System;
using UnityEngine;

namespace FrostfallSaga.GameObjectVisuals
{
    /// <summary>
    /// Helper class to move an object around the world.
    /// </summary>
    public class GameObjectTranslator : MonoBehaviour
    {
        public Action<Transform> onTargetLocationReached;
        public Action<Transform> onTargetRotationReached;

        private Transform _objectToMove;
        private Vector3 _targetLocation;
        private Quaternion _targetRotation;
        private float _translationSpeed;
        private float _rotationSpeed;
        private bool _isMoving = false;
        private bool _isRotating = false;

        public void TranslateTo(Transform objectToMove, Vector3 targetLocation, float translationSpeed = 1f)
        {
            _objectToMove = objectToMove;
            _targetLocation = targetLocation;
            _translationSpeed = translationSpeed;
            _isMoving = true;
        }

        public void TranslateAndFaceTo(Transform objectToMove, Vector3 targetLocation, float translationSpeed = 1f, float rotationSpeed = 1f)
        {
            _objectToMove = objectToMove;
            _targetLocation = targetLocation;
            _translationSpeed = translationSpeed;
            _rotationSpeed = rotationSpeed;
            _isMoving = true;
            _isRotating = true;
        }

        private void Update()
        {
            if (!IsMovingOrRotating())
            {
                return;
            }

            if (HasReachedTargetLocation())
            {
                _isMoving = false;
                onTargetLocationReached?.Invoke(_objectToMove);
            }

            if (HasReachedTargetRotation())
            {
                _isRotating = false;
                onTargetRotationReached?.Invoke(_objectToMove);
            }

            if (_isMoving && _isRotating)
            {
                MakeObjectRotateTowardsTargetLocation();
                MakeObjectMoveTowardsTargetLocation();
            }
            else if (_isMoving && !_isRotating)
            {
                MakeObjectMoveTowardsTargetLocation();
            }
        }

        private void MakeObjectMoveTowardsTargetLocation()
        {
            _objectToMove.transform.localPosition = Vector3.MoveTowards(
                _objectToMove.transform.localPosition, _targetLocation, _translationSpeed * Time.deltaTime
            );
        }

        private void MakeObjectRotateTowardsTargetLocation()
        {
            Vector3 nextRotation = Vector3.RotateTowards(
                _objectToMove.transform.forward,
                new Vector3(_targetLocation.x, 0, _targetLocation.z) -
                new Vector3(_objectToMove.transform.localPosition.x, 0, _objectToMove.transform.localPosition.z),
                _rotationSpeed * Time.deltaTime, 0.0f
            );
            _objectToMove.transform.localRotation = Quaternion.LookRotation(nextRotation);
        }

        private bool IsMovingOrRotating()
        {
            return _isMoving || _isRotating;
        }

        private bool HasReachedTargetRotation()
        {
            return _isRotating && _objectToMove.transform.localRotation == _targetRotation;
        }

        private bool HasReachedTargetLocation()
        {
            return _isMoving && _objectToMove.transform.localPosition == _targetLocation;
        }
    }
}