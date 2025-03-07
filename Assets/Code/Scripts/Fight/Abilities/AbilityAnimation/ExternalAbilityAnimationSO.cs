using FrostfallSaga.Fight.FightCells;
using FrostfallSaga.Fight.Fighters;
using UnityEngine;

namespace FrostfallSaga.Fight.Abilities.AbilityAnimation
{
    [CreateAssetMenu(
        fileName = "ExternalAbilityAnimation",
        menuName = "ScriptableObjects/Fight/Abilities/Animations/ExternalAbilityAnimation",
        order = 0
    )]
    public class ExternalAbilityAnimationSO : AAbilityAnimationSO
    {
        [field: SerializeField] public GameObject ProjectilePrefab { get; private set; }
        [SerializeReference] public AExternalAbilityAnimationExecutor Executor;

        /// <summary>
        ///     Executes the external ability animation as configured.
        /// </summary>
        public override void Execute(Fighter fighterThatWillExecute, FightCell[] abilityTargetCells)
        {
            Executor.onFighterTouched += OnFighterTouched;
            Executor.onCellTouched += OnCellTouched;
            Executor.onAnimationEnded += OnExecutorAnimationEnded;

            Executor.Execute(fighterThatWillExecute, abilityTargetCells, ProjectilePrefab);
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