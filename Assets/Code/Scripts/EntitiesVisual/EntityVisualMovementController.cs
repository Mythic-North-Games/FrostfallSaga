using System;
using UnityEngine;
using FrostfallSaga.Core;
using FrostfallSaga.Grid;
using FrostfallSaga.Grid.Cells;

namespace FrostfallSaga.EntitiesVisual
{
    public class EntityVisualMovementController : MonoBehaviour
    {
        [field: SerializeField] public EntityVisualAnimationController EntityVisualAnimationController { get; private set; }
        [field: SerializeField] public float MoveSpeed { get; private set; } = 2.5f;
        [field: SerializeField] public float RotationSpeed { get; private set; } = 6f;
        [field: SerializeField] public float ReachedDistanceOffset { get; private set; } = 0.01f;
        public Action onRotationEnded;
        public Action<Cell> onMoveEnded;

        [SerializeField] private GameObject _parentToMove;
        private bool _isMoving = false;
        private bool _isRotating = false;
        private bool _isLastMove = false;
        private Cell _targetCell;
        private Vector3 _targetCellPosition;
        private Quaternion _targetRotation;
        private Quaternion _lastRotationStep;

        private void Start()
        {
            EntityVisualAnimationController = GetComponent<EntityVisualAnimationController>();
            if (_parentToMove == null)
            {
                _parentToMove = gameObject;
            }
        }

        public void Move(Cell currentCell, Cell newTargetCell, bool isLastMove)
        {
            _targetCell = newTargetCell;
            _targetCellPosition = newTargetCell.GetCenter();
    
            RotateTowardsCell(newTargetCell);
            _isMoving = true;
            _isLastMove = isLastMove;

            if (CellsNeighbors.GetHeightDifference(currentCell, newTargetCell) == 0)
            {
                EntityVisualAnimationController.PlayAnimationState("Run");
            }
            else
            {
                EntityVisualAnimationController.PlayAnimationState("Jump");
            }
        }
        
        public void RotateTowardsCell(Cell targetCell)
        {
            _targetRotation = Quaternion.LookRotation(targetCell.GetCenter());
            _targetCellPosition = targetCell.GetCenter();
            _isRotating = true;
        }

        private void Update()
        {
            if (!IsMovingOrRotating())
            {
                return;
            }

            if (HasReachedTargetRotation())
            {
                _isRotating = false;
                onRotationEnded?.Invoke();
            }

            if (HasReachedTargetLocation())
            {
                _isMoving = false;
                if (_isLastMove)
                {
                    EntityVisualAnimationController.RestoreDefaultAnimation();
                }
                onMoveEnded?.Invoke(_targetCell);
            }

            if (_isRotating)
            {
                MakeParentRotateTowardsTarget();
            }
            if (_isMoving)
            {
                MakeParentMoveTowardsTarget();
            }
        }

        private void MakeParentRotateTowardsTarget()
        {
            Vector3 nextRotation = Vector3.RotateTowards(
                _parentToMove.transform.forward,
                new Vector3(_targetCellPosition.x, 0, _targetCellPosition.z) - new Vector3(_parentToMove.transform.position.x, 0, _parentToMove.transform.position.z),
                RotationSpeed * Time.deltaTime, 0.0f
            );
            _parentToMove.transform.rotation = Quaternion.LookRotation(nextRotation);
            _lastRotationStep = Quaternion.LookRotation(nextRotation);
        }

        private void MakeParentMoveTowardsTarget()
        {
            _parentToMove.transform.position = Vector3.MoveTowards(
                _parentToMove.transform.position, _targetCellPosition, MoveSpeed * Time.deltaTime
            );
        }

        private bool IsMovingOrRotating()
        {
            return _isMoving || _isRotating;
        }

        private bool HasReachedTargetRotation()
        {
            return _isRotating && _parentToMove.transform.rotation == _targetRotation;
        }

        private bool HasReachedTargetLocation()
        {
            return _isMoving && _parentToMove.transform.position == _targetCellPosition;
        }

        #if UNITY_EDITOR
            public void SetParentToMoveForTests(GameObject parentToMove)
            {
                _parentToMove = parentToMove;
            }
        #endif
    }
}