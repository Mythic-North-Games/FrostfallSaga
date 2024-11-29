using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Fight.FightCells;
using UnityEngine;

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
            Debug.Log($"Executing internal ability animation {fighterWeaponCollider}.");
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