using System;
using System.Collections;
using UnityEngine;
using FrostfallSaga.Grid.Cells;
using FrostfallSaga.Fight.Fighters;

namespace FrostfallSaga.Fight.Abilities.AbilityAnimation
{

    [Serializable]
    public class ReturnToOriginCellInternalExecutor : AInternalAbilityAnimationExecutor
    {
        private Cell[] _abilityTargetCells;
        private string _animationStateNameToTrigger;
        private float _animationDuration;
        private int _currentExecutionCellIndex;

        public override void Execute(
            Fighter fighterThatWillExecute,
            Cell[] abilityTargetCells,
            string animationStateNameToTrigger,
            float animationDuration,
            FighterCollider colliderToTrack
        )
        {
            if (abilityTargetCells.Length == 0)
            {
                Debug.LogWarning("Can't execute ability animation without target cells.");
                return;
            }

            _fighterThatExecutes = fighterThatWillExecute;
            _fighterThatExecutes.UnsubscribeToMovementControllerEvents();
            _fighterThatExecutes.MovementController.onMoveEnded += OnFighterMoveEnded;

            _abilityTargetCells = abilityTargetCells;

            _animationStateNameToTrigger = animationStateNameToTrigger;
            _animationDuration = animationDuration;
            SetupFighterColliderEvent(colliderToTrack);

            _currentExecutionCellIndex = -1;
            MoveToNextTargetCell();
        }

        private void MoveToNextTargetCell()
        {
            _currentExecutionCellIndex++;
            Cell nextTargetCell = _abilityTargetCells[_currentExecutionCellIndex];
            _fighterThatExecutes.MovementController.Move(_fighterThatExecutes.cell, nextTargetCell, true);
        }

        private void OnFighterMoveEnded(Cell destinationCell)
        {
            if (destinationCell != _fighterThatExecutes.cell)
            {
                _fighterThatExecutes.AnimationController.PlayAnimationState(_animationStateNameToTrigger);
                _fighterThatExecutes.StartCoroutine(WaitAndComeBackToOriginCell(destinationCell));
            }
            else if (_currentExecutionCellIndex + 1 < _abilityTargetCells.Length)
            {
                MoveToNextTargetCell();
            }
            else
            {
                _fighterThatExecutes.TrySetupEntitiyVisualMoveController();
                onAnimationEnded?.Invoke(_fighterThatExecutes);
            }
        }

        private IEnumerator WaitAndComeBackToOriginCell(Cell currentCell)
        {
            yield return new WaitForSeconds(_animationDuration);
            _fighterThatExecutes.MovementController.Move(currentCell, _fighterThatExecutes.cell, true);
        }
    }
}
