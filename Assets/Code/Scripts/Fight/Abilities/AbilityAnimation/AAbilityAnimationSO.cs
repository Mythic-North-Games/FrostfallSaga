using System;
using FrostfallSaga.Fight.FightCells;
using FrostfallSaga.Fight.Fighters;
using UnityEngine;

namespace FrostfallSaga.Fight.Abilities.AbilityAnimation
{
    public abstract class AAbilityAnimationSO : ScriptableObject
    {
        public Action<Fighter> onAnimationEnded;
        public Action<FightCell> onCellTouched;
        public Action<Fighter> onFighterTouched;

        protected AbilityCameraFollow _cameraFollow;

        /// <summary>
        ///     Executes the ability animation as configured.
        /// </summary>
        public abstract void Execute(Fighter fighterThatWillExecute, FightCell[] abilityTargetCells);

        protected virtual void OnFighterTouched(Fighter touchedFighter)
        {
            onFighterTouched?.Invoke(touchedFighter);
        }

        protected virtual void OnCellTouched(FightCell touchedCell)
        {
            onCellTouched?.Invoke(touchedCell);
        }

        protected virtual void OnExecutorAnimationEnded(Fighter initiator)
        {
            onAnimationEnded?.Invoke(initiator);
        }

        protected void FindFollowCamera()
        {
            _cameraFollow = FindObjectOfType<AbilityCameraFollow>();
            if (_cameraFollow == null)
            {
                Debug.LogError("AbilityCameraFollow not found in the scene.");
            }
        }
    }
}