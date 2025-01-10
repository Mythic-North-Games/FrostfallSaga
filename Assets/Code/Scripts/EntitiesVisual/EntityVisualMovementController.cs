using System;
using UnityEngine;
using FrostfallSaga.Grid;
using FrostfallSaga.Grid.Cells;

namespace FrostfallSaga.EntitiesVisual
{
    public class EntityVisualMovementController : MonoBehaviour
    {
        [field: SerializeField] public EntityVisualAnimationController EntityVisualAnimationController { get; private set; }
        [field: SerializeField] public float MoveSpeed { get; private set; } = 2.5f;
        [field: SerializeField] public float JumpSpeed { get; private set; } = 2f;
        [field: SerializeField] public float JumpForce { get; private set; } = 6f;
        [field: SerializeField] public float RotationSpeed { get; private set; } = 6f;
        [field: SerializeField] public float ReachedDistanceOffset { get; private set; } = 0.01f;
        public Action onRotationEnded;
        public Action<Cell> onMoveEnded;

        [SerializeField] private GameObject _parentToMove;
        private Rigidbody _rigidbody;
        private bool _isMoving = false;
        private bool _isRotating = false;
        private bool _isLastMove = false;
        private bool _movementIsJump = false;
        private Cell _targetCell;
        private Vector3 _targetCellPosition;
        private Quaternion _targetRotation;

        private void Start()
        {
            EntityVisualAnimationController = GetComponent<EntityVisualAnimationController>();
            _rigidbody = GetComponent<Rigidbody>();
            if (_parentToMove == null)
            {
                _parentToMove = gameObject;
            }
        }

        public void Move(Cell currentCell, Cell newTargetCell, bool isLastMove)
        {
            _targetCell = newTargetCell;
            _isLastMove = isLastMove;

            Vector3 direction = (newTargetCell.GetCenter() - _parentToMove.transform.position).normalized;
            _targetRotation = Quaternion.LookRotation(direction);

            _targetCellPosition = newTargetCell.GetCenter();


            if (CellsNeighbors.GetHeightDifference(currentCell, newTargetCell) == 0)
            {
                EntityVisualAnimationController.PlayAnimationState("Run");
            }
            else
            {
                EntityVisualAnimationController.PlayAnimationState("Jump");
                _rigidbody.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
                _movementIsJump = true;
            }

            // Launch rotation and movement when everything is computed
            _isMoving = true;
            _isRotating = true;
        }

        public void RotateTowardsCell(Cell targetCell)
        {
            _targetRotation = Quaternion.LookRotation(targetCell.GetCenter());
            _targetCellPosition = targetCell.GetCenter();
            _isRotating = true;
        }

        public void TeleportToCell(Cell targetCell)
        {
            _parentToMove.transform.position = targetCell.GetCenter();
        }

        public void UpdateParentToMove(GameObject newParentToMove)
        {
            _parentToMove = newParentToMove;
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
                _movementIsJump = false;
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
        }

        private void MakeParentMoveTowardsTarget()
        {
            float currentMovementSpeed = _movementIsJump ? JumpSpeed : MoveSpeed;
            _parentToMove.transform.position = Vector3.MoveTowards(
                _parentToMove.transform.position, _targetCellPosition, currentMovementSpeed * Time.deltaTime
            );
        }

        private bool IsMovingOrRotating()
        {
            return _isMoving || _isRotating;
        }

        private bool HasReachedTargetRotation()
        {
            return Quaternion.Angle(_parentToMove.transform.rotation, _targetRotation) < 0.1f;
        }

        private bool HasReachedTargetLocation()
        {
            return _parentToMove.transform.position == _targetCellPosition;
        }

#if UNITY_EDITOR
        public void SetParentToMoveForTests(GameObject parentToMove)
        {
            _parentToMove = parentToMove;
        }
#endif
    }
}