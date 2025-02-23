using System;
using UnityEngine;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Fight.FightCells;

namespace FrostfallSaga.Fight.Abilities.AbilityAnimation
{
    public abstract class AAbilityAnimationSO : ScriptableObject
    {
        public Action<Fighter> onFighterTouched;
        public Action<FightCell> onCellTouched;
        public Action<Fighter> onAnimationEnded;


        /// <summary>
        /// Executes the ability animation as configured.
        /// </summary>
        public abstract void Execute(Fighter fighterThatWillExecute, FightCell[] abilityTargetCells);

        protected void OnFighterTouched(Fighter touchedFighter)
        {
            onFighterTouched?.Invoke(touchedFighter);
        }

        protected void OnCellTouched(FightCell touchedCell)
        {
            onCellTouched?.Invoke(touchedCell);
        }

        protected virtual void OnExecutorAnimationEnded(Fighter initiator)
        {
            onAnimationEnded?.Invoke(initiator);
        }
    }
}