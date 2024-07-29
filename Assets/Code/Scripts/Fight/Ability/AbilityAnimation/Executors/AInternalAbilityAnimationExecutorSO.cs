using System;
using UnityEngine;
using FrostfallSaga.Grid.Cells;
using FrostfallSaga.Fight.Fighters;

namespace FrostfallSaga.Fight.Abilities.AbilityAnimation
{
    /// <summary>
    /// Base class for all internal abilities animation executors.
    /// Exposes a onFighterTouched and onAnimationEnded events that will be triggered after the call of the Execute method.
    /// </summary>
    public abstract class AInternalAbilityAnimationExecutorSO : ScriptableObject
    {
        public Action<Fighter> onFighterTouched;
        public Action<Fighter> onAnimationEnded;

        protected Fighter _fighterThatExecutes;

        /// <summary>
        /// Executes the given animation as defined by the executor.
        /// </summary>
        /// <param name="fighterThatWillExecute">The fighter that will execute the animation.</param>
        /// <param name="abilityTargetCells">The cells the ability targets.</param>
        /// <param name="animationStateNameToTrigger">The name of the animation state to trigger.</param>
        /// <param name="animationDuration">The duration of the animation.</param>
        /// <param name="colliderToTrack">The weapon or other collider to track for collision.</param>
        public abstract void Execute(
            Fighter fighterThatWillExecute,
            Cell[] abilityTargetCells,
            string animationStateNameToTrigger,
            float animationDuration,
            FighterCollider colliderToTrack
        );

        protected void SetupFighterColliderEvent(FighterCollider collider)
        {
            collider.onFighterEnter += OnFighterEnteredCollider;
        }

        protected void OnFighterEnteredCollider(Fighter fighter)
        {
            if (fighter != _fighterThatExecutes)
            {
                onFighterTouched?.Invoke(fighter);
            }
        }
    }
}