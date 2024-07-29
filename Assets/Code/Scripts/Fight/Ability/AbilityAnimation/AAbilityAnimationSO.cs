using UnityEngine;
using FrostfallSaga.Grid.Cells; 
using FrostfallSaga.Fight.Fighters;
using System;

namespace FrostfallSaga.Fight.Abilities.AbilityAnimation
{
    public abstract class AAbilityAnimationSO : ScriptableObject
    {
        public Action<Fighter> onFighterTouched;
        public Action<Fighter> onAnimationEnded;

        /// <summary>
        /// Executes the ability animation as configured.
        /// </summary>
        public abstract void Execute(Fighter fighterThatWillExecute, Cell[] abilityTargetCells);

        protected void OnFighterTouched(Fighter touchedFighter)
        {
            onFighterTouched?.Invoke(touchedFighter);
        }

        protected virtual void OnExecutorAnimationEnded(Fighter initiator)
        {
            onAnimationEnded?.Invoke(initiator);
        }
    }
}