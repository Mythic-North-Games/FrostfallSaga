using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Fight.FightCells;
using UnityEngine;

namespace FrostfallSaga.Fight.Abilities.AbilityAnimation
{

    public class InternalAbilityAnimationSO : AAbilityAnimation
    {
        [field: SerializeField] public string AnimationStateName { get; private set; }
        [field: SerializeField, Range(0f, 99f)] public float AnimationDuration { get; private set; }
        [SerializeReference] public AInternalAbilityAnimationExecutor Executor;

        /// <summary>
        /// Executes the internal ability animation as configured.
        /// </summary>
        public override void Execute(Fighter fighterThatWillExecute, FightCell[] abilityTargetCells)
        {
            Executor.onFighterTouched += OnFighterTouched;
            Executor.onCellTouched += OnCellTouched;
            Executor.onAnimationEnded += OnExecutorAnimationEnded;

            FighterCollider fighterWeaponCollider = fighterThatWillExecute.GetWeaponCollider();
            Executor.Execute(fighterThatWillExecute, abilityTargetCells, AnimationStateName, AnimationDuration, fighterWeaponCollider);
        }

        protected override void OnExecutorAnimationEnded(Fighter initiator)
        {
            Executor.onFighterTouched -= OnFighterTouched;
            Executor.onCellTouched -= OnCellTouched;
            Executor.onAnimationEnded -= OnExecutorAnimationEnded;
            base.OnExecutorAnimationEnded(initiator);
        }
    }
}