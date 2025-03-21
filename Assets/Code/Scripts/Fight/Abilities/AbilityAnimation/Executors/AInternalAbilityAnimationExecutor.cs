using System;
using FrostfallSaga.Fight.FightCells;
using FrostfallSaga.Fight.Fighters;

namespace FrostfallSaga.Fight.Abilities.AbilityAnimation
{
    /// <summary>
    ///     Base class for all internal abilities animation executors.
    ///     Exposes a onFighterTouched and onAnimationEnded events that will be triggered after the call of the Execute method.
    /// </summary>
    [Serializable]
    public abstract class AInternalAbilityAnimationExecutor
    {
        protected Fighter _fighterThatExecutes;
        public Action<Fighter> onAnimationEnded;
        public Action<FightCell> onCellTouched;
        public Action<Fighter> onFighterTouched;

        /// <summary>
        ///     Executes the given animation as defined by the executor.
        /// </summary>
        /// <param name="fighterThatWillExecute">The fighter that will execute the animation.</param>
        /// <param name="abilityTargetCells">The cells the ability targets.</param>
        /// <param name="animationStateNameToTrigger">The name of the animation state to trigger.</param>
        /// <param name="animationDuration">The duration of the animation.</param>
        /// <param name="colliderToTrack">The weapon or other collider to track for collision.</param>
        public abstract void Execute(
            Fighter fighterThatWillExecute,
            FightCell[] abilityTargetCells,
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
            if (fighter != _fighterThatExecutes) onFighterTouched?.Invoke(fighter);
        }
    }
}