using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Fight.FightCells;
using UnityEngine;

namespace FrostfallSaga.Fight.Abilities.AbilityAnimation
{
  
    public class ExternalAbilityAnimationSO : AAbilityAnimation
    {
        [field: SerializeField] public GameObject ProjectilePrefab { get; private set; }
        [SerializeReference] public AExternalAbilityAnimationExecutor Executor;

        /// <summary>
        /// Executes the external ability animation as configured.
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