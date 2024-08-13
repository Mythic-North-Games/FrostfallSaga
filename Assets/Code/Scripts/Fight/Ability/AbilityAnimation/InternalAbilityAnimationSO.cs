using UnityEngine;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Grid.Cells;

namespace FrostfallSaga.Fight.Abilities.AbilityAnimation
{
    [CreateAssetMenu(
        fileName = "InternalAbilityAnimation",
        menuName = "ScriptableObjects/Fight/Abilities/Animations/InternalAbilityAnimation",
        order = 0
    )]
    public class InternalAbilityAnimationSO : AAbilityAnimationSO
    {
        [field: SerializeField] public string AnimationStateName { get; private set; }
        [field: SerializeField, Range(0f, 99f)] public float AnimationDuration { get; private set; }
        [field: SerializeField] public AInternalAbilityAnimationExecutorSO Executor { get; private set; }

        /// <summary>
        /// Executes the internal ability animation as configured.
        /// </summary>
        public override void Execute(Fighter fighterThatWillExecute, Cell[] abilityTargetCells)
        {
            Executor.onFighterTouched += OnFighterTouched;
            Executor.onAnimationEnded += OnExecutorAnimationEnded;

            FighterCollider fighterWeaponCollider = fighterThatWillExecute.GetWeaponCollider();
            Executor.Execute(fighterThatWillExecute, abilityTargetCells, AnimationStateName, AnimationDuration, fighterWeaponCollider);
        }

        protected override void OnExecutorAnimationEnded(Fighter initiator)
        {
            Executor.onFighterTouched -= OnFighterTouched;
            Executor.onAnimationEnded -= OnExecutorAnimationEnded;
            base.OnExecutorAnimationEnded(initiator);
        }
    }
}