using System;
using System.Collections;
using FrostfallSaga.Grid;
using FrostfallSaga.Grid.Cells;
using UnityEngine;

namespace FrostfallSaga.EntitiesVisual
{
    public class EntityVisualMovementController : MonoBehaviour
    {
        [field: SerializeField]
        public EntityVisualAnimationController EntityVisualAnimationController { get; private set; }

        [field: SerializeField] public float RunSpeed { get; private set; } = 1f;
        [field: SerializeField] public float JumpSpeed { get; private set; } = 1.5f;
        [field: SerializeField] public float RotationSpeed { get; private set; } = 6f;

        [SerializeField] private GameObject _parentToMove;
        public Action<Cell> onMoveEnded;
        public Action onRotationEnded;

        private void Start()
        {
            EntityVisualAnimationController = GetComponent<EntityVisualAnimationController>();
            if (_parentToMove == null) _parentToMove = gameObject;
        }

        public void Move(Cell currentCell, Cell newTargetCell, bool isLastMove)
        {
            var movementIsJump = CellsNeighbors.GetHeightDifference(currentCell, newTargetCell) != 0;
            if (movementIsJump)
                EntityVisualAnimationController.PlayAnimationState("Jump");
            else
                EntityVisualAnimationController.PlayAnimationState("Run");

            MoveTowardsCell(currentCell, newTargetCell, movementIsJump, isLastMove);
            RotateTowardsCell(newTargetCell);
        }

        public void RotateTowardsCell(Cell targetCell)
        {
            Vector3 direction = targetCell.GetCenter() - _parentToMove.transform.position;
            if (direction.sqrMagnitude > 0.001f) // Validate the direction vector
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction.normalized);
                StartCoroutine(SmoothRotate(_parentToMove.transform.rotation, targetRotation, 1f / RotationSpeed));
            }
        }

        public void TeleportToCell(Cell targetCell)
        {
            _parentToMove.transform.position = targetCell.GetCenter();
        }

        public void UpdateParentToMove(GameObject newParentToMove)
        {
            _parentToMove = newParentToMove;
        }

        private void MoveTowardsCell(Cell currentCell, Cell targetCell, bool isJump, bool isLastMove)
        {
            StartCoroutine(SmoothMove(currentCell, targetCell, isJump, isLastMove));
        }

        private IEnumerator SmoothMove(Cell currentCell, Cell targetCell, bool isJump, bool isLastMove)
        {
            var duration = 1f / (isJump ? JumpSpeed : RunSpeed);
            var elapsedTime = 0f;

            float heightDifference = CellsNeighbors.GetHeightDifference(currentCell, targetCell);
            var jumpHeight = Mathf.Max(heightDifference, 1f);
            if (targetCell.Height > currentCell.Height)
                jumpHeight += 1.5f;
            else if (targetCell.Height < currentCell.Height) jumpHeight += 1f;

            Vector3 startPosition = _parentToMove.transform.position;
            Vector3 endPosition = targetCell.GetCenter();

            while (elapsedTime < duration)
            {
                var t = elapsedTime / duration;

                t = Mathf.Lerp(0, 1, t); // Linear interpolation for movement
                Vector3 position = Vector3.Lerp(startPosition, endPosition, t);

                if (isJump)
                {
                    var height = Mathf.Sin(t * Mathf.PI) * jumpHeight; // Arc height
                    position.y += height;
                }

                _parentToMove.transform.position = position;
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            _parentToMove.transform.position = endPosition;

            if (isLastMove) EntityVisualAnimationController.RestoreDefaultAnimation();
            onMoveEnded?.Invoke(targetCell);
        }

        private IEnumerator SmoothRotate(Quaternion startRotation, Quaternion targetRotation, float duration)
        {
            var elapsedTime = 0f;

            // Lock target rotation to the Y-axis only
            Vector3 targetEulerAngles = targetRotation.eulerAngles;
            targetEulerAngles.x = 0f;
            targetEulerAngles.z = 0f;
            targetRotation = Quaternion.Euler(targetEulerAngles);

            while (elapsedTime < duration)
            {
                Quaternion smoothedRotation = Quaternion.Slerp(startRotation, targetRotation, elapsedTime / duration);

                // Lock the interpolated rotation to the Y-axis only
                Vector3 smoothedEulerAngles = smoothedRotation.eulerAngles;
                smoothedEulerAngles.x = 0f;
                smoothedEulerAngles.z = 0f;
                _parentToMove.transform.rotation = Quaternion.Euler(smoothedEulerAngles);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Ensure final rotation is locked to the Y-axis
            Vector3 finalEulerAngles = targetRotation.eulerAngles;
            finalEulerAngles.x = 0f;
            finalEulerAngles.z = 0f;
            _parentToMove.transform.rotation = Quaternion.Euler(finalEulerAngles);

            onRotationEnded?.Invoke();
        }

#if UNITY_EDITOR
        public void SetParentToMoveForTests(GameObject parentToMove)
        {
            _parentToMove = parentToMove;
        }

        public void SetAnimationControllerForTests()
        {
            EntityVisualAnimationController = GetComponent<EntityVisualAnimationController>();
        }
#endif
    }
}